/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;

namespace Uni.Scan.Infrastructure.ByDesign.Logistics.Inbound
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {
        public static Envelope ReadFromXml(string xml)
        {
            var serializer = new XmlSerializer(typeof(Envelope));
            using (var reader = new StringReader(xml))
            {
                return (Envelope)serializer.Deserialize(reader);
            }
        }

        private object headerField;

        private EnvelopeBody bodyField;

        /// <remarks/>
        public object Header
        {
            get { return headerField; }
            set { headerField = value; }
        }

        /// <remarks/>
        public EnvelopeBody Body
        {
            get { return bodyField; }
            set { bodyField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {
        private EnvelopeBodyFault faultField;

        /// <remarks/>
        public EnvelopeBodyFault Fault
        {
            get { return faultField; }
            set { faultField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyFault
    {
        private string faultcodeField;

        private faultstring faultstringField;

        private detail detailField;

        /// <remarks/>
        [XmlElement(Namespace = "")]
        public string faultcode
        {
            get { return faultcodeField; }
            set { faultcodeField = value; }
        }

        /// <remarks/>
        [XmlElement(Namespace = "")]
        public faultstring faultstring
        {
            get { return faultstringField; }
            set { faultstringField = value; }
        }

        /// <remarks/>
        [XmlElement(Namespace = "")]
        public detail detail
        {
            get { return detailField; }
            set { detailField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class faultstring
    {
        private string langField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get { return langField; }
            set { langField = value; }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class detail
    {
        private StandardFaultMessage standardFaultMessageField;

        /// <remarks/>
        [XmlElement(Namespace = "http://sap.com/xi/AP/Common/Global")]
        public StandardFaultMessage StandardFaultMessage
        {
            get { return standardFaultMessageField; }
            set { standardFaultMessageField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://sap.com/xi/AP/Common/Global")]
    [XmlRoot(Namespace = "http://sap.com/xi/AP/Common/Global", IsNullable = false)]
    public partial class StandardFaultMessage
    {
        private standard standardField;

        private object additionField;

        /// <remarks/>
        [XmlElement(Namespace = "")]
        public standard standard
        {
            get { return standardField; }
            set { standardField = value; }
        }

        /// <remarks/>
        [XmlElement(Namespace = "")]
        public object addition
        {
            get { return additionField; }
            set { additionField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class standard
    {
        private string faultTextField;

        private string faultUrlField;

        private standardFaultDetail[] faultDetailField;

        /// <remarks/>
        public string faultText
        {
            get { return faultTextField; }
            set { faultTextField = value; }
        }

        /// <remarks/>
        public string faultUrl
        {
            get { return faultUrlField; }
            set { faultUrlField = value; }
        }

        /// <remarks/>
        [XmlElement("faultDetail")]
        public standardFaultDetail[] faultDetail
        {
            get { return faultDetailField; }
            set { faultDetailField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class standardFaultDetail
    {
        private string severityField;

        private string textField;

        private string idField;

        /// <remarks/>
        public string severity
        {
            get { return severityField; }
            set { severityField = value; }
        }

        /// <remarks/>
        public string text
        {
            get { return textField; }
            set { textField = value; }
        }

        /// <remarks/>
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }
    }
}