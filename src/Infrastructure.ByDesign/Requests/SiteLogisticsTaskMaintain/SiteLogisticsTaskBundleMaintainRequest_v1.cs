using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Uni.Scan.Infrastructure.ByDesign.Requests.SiteLogisticsTaskMaintain
{
    public static class SiteLogisticsTaskBundleMaintainRequest_Extentions
    {
        public static string getXmlEnvelope(this XmlSiteLogisticsTaskBundleMaintainRequestSyncV1 request)
        {
            string xml =
                @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:glob=""http://sap.com/xi/SAPGlobal20/Global""> 
                       <soapenv:Header/><soapenv:Body> {0} </soapenv:Body></soapenv:Envelope>";


            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(request.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, request, emptyNamespaces);
                xml = string.Format(xml, stream.ToString());
            }

            xml = xml.Replace("SiteLogisticsTaskBundleMaintainRequest_sync_V1",
                "glob:SiteLogisticsTaskBundleMaintainRequest_sync_V1");


            return xml;
        }
    }
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Root));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Root)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "BasicMessageHeader")]
    public class XmlBasicMessageHeader
    {
        [XmlElement(ElementName = "ID")] public string ID { get; set; }

        [XmlElement(ElementName = "UUID")] public string UUID { get; set; }

        [XmlElement(ElementName = "ReferenceID")]
        public string ReferenceID { get; set; }

        [XmlElement(ElementName = "ReferenceUUID")]
        public string ReferenceUUID { get; set; }
    }

    [XmlRoot(ElementName = "StartDateTime")]
    public class XmlStartDateTime
    {
        [XmlAttribute(AttributeName = "timeZoneCode")]
        public string TimeZoneCode { get; set; }

        [XmlText] public string Text { get; set; }
    }

    [XmlRoot(ElementName = "EndDateTime")]
    public class XmlEndDateTime
    {
        [XmlAttribute(AttributeName = "timeZoneCode")]
        public string TimeZoneCode { get; set; }

        [XmlText] public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ActualDeliveryDate")]
    public class XmlActualDeliveryDate
    {
        [XmlElement(ElementName = "StartDateTime")]
        public XmlStartDateTime StartDateTime { get; set; }

        [XmlElement(ElementName = "EndDateTime")]
        public XmlEndDateTime EndDateTime { get; set; }
    }

    [XmlRoot(ElementName = "ActualQuantity")]
    public class XmlActualQuantity
    {
        [XmlAttribute(AttributeName = "unitCode")]
        public string UnitCode { get; set; }

        [XmlText] public decimal Text { get; set; }
    }

    [XmlRoot(ElementName = "LogisticsUnitQuantity")]
    public class XmlLogisticsUnitQuantity
    {
        [XmlAttribute(AttributeName = "unitCode")]
        public string UnitCode { get; set; }

        [XmlText] public string Text { get; set; }
    }

    [XmlRoot(ElementName = "LogisticsPackageInput")]
    public class XmlLogisticsPackageInput
    {
        [XmlElement(ElementName = "LogisticsUnitUUID")]
        public string LogisticsUnitUUID { get; set; }

        [XmlElement(ElementName = "LogisticsUnitID")]
        public string LogisticsUnitID { get; set; }

        [XmlElement(ElementName = "LogisticsUnitQuantity")]
        public XmlLogisticsUnitQuantity LogisticsUnitQuantity { get; set; }
    }

    [XmlRoot(ElementName = "AutomaticIdentification")]
    public class XmlAutomaticIdentification
    {
        [XmlElement(ElementName = "BarCodeText")]
        public List<string> BarCodeText { get; set; }
    }

    [XmlRoot(ElementName = "MaterialInput")]
    public class XmlMaterialInput
    {
        [XmlElement(ElementName = "MaterialInputUUID")]
        public string MaterialInputUUID { get; set; }

        [XmlElement(ElementName = "ProductID")]
        public string ProductID { get; set; }

        [XmlElement(ElementName = "SourceLogisticsAreaID")]
        public string SourceLogisticsAreaID { get; set; }

        [XmlElement(ElementName = "ActualQuantity")]
        public XmlActualQuantity ActualQuantity { get; set; }

        [XmlElement(ElementName = "IdentifiedStockID")]
        public string IdentifiedStockID { get; set; }

        [XmlElement(ElementName = "ProductRequirementSpecificationID")]
        public string ProductRequirementSpecificationID { get; set; }

        [XmlElement(ElementName = "InventoryRestrictedIndicator")]
        public string InventoryRestrictedIndicator { get; set; }

        [XmlElement(ElementName = "LogisticsDeviationReasonCode")]
        public string LogisticsDeviationReasonCode { get; set; }

        [XmlElement(ElementName = "SplitIndicator")]
        public string SplitIndicator { get; set; }

        [XmlElement(ElementName = "LogisticsPackageInput")]
        public XmlLogisticsPackageInput LogisticsPackageInput { get; set; }

        [XmlElement(ElementName = "AutomaticIdentification")]
        public XmlAutomaticIdentification AutomaticIdentification { get; set; }
    }

    [XmlRoot(ElementName = "LogisticsPackageOutput")]
    public class XmlLogisticsPackageOutput
    {
        [XmlElement(ElementName = "LogisticsUnitUUID")]
        public string LogisticsUnitUUID { get; set; }

        [XmlElement(ElementName = "LogisticsUnitID")]
        public string LogisticsUnitID { get; set; }

        [XmlElement(ElementName = "LogisticsUnitQuantity")]
        public XmlLogisticsUnitQuantity LogisticsUnitQuantity { get; set; }
    }

    [XmlRoot(ElementName = "SerialNumber")]
    public class XmlSerialNumber
    {
        [XmlElement(ElementName = "SerialID")] public List<string> SerialID { get; set; }
    }

    [XmlRoot(ElementName = "MaterialOutput")]
    public class XmlMaterialOutput
    {
        [XmlElement(ElementName = "LogisticsPackageOutput")]
        public List<XmlLogisticsPackageOutput> LogisticsPackageOutput { get; set; }

        [XmlElement(ElementName = "AutomaticIdentification")]
        public XmlAutomaticIdentification AutomaticIdentification { get; set; }

        [XmlElement(ElementName = "SerialNumber")]
        public XmlSerialNumber SerialNumber { get; set; }

        [XmlElement(ElementName = "ClearIdentifiedStockIDIndicator")]
        public string ClearIdentifiedStockIDIndicator { get; set; }

        [XmlElement(ElementName = "MaterialOutputUUID")]
        public string MaterialOutputUUID { get; set; }

        [XmlElement(ElementName = "ProductID")]
        public string ProductID { get; set; }

        [XmlElement(ElementName = "SourceLogisticsAreaIDPostSplit")]
        public string SourceLogisticsAreaIDPostSplit { get; set; }

        [XmlElement(ElementName = "TargetLogisticsAreaID")]
        public string TargetLogisticsAreaID { get; set; }

        [XmlElement(ElementName = "ActualQuantity")]
        public XmlActualQuantity ActualQuantity { get; set; }

        [XmlElement(ElementName = "IdentifiedStockID")]
        public string IdentifiedStockID { get; set; }

        [XmlElement(ElementName = "ProductRequirementSpecificationID")]
        public string ProductRequirementSpecificationID { get; set; }

        [XmlElement(ElementName = "InventoryRestrictedIndicator")]
        public string InventoryRestrictedIndicator { get; set; }

        [XmlElement(ElementName = "LogisticsDeviationReasonCode")]
        public string LogisticsDeviationReasonCode { get; set; }

        [XmlElement(ElementName = "SplitIndicator")]
        public string SplitIndicator { get; set; }
    }

    [XmlRoot(ElementName = "OperationActivity")]
    public class XmlOperationActivity
    {
        [XmlElement(ElementName = "OperationActivityUUID")]
        public string OperationActivityUUID { get; set; }

        [XmlElement(ElementName = "DeliverySplitIndicator")]
        public string DeliverySplitIndicator { get; set; }

        [XmlElement(ElementName = "MaterialInput")]
        public List<XmlMaterialInput> MaterialInput { get; set; }

        [XmlElement(ElementName = "MaterialOutput")]
        public List<XmlMaterialOutput> MaterialOutput { get; set; }
    }

    [XmlRoot(ElementName = "ReferenceObject")]
    public class XmlReferenceObject
    {
        [XmlElement(ElementName = "ReferenceObjectUUID")]
        public string ReferenceObjectUUID { get; set; }

        [XmlElement(ElementName = "OperationActivity")]
        public List<XmlOperationActivity> OperationActivity { get; set; }
    }

    [XmlRoot(ElementName = "SiteLogisticsTask")]
    public class XmlSiteLogisticsTask
    {
        [XmlElement(ElementName = "SiteLogisticTaskID")]
        public string SiteLogisticTaskID { get; set; }

        [XmlElement(ElementName = "SiteLogisticTaskUUID")]
        public string SiteLogisticTaskUUID { get; set; }

        [XmlElement(ElementName = "ActualExecutionOn")]
        public string ActualExecutionOn { get; set; }

        [XmlElement(ElementName = "PlannedStartDate")]
        public string PlannedStartDate { get; set; }

        [XmlElement(ElementName = "ActualDeliveryDate")]
        public XmlActualDeliveryDate ActualDeliveryDate { get; set; }

        [XmlElement(ElementName = "ReferenceObject")]
        public XmlReferenceObject ReferenceObject { get; set; }
    }

    [XmlRoot(ElementName = "SiteLogisticsTaskBundleMaintainRequest_sync_V1")]
    public class XmlSiteLogisticsTaskBundleMaintainRequestSyncV1
    {
        [XmlElement(ElementName = "BasicMessageHeader")]
        public XmlBasicMessageHeader BasicMessageHeader { get; set; }

        [XmlElement(ElementName = "SiteLogisticsTask")]
        public List<XmlSiteLogisticsTask> SiteLogisticsTask { get; set; }
    }
}