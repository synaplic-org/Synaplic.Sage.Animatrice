using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Uni.Scan.Infrastructure.ByDesign.Logistics.Inbound
{
    [XmlRoot(ElementName = "BasicMessageHeader")]
    public class XmlBasicMessageHeader
    {
        [XmlElement(ElementName = "ID")] public string ID { get; set; }
    }

    [XmlRoot(ElementName = "ActualQuantity")]
    public class XmlActualQuantity
    {
        [XmlAttribute(AttributeName = "unitCode")]
        public string UnitCode { get; set; }

        [XmlText] public decimal Value { get; set; }
    }

    [XmlRoot(ElementName = "SerialNumber")]
    public class XmlSerialNumber
    {
        [XmlElement(ElementName = "SerialID")] public List<string> SerialID { get; set; }
    }

    [XmlRoot(ElementName = "MaterialOutput")]
    public class XmlMaterialOutput
    {
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

        [XmlElement(ElementName = "ClearIdentifiedStockIDIndicator")]
        public bool ClearIdentifiedStockIDIndicator { get; set; }

        [XmlElement(ElementName = "SplitIndicator")]
        public string SplitIndicator { get; set; }

        [XmlElement(ElementName = "SerialNumber")]
        public XmlSerialNumber SerialNumber { get; set; }
    }

    [XmlRoot(ElementName = "OperationActivity")]
    public class XmlOperationActivity
    {
        [XmlElement(ElementName = "OperationActivityUUID")]
        public string OperationActivityUUID { get; set; }

        [XmlElement(ElementName = "MaterialOutput")]
        public List<XmlMaterialOutput> MaterialOutput { get; set; }
    }

    [XmlRoot(ElementName = "ReferenceObject")]
    public class XmlReferenceObject
    {
        [XmlElement(ElementName = "ReferenceObjectUUID")]
        public string ReferenceObjectUUID { get; set; }

        [XmlElement(ElementName = "OperationActivity")]
        public XmlOperationActivity OperationActivity { get; set; }
    }

    [XmlRoot(ElementName = "SiteLogisticsTask")]
    public class XmlSiteLogisticsTask
    {
        [XmlElement(ElementName = "SiteLogisticTaskID")]
        public string SiteLogisticTaskID { get; set; }

        [XmlElement(ElementName = "SiteLogisticTaskUUID")]
        public string SiteLogisticTaskUUID { get; set; }

        [XmlElement(ElementName = "ReferenceObject")]
        public XmlReferenceObject ReferenceObject { get; set; }
    }

    [XmlRoot(ElementName = "SiteLogisticsTaskBundleMaintainRequest_sync_V1")]
    public class XmlSiteLogisticsTaskBundleMaintainRequest_sync_V1
    {
        [XmlElement(ElementName = "BasicMessageHeader")]
        public XmlBasicMessageHeader BasicMessageHeader { get; set; }

        [XmlElement(ElementName = "SiteLogisticsTask")]
        public List<XmlSiteLogisticsTask> SiteLogisticsTask { get; set; }

        public string ToXmlString()
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(this.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, this, emptyNamespaces);
                return stream.ToString();
            }
        }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class XmlBody
    {
        [XmlElement(ElementName = "SiteLogisticsTaskBundleMaintainRequest_sync_V1",
            Namespace = "http://sap.com/xi/SAPGlobal20/Global")]
        public XmlSiteLogisticsTaskBundleMaintainRequest_sync_V1 SiteLogisticsTaskBundleMaintainRequest_sync_V1
        {
            get;
            set;
        }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class XmlEnvelope
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public XmlBody Body { get; set; }

        [XmlAttribute(AttributeName = "soapenv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Soapenv { get; set; }

        [XmlAttribute(AttributeName = "glob", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Glob { get; set; }


        public string ToXmlString()
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stringwriter, this);
                return stringwriter.ToString();
            }
        }
    }
}


//namespace testahmed
//{

//    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
//    public partial class Envelope
//    {

//        private object headerField;

//        private EnvelopeBody bodyField;

//        /// <remarks/>
//        public object Header
//        {
//            get
//            {
//                return this.headerField;
//            }
//            set
//            {
//                this.headerField = value;
//            }
//        }

//        /// <remarks/>
//        public EnvelopeBody Body
//        {
//            get
//            {
//                return this.bodyField;
//            }
//            set
//            {
//                this.bodyField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
//    public partial class EnvelopeBody
//    {

//        private SiteLogisticsTaskBundleMaintainRequest_sync_V1 siteLogisticsTaskBundleMaintainRequest_sync_V1Field;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://sap.com/xi/SAPGlobal20/Global")]
//        public SiteLogisticsTaskBundleMaintainRequest_sync_V1 SiteLogisticsTaskBundleMaintainRequest_sync_V1
//        {
//            get
//            {
//                return this.siteLogisticsTaskBundleMaintainRequest_sync_V1Field;
//            }
//            set
//            {
//                this.siteLogisticsTaskBundleMaintainRequest_sync_V1Field = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://sap.com/xi/SAPGlobal20/Global")]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://sap.com/xi/SAPGlobal20/Global", IsNullable = false)]
//    public partial class SiteLogisticsTaskBundleMaintainRequest_sync_V1
//    {

//        private BasicMessageHeader basicMessageHeaderField;

//        private SiteLogisticsTask[] siteLogisticsTaskField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
//        public BasicMessageHeader BasicMessageHeader
//        {
//            get
//            {
//                return this.basicMessageHeaderField;
//            }
//            set
//            {
//                this.basicMessageHeaderField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("SiteLogisticsTask", Namespace = "")]
//        public SiteLogisticsTask[] SiteLogisticsTask
//        {
//            get
//            {
//                return this.siteLogisticsTaskField;
//            }
//            set
//            {
//                this.siteLogisticsTaskField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
//    public partial class BasicMessageHeader
//    {

//        private string idField;

//        /// <remarks/>
//        public string ID
//        {
//            get
//            {
//                return this.idField;
//            }
//            set
//            {
//                this.idField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
//    public partial class SiteLogisticsTask
//    {

//        private string siteLogisticTaskIDField;

//        private string siteLogisticTaskUUIDField;

//        private SiteLogisticsTaskReferenceObject referenceObjectField;

//        /// <remarks/>
//        public string SiteLogisticTaskID
//        {
//            get
//            {
//                return this.siteLogisticTaskIDField;
//            }
//            set
//            {
//                this.siteLogisticTaskIDField = value;
//            }
//        }

//        /// <remarks/>
//        public string SiteLogisticTaskUUID
//        {
//            get
//            {
//                return this.siteLogisticTaskUUIDField;
//            }
//            set
//            {
//                this.siteLogisticTaskUUIDField = value;
//            }
//        }

//        /// <remarks/>
//        public SiteLogisticsTaskReferenceObject ReferenceObject
//        {
//            get
//            {
//                return this.referenceObjectField;
//            }
//            set
//            {
//                this.referenceObjectField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class SiteLogisticsTaskReferenceObject
//    {

//        private string referenceObjectUUIDField;

//        private SiteLogisticsTaskReferenceObjectOperationActivity operationActivityField;

//        /// <remarks/>
//        public string ReferenceObjectUUID
//        {
//            get
//            {
//                return this.referenceObjectUUIDField;
//            }
//            set
//            {
//                this.referenceObjectUUIDField = value;
//            }
//        }

//        /// <remarks/>
//        public SiteLogisticsTaskReferenceObjectOperationActivity OperationActivity
//        {
//            get
//            {
//                return this.operationActivityField;
//            }
//            set
//            {
//                this.operationActivityField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class SiteLogisticsTaskReferenceObjectOperationActivity
//    {

//        private string operationActivityUUIDField;

//        private SiteLogisticsTaskReferenceObjectOperationActivityMaterialOutput[] materialOutputField;

//        /// <remarks/>
//        public string OperationActivityUUID
//        {
//            get
//            {
//                return this.operationActivityUUIDField;
//            }
//            set
//            {
//                this.operationActivityUUIDField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("MaterialOutput")]
//        public SiteLogisticsTaskReferenceObjectOperationActivityMaterialOutput[] MaterialOutput
//        {
//            get
//            {
//                return this.materialOutputField;
//            }
//            set
//            {
//                this.materialOutputField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class SiteLogisticsTaskReferenceObjectOperationActivityMaterialOutput
//    {

//        private string materialOutputUUIDField;

//        private string productIDField;

//        private string sourceLogisticsAreaIDPostSplitField;

//        private string targetLogisticsAreaIDField;

//        private SiteLogisticsTaskReferenceObjectOperationActivityMaterialOutputActualQuantity actualQuantityField;

//        private string identifiedStockIDField;

//        private string productRequirementSpecificationIDField;

//        private string inventoryRestrictedIndicatorField;

//        private string logisticsDeviationReasonCodeField;

//        private string splitIndicatorField;

//        private string[] serialNumberField;

//        /// <remarks/>
//        public string MaterialOutputUUID
//        {
//            get
//            {
//                return this.materialOutputUUIDField;
//            }
//            set
//            {
//                this.materialOutputUUIDField = value;
//            }
//        }

//        /// <remarks/>
//        public string ProductID
//        {
//            get
//            {
//                return this.productIDField;
//            }
//            set
//            {
//                this.productIDField = value;
//            }
//        }

//        /// <remarks/>
//        public string SourceLogisticsAreaIDPostSplit
//        {
//            get
//            {
//                return this.sourceLogisticsAreaIDPostSplitField;
//            }
//            set
//            {
//                this.sourceLogisticsAreaIDPostSplitField = value;
//            }
//        }

//        /// <remarks/>
//        public string TargetLogisticsAreaID
//        {
//            get
//            {
//                return this.targetLogisticsAreaIDField;
//            }
//            set
//            {
//                this.targetLogisticsAreaIDField = value;
//            }
//        }

//        /// <remarks/>
//        public SiteLogisticsTaskReferenceObjectOperationActivityMaterialOutputActualQuantity ActualQuantity
//        {
//            get
//            {
//                return this.actualQuantityField;
//            }
//            set
//            {
//                this.actualQuantityField = value;
//            }
//        }

//        /// <remarks/>
//        public string IdentifiedStockID
//        {
//            get
//            {
//                return this.identifiedStockIDField;
//            }
//            set
//            {
//                this.identifiedStockIDField = value;
//            }
//        }

//        /// <remarks/>
//        public string ProductRequirementSpecificationID
//        {
//            get
//            {
//                return this.productRequirementSpecificationIDField;
//            }
//            set
//            {
//                this.productRequirementSpecificationIDField = value;
//            }
//        }

//        /// <remarks/>
//        public string InventoryRestrictedIndicator
//        {
//            get
//            {
//                return this.inventoryRestrictedIndicatorField;
//            }
//            set
//            {
//                this.inventoryRestrictedIndicatorField = value;
//            }
//        }

//        /// <remarks/>
//        public string LogisticsDeviationReasonCode
//        {
//            get
//            {
//                return this.logisticsDeviationReasonCodeField;
//            }
//            set
//            {
//                this.logisticsDeviationReasonCodeField = value;
//            }
//        }

//        /// <remarks/>
//        public string SplitIndicator
//        {
//            get
//            {
//                return this.splitIndicatorField;
//            }
//            set
//            {
//                this.splitIndicatorField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlArrayItemAttribute("SerialID", IsNullable = false)]
//        public string[] SerialNumber
//        {
//            get
//            {
//                return this.serialNumberField;
//            }
//            set
//            {
//                this.serialNumberField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.SerializableAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class SiteLogisticsTaskReferenceObjectOperationActivityMaterialOutputActualQuantity
//    {

//        private string unitCodeField;

//        private string valueField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string unitCode
//        {
//            get
//            {
//                return this.unitCodeField;
//            }
//            set
//            {
//                this.unitCodeField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlTextAttribute()]
//        public string Value
//        {
//            get
//            {
//                return this.valueField;
//            }
//            set
//            {
//                this.valueField = value;
//            }
//        }
//    }


//}