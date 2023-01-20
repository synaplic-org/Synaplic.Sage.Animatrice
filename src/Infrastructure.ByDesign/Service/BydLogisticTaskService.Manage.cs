using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Uni.Scan.Infrastructure.ByDesign.Helper;
using Uni.Scan.Infrastructure.ByDesign.Requests.SiteLogisticsTaskMaintain;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel.LogisticTask;

namespace Uni.Scan.Infrastructure.ByDesign.Service;

public partial class BydLogisticTaskService
{
    public async Task<IResult<LogisticTaskDTO2>> PostTaskAsync(LogisticTaskDTO2 dto)
    {
        //"1"  => "Make",
        //"10" => "Supply",

        //"11" => "Put Away",
        //"12" => "Unload",
        //"13" => "Returns Put Away",
        //"14" => "Returns Unload",
        //"21" => "Pick",
        //"22" => "Load",
        //"23" => "Returns Pick",
        //"24" => "Returns Load",
        //"30" => "Replenish",
        //"31" => "Remove",

        //"8"  => "Check",


        List<string> errors = new();
        List<XmlOperationActivity> OperationActivityListe = new List<XmlOperationActivity>();

        var pickCodes = new[] { "21", "23", "30" };

        if (pickCodes.Contains(dto.OperationTypeCode))
        {
            OperationActivityListe = GetInputActivities(dto);
        }
        else
        {
            OperationActivityListe = GetOutPutActivities(dto);
        }

        if (OperationActivityListe.IsNullOrEmpty()) errors.Add(_localizer["Liste d'envoi vide !"]);


        if (!errors.IsNullOrEmpty()) return await Result<LogisticTaskDTO2>.FailAsync(errors, dto);


        var request = new XmlSiteLogisticsTaskBundleMaintainRequestSyncV1()
        {
            BasicMessageHeader = new XmlBasicMessageHeader()
            {
                ID = $"UNI_{dto.Id}"
            },
            SiteLogisticsTask = new List<XmlSiteLogisticsTask>()
            {
                new XmlSiteLogisticsTask()
                {
                    SiteLogisticTaskID = dto.Id,
                    SiteLogisticTaskUUID = dto.TaskUuid,
                    ActualDeliveryDate = dto.ProcessTypeCode == "1"
                        ? new XmlActualDeliveryDate()
                        {
                            StartDateTime = new XmlStartDateTime() { Text = DateTime.Now.ToString("o"), TimeZoneCode = "MOROCC" },
                            EndDateTime = new XmlEndDateTime() { Text = DateTime.Now.ToString("o"), TimeZoneCode = "MOROCC" }
                        }
                        : null,
                    ReferenceObject = new XmlReferenceObject()
                    {
                        ReferenceObjectUUID = dto.ReferencedObjectUUID,
                        OperationActivity = OperationActivityListe
                    }
                }
            }
        };


        try
        {
            var xml = request.getXmlEnvelope();
            //return await Result<LogisticTaskDTO2>.FailAsync(new List<string>() { xml }, dto);


            var serors = await SendXmlEnvelopeAsync(xml);
            // item.Errors = serors;
            if (!serors.IsNullOrEmpty()) errors.AddRange(serors);
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
        }

        if (errors.IsNullOrEmpty()) return await Result<LogisticTaskDTO2>.SuccessAsync(dto);

        return await Result<LogisticTaskDTO2>.FailAsync(errors, dto);
    }

    private List<XmlOperationActivity> GetOutPutActivities(LogisticTaskDTO2 dto)
    {
        var oActivityListe = new List<XmlOperationActivity>();
        foreach (var item in dto.Items)
        {
            item.Errors = null;
            // check idetifies stock existe 

            // l'element parent un seul 
            var baseDteails = item.Details.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.OutputUUID)
                                                               && o.OpenQuantity > 0 && o.ConfirmQuantity > 0);
            if (baseDteails == null) continue;
            List<XmlMaterialInput> oXmlInputs = new List<XmlMaterialInput>();
            List<XmlMaterialOutput> oXmloutPuts = new List<XmlMaterialOutput>();

            // les elements ajoutés
            var newDetails = item.Details.Where(o => o.IsNew && o.ConfirmQuantity > 0).ToList();

            var split = !newDetails.IsNullOrEmpty() || (baseDteails.ConfirmQuantity < baseDteails.OpenQuantity);

            var xmlBaseOutPut = new XmlMaterialOutput
            {
                MaterialOutputUUID = baseDteails.OutputUUID,
                ProductID = baseDteails.ProductID,
                TargetLogisticsAreaID = baseDteails.TargetLogisticsAreaID,
                ActualQuantity = new XmlActualQuantity()
                    { Text = baseDteails.ConfirmQuantity, UnitCode = baseDteails.ConfirmQuantityUnitCode },
                SplitIndicator = split ? "true" : null
            };
            if (baseDteails.IdentifiedStockIDNew != baseDteails.IdentifiedStockID)
            {
                xmlBaseOutPut.IdentifiedStockID = baseDteails.IdentifiedStockIDNew;
                xmlBaseOutPut.ClearIdentifiedStockIDIndicator = "true";
            }


            oXmloutPuts.Add(xmlBaseOutPut);

            foreach (var oNewDetail in newDetails)
            {
                var newXmlBaseOutPut = new XmlMaterialOutput
                {
                    ProductID = baseDteails.ProductID,
                    TargetLogisticsAreaID = oNewDetail.TargetLogisticsAreaID,
                    IdentifiedStockID = oNewDetail.IdentifiedStockIDNew,
                    ActualQuantity = new XmlActualQuantity()
                        { Text = oNewDetail.ConfirmQuantity, UnitCode = oNewDetail.ConfirmQuantityUnitCode },
                    SplitIndicator = split ? "true" : null
                };

                oXmloutPuts.Add(newXmlBaseOutPut);
            }

            if (item.DeviationFound)
            {
                if (!string.IsNullOrWhiteSpace(_config.LogisticsDeviationReasonCode))
                {
                    oXmloutPuts.Last().LogisticsDeviationReasonCode = _config.LogisticsDeviationReasonCode;
                    oXmloutPuts.Last().SplitIndicator = null;
                }
            }
            else if (item.SumConfirmQuantity >= item.SumOpenQuantity)
            {
                oXmloutPuts.Last().SplitIndicator = null;
            }


            if (oXmloutPuts.IsNullOrEmpty()) continue;


            oActivityListe.Add(new XmlOperationActivity()
            {
                OperationActivityUUID = dto.OperationActivityUUID,
                MaterialInput = oXmlInputs,
                MaterialOutput = oXmloutPuts
            });
        }

        return oActivityListe;
    }

    private List<XmlOperationActivity> GetInputActivities(LogisticTaskDTO2 dto)
    {
        var oActivityListe = new List<XmlOperationActivity>();
        foreach (var item in dto.Items)
        {
            item.Errors = null;
            // check idetifies stock existe 

            // l'element parent un seul 
            var baseDteails = item.Details.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.OutputUUID)
                                                               && o.OpenQuantity > 0 && o.ConfirmQuantity > 0);
            if (baseDteails == null) continue;
            List<XmlMaterialInput> oXmlInputs = new List<XmlMaterialInput>();
            List<XmlMaterialOutput> oXmloutPuts = new List<XmlMaterialOutput>();

            // les elements ajoutés
            var newDetails = item.Details.Where(o => o.IsNew && o.ConfirmQuantity > 0).ToList();

            var split = !newDetails.IsNullOrEmpty() || (baseDteails.ConfirmQuantity < baseDteails.OpenQuantity);

            var xmlBaseOutPut = new XmlMaterialOutput
            {
                MaterialOutputUUID = baseDteails.OutputUUID,
                ProductID = baseDteails.ProductID,
                SourceLogisticsAreaIDPostSplit = baseDteails.SourceLogisticsAreaID,
                TargetLogisticsAreaID = baseDteails.TargetLogisticsAreaID,
                ActualQuantity = new XmlActualQuantity()
                    { Text = baseDteails.ConfirmQuantity, UnitCode = baseDteails.ConfirmQuantityUnitCode },
                SplitIndicator = split ? "true" : null
            };
            if (baseDteails.IdentifiedStockIDNew != baseDteails.IdentifiedStockID)
            {
                xmlBaseOutPut.IdentifiedStockID = baseDteails.IdentifiedStockIDNew;
                xmlBaseOutPut.ClearIdentifiedStockIDIndicator = "true";
            }

            oXmloutPuts.Add(xmlBaseOutPut);

            var xmlBaseInPut = new XmlMaterialInput()
            {
                MaterialInputUUID = baseDteails.InputUUID,
                SourceLogisticsAreaID = baseDteails.SourceLogisticsAreaID
            };

            oXmlInputs.Add(xmlBaseInPut);

            foreach (var oNewDetail in newDetails)
            {
                var newXmlOutPut = new XmlMaterialOutput
                {
                    ProductID = baseDteails.ProductID,
                    SourceLogisticsAreaIDPostSplit = oNewDetail.SourceLogisticsAreaID,
                    TargetLogisticsAreaID = oNewDetail.TargetLogisticsAreaID,
                    IdentifiedStockID = oNewDetail.IdentifiedStockIDNew,
                    ActualQuantity = new XmlActualQuantity()
                        { Text = oNewDetail.ConfirmQuantity, UnitCode = oNewDetail.ConfirmQuantityUnitCode },
                    SplitIndicator = split ? "true" : null
                };

                oXmloutPuts.Add(newXmlOutPut);
            }

            if (item.DeviationFound)
            {
                if (!string.IsNullOrWhiteSpace(_config.LogisticsDeviationReasonCode))
                {
                    oXmloutPuts.Last().LogisticsDeviationReasonCode = _config.LogisticsDeviationReasonCode;
                    oXmloutPuts.Last().SplitIndicator = null;
                }
            }
            else if (item.SumConfirmQuantity >= item.SumOpenQuantity)
            {
                oXmloutPuts.Last().SplitIndicator = null;
                oXmlInputs.Last().SplitIndicator = null;
            }


            if (oXmloutPuts.IsNullOrEmpty()) continue;


            oActivityListe.Add(new XmlOperationActivity()
            {
                OperationActivityUUID = dto.OperationActivityUUID,
                MaterialInput = oXmlInputs,
                MaterialOutput = oXmloutPuts
            });
        }

        return oActivityListe;
    }


    private async Task<List<string>> SendXmlEnvelopeAsync(string xmlRequest)
    {
        var erros = new List<string>();

        var oRestClient = BydServiceHelper.GetSoapRestClient(_config, "managesitelogisticstaskin");


        var getRequest = new RestRequest();

        getRequest.AddHeader("Content-Type", "text/xml");
        getRequest.AddBody(xmlRequest, "text/xml");


        var oResponse = await oRestClient.ExecuteAsync(getRequest);

        if (oResponse.StatusCode != System.Net.HttpStatusCode.OK)
        {
            var responseXml = await oResponse.ResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(responseXml);
            if (responseXml.Contains("<standard>"))
            {
                var xdoc = XDocument.Parse(responseXml);
                var faults = xdoc.Root.DescendantsAndSelf("faultDetail");
                foreach (var e in faults)
                {
                    if (e.Element("severity")?.Value == "E" && e.Element("text") != null)
                    {
                        erros.Add("E:" + e.Element("text")?.Value);
                        erros.Add("E:" + e.Parent.Element("faultText").Value);
                    }
                }

                if (erros.IsNullOrEmpty() == false)
                {
                    return erros.Distinct().ToList();
                }
            }
            else
            {
                return new List<string>() { oResponse.ErrorMessage };
            }
        }

        return new List<string>();
    }
}