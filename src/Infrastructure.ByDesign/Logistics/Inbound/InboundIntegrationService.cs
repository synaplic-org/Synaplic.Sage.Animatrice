using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using RestSharp;
using RestSharp.Authenticators;
using Uni.Scan.Infrastructure.ByDesign.Extentions;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.ByDesign.Logistics.Inbound
{
    public class InboundIntegrationService
    {
        public static async Task<IResult<bool>> SendInboundScans(List<LogisticTaskDetailDTO> outpus,
            string tenantId, string userName, string passWord)
        {
            var oSendableOUtput = outpus.Where(o => o.OpenQuantity > 0 && o.ConfirmQuantity > 0)
                .OrderBy(o => o.LineItemID).ToList();
            if (oSendableOUtput.IsNullOrEmpty()) return await Result<bool>.FailAsync("Nothing to send !");
            var oIds = oSendableOUtput.Select(o => o.LineItemID).Distinct().ToList();


            var oSiteLogisticsTasks = new List<XmlSiteLogisticsTask>();
            var erros = new List<string>();

            foreach (var oId in oIds)
            {
                var oLines = oSendableOUtput.Where(o => o.LineItemID == oId).OrderByDescending(o => o.OutputUUID)
                    .ToList();
                var oReqOuts = new List<XmlMaterialOutput>();
                var oTaskID = string.Empty;
                var oTaskUID = string.Empty;

                foreach (var oLine in oLines)
                {
                    var oOut = new XmlMaterialOutput()
                    {
                        MaterialOutputUUID = oLine.OutputUUID,
                        ProductID = oLine.ProductID,
                        ActualQuantity = new XmlActualQuantity()
                            { Value = oLine.ConfirmQuantity, UnitCode = oLine.ConfirmQuantityUnitCode },
                        TargetLogisticsAreaID = oLine.TargetLogisticsAreaID
                    };
                    if (!string.IsNullOrEmpty(oLine.IdentifiedStockID))
                    {
                        oOut.IdentifiedStockID = oLine.IdentifiedStockID;
                        oOut.ClearIdentifiedStockIDIndicator = true;
                        //var stk = await ODataService.GetStockIDAsync(oOut.IdentifiedStockID, oOut.ProductID);
                        //if (stk.IsNullOrEmpty())
                        //{
                        //    var stkidcreated = await ODataService.CreateStockID(oOut.ProductID, oOut.IdentifiedStockID);
                        //    if (!stkidcreated)
                        //    {
                        //        erros.Add($"Unable to create stock ID {oOut.IdentifiedStockID} ");
                        //    }
                        //}
                    }

                    if (!string.IsNullOrWhiteSpace(oLine.OutputUUID))
                    {
                        oTaskID = oLine.TaskId;
                        oTaskUID = oLine.OutputUUID;

                        if (oLines.Count > 1)
                        {
                            oOut.SplitIndicator = "true";
                        }
                    }


                    //LogisticsDeviationReasonCode =  new Saop.Byd.ManageSiteLogisticsTaskIn.LogisticsDeviationReasonCode() { Value = oLine.LogisticsDeviationReasonCode }

                    if (oLine.ConfirmQuantity < oLine.OpenQuantity &&
                        (string.IsNullOrWhiteSpace(oLine.LogisticsDeviationReasonCode) ||
                         oLine.LogisticsDeviationReasonCode == "1"))
                    {
                        oOut.SplitIndicator = "true";
                    }


                    // oOut.SerialNumber = new[] { "", "", "", "", "" },
                    oReqOuts.Add(oOut);
                }

                var oReq = new XmlSiteLogisticsTask()
                {
                    SiteLogisticTaskID = oTaskID,
                    SiteLogisticTaskUUID = oTaskUID,
                    ReferenceObject = new XmlReferenceObject()
                    {
                        ReferenceObjectUUID = oTaskUID,
                        OperationActivity = new XmlOperationActivity()
                        {
                            OperationActivityUUID = oTaskUID,
                            MaterialOutput = oReqOuts
                        }
                    }
                };
                oSiteLogisticsTasks.Add(oReq);
            }


            try
            {
                var xmlRequest = new XmlEnvelope()
                {
                    Body = new XmlBody()
                    {
                        SiteLogisticsTaskBundleMaintainRequest_sync_V1 =
                            new XmlSiteLogisticsTaskBundleMaintainRequest_sync_V1()
                            {
                                BasicMessageHeader = new XmlBasicMessageHeader()
                                    { ID = DateTime.Now.ToString("yyMMddhhmmssfffffff") },
                                SiteLogisticsTask = oSiteLogisticsTasks
                            }
                    }
                };


                string envlop =
                    @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:glob=""http://sap.com/xi/SAPGlobal20/Global""> 
                       <soapenv:Header/><soapenv:Body> {0} </soapenv:Body></soapenv:Envelope>";


                var client =
                    new RestClient($"https://{tenantId}/sap/bc/srt/scs/sap/managesitelogisticstaskin");
                client.Authenticator = new HttpBasicAuthenticator("_UNIVERSCAN", "Azur@232425");
                var getRequest = new RestRequest();
                getRequest.AddHeader("Content-Type", "text/xml");
                var oBody = xmlRequest.Body.SiteLogisticsTaskBundleMaintainRequest_sync_V1.ToXmlString();
                oBody = string.Format(envlop, oBody);
                oBody = oBody.Replace("SiteLogisticsTaskBundleMaintainRequest_sync_V1",
                    "glob:SiteLogisticsTaskBundleMaintainRequest_sync_V1");
                getRequest.AddBody(oBody, "text/xml");


                var oResponse = await client.ExecuteAsync(getRequest);
                if (oResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await Result<bool>.SuccessAsync(true);
                }
                else
                {
                    var responseXml = await oResponse.ResponseMessage.Content.ReadAsStringAsync();
                    if (responseXml.Contains("<standard>"))
                    {
                        var xdoc = XDocument.Parse(responseXml);

                        foreach (var e in xdoc.Descendants("faultDetail"))
                        {
                            if (e.Element("severity").Value == "E")
                            {
                                erros.Add($"{e.Element("id").Value}:{e.Element("text").Value}");
                            }

                            erros.Add(e.Value);
                        }


                        if (erros.IsNullOrEmpty() == false)
                        {
                            return await Result<bool>.FailAsync(erros);
                        }
                    }
                    else
                    {
                        return await Result<bool>.FailAsync("Application exception occurred!");
                    }
                }


                //    if (TaskResponse.SiteLogisticsTaskBundleMaintainResponse_sync_V1.SiteLogisticsTaskResponse != null)
                //    {
                //        var erros = new List<string>();
                //        foreach (var res in TaskResponse.SiteLogisticsTaskBundleMaintainResponse_sync_V1.SiteLogisticsTaskResponse)
                //        {
                //            foreach (var log in res.SiteLogisticsTaskLog)
                //            {
                //                if (log.SiteLogisticsTaskSeverityCode.StartsWith("E", StringComparison.InvariantCultureIgnoreCase))
                //                {
                //                    erros.Add($"{res.TaskID}:{log.SiteLogisticsTaskNodeName}:{log.SiteLogisticsTaskNote}");
                //                }

                //                //S	Success
                //                //E	Error
                //                //I	Information
                //                //W	Warning
                //                //A	Abort
                //            }
                //        }

                //        if (erros.IsNullOrEmpty() ==false)
                //        {
                //            return await Result<bool>.FailAsync(erros);
                //        }

                //    }
            }
            catch (Exception e)
            {
                return await Result<bool>.SuccessAsync(false, e.Message);
            }

            return await Result<bool>.SuccessAsync(true);
        }
    }
}