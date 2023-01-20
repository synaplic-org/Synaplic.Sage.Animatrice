using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Uni.Scan.Infrastructure.ByDesign.Requests.SiteLogisticsTaskMaintain
{
    //https://json2csharp.com/code-converters/xml-to-csharp

    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Envelope)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "faultstring")]
    public class XmlResponseFaultstring
    {
        [XmlAttribute(AttributeName = "lang")] public string Lang { get; set; }

        [XmlText] public string Text { get; set; }
    }

    [XmlRoot(ElementName = "faultDetail")]
    public class XmlResponseFaultDetail
    {
        [XmlElement(ElementName = "severity")] public string Severity { get; set; }

        [XmlElement(ElementName = "text")] public string Text { get; set; }

        [XmlElement(ElementName = "id")] public string Id { get; set; }
    }

    [XmlRoot(ElementName = "standard")]
    public class XmlResponseStandard
    {
        [XmlElement(ElementName = "faultText")]
        public string FaultText { get; set; }

        [XmlElement(ElementName = "faultUrl")] public string FaultUrl { get; set; }

        [XmlElement(ElementName = "faultDetail")]
        public List<XmlResponseFaultDetail> FaultDetail { get; set; }
    }

    [XmlRoot(ElementName = "StandardFaultMessage")]
    public class XmlResponseStandardFaultMessage
    {
        [XmlElement(ElementName = "standard")] public XmlResponseStandard Standard { get; set; }

        [XmlElement(ElementName = "addition")] public object Addition { get; set; }

        [XmlAttribute(AttributeName = "n0")] public string N0 { get; set; }

        [XmlAttribute(AttributeName = "prx")] public string Prx { get; set; }

        [XmlText] public string Text { get; set; }
    }

    [XmlRoot(ElementName = "detail")]
    public class XmlResponseDetail
    {
        [XmlElement(ElementName = "StandardFaultMessage")]
        public XmlResponseStandardFaultMessage StandardFaultMessage { get; set; }
    }

    [XmlRoot(ElementName = "Fault")]
    public class XmlResponseFault
    {
        [XmlElement(ElementName = "faultcode")]
        public string Faultcode { get; set; }

        [XmlElement(ElementName = "faultstring")]
        public XmlResponseFaultstring Faultstring { get; set; }

        [XmlElement(ElementName = "detail")] public XmlResponseDetail Detail { get; set; }
    }

    [XmlRoot(ElementName = "Body")]
    public class XmlResponseBody
    {
        [XmlElement(ElementName = "Fault")] public XmlResponseFault Fault { get; set; }
    }

    [XmlRoot(ElementName = "Envelope")]
    public class XmlResponseEnvelope
    {
        [XmlElement(ElementName = "Header")] public object Header { get; set; }

        [XmlElement(ElementName = "Body")] public XmlResponseBody Body { get; set; }

        [XmlAttribute(AttributeName = "soap-env")]
        public string SoapEnv { get; set; }

        [XmlText] public string Text { get; set; }

        public static XmlResponseEnvelope GetFromXml(string xml)
        {
            XmlResponseEnvelope result;
            XmlSerializer serializer = new XmlSerializer(typeof(XmlResponseEnvelope));
            using StringReader reader = new StringReader(xml);
            result = (XmlResponseEnvelope)serializer.Deserialize(reader);
            return result;
        }
    }
}