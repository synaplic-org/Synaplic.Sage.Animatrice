using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Uni.Scan.Shared.Configurations;
using RestSharp.Authenticators;
using Infrastructure.Bydesign.OData.UniScan;

namespace Uni.Scan.Infrastructure.ByDesign.Helper
{
    public static class BydServiceHelper
    {
        public static System.ServiceModel.BasicHttpBinding BasicBinding
        {
            get
            {
                // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var binding = new System.ServiceModel.BasicHttpBinding
                {
                    SendTimeout = TimeSpan.FromSeconds(900),
                    MaxBufferSize = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,
                    AllowCookies = true,
                    ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max
                };
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

                return binding;
            }
        }

        public static string GetOdataServiceAdresse(string tenantId, string odataservice)
        {
            return $"https://{tenantId}/sap/byd/odata/cust/v1/{odataservice}/";
        }

        internal static string GetSoapAdresse(string tenantId, string soapServiceName)
        {
            return $"https://{tenantId}/sap/bc/srt/scs/sap/{soapServiceName}?sap-vhost={tenantId}";
        }

        public static string GetAnalyticsAdresse(string tenantId)
        {
            return $"https://{tenantId}/sap/byd/odata/ana_businessanalytics_analytics.svc/";
        }

        internal static EndpointAddress ManageSiteLogisticsTaskInAdresse(string tenantId)
        {
            return new EndpointAddress(GetSoapAdresse(tenantId, "managesitelogisticstaskin"));
        }

        public static EndpointAddress InventoryProcessingGoodsAndAc2Adresse(string tenantId)
        {
            return new EndpointAddress(GetSoapAdresse(tenantId, "inventoryprocessinggoodsandac2"));
        }

        public static EndpointAddress QuerySiteLogisticsTaskInAdresse(string tenantId)
        {
            return new EndpointAddress(GetSoapAdresse(tenantId, "querysitelogisticstaskin"));
        }

        public static void BuildingOdataRequest(BuildingRequestEventArgs e, string tenantUser, string tenantPassword)
        {
            var obyteArray = Encoding.ASCII.GetBytes($"{tenantUser}:{tenantPassword}");
            string odataAuthorization = Convert.ToBase64String(obyteArray);
            e.Headers.Add("Authorization", "Basic " + odataAuthorization);
        }

        /// <summary>
        /// Get rest client with odata endpoint
        /// </summary>
        /// <param name="config"></param>
        /// <param name="oDataServiceName"></param>
        /// <returns></returns>
        public static RestClient GetOdataRestClient(BydesignConfiguration config, string oDataServiceName)
        {
            var oRestClient = new RestClient(GetOdataServiceAdresse(config.TenantId, oDataServiceName));
            oRestClient.Authenticator = new HttpBasicAuthenticator(config.OdataUser, config.OdataPassword);
            return oRestClient;
        }

        /// <summary>
        /// Get rest client with soap endpoint 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="oDataServiceName"></param>
        /// <returns></returns>
        public static RestClient GetSoapRestClient(BydesignConfiguration config, string oDataServiceName)
        {
            var oRestClient = new RestClient(GetSoapAdresse(config.TenantId, oDataServiceName));
            oRestClient.Authenticator = new HttpBasicAuthenticator(config.SoapUser, config.SoapPassword);
            return oRestClient;
        }

        /// <summary>
        /// Get rest client with odata endpoint
        /// </summary>
        /// <param name="config"></param>
        /// <param name="oDataServiceName"></param>
        /// <returns></returns>
        public static RestClient GetAnaliticsRestClient(BydesignConfiguration config, string oDataReportID)
        {
            var oRestClient = new RestClient(GetAnalyticsAdresse(config.TenantId));
            oRestClient.Authenticator = new HttpBasicAuthenticator(config.OdataUser, config.OdataPassword);
            return oRestClient;
        }

        public static uniscan GetUniScanOdataClient(BydesignConfiguration _config)
        {
            var oDataClient =
                new uniscan(new Uri(GetOdataServiceAdresse(_config.TenantId, _config.OdataServiceName)));
            oDataClient.BuildingRequest += (sender, args) =>
                BydServiceHelper.BuildingOdataRequest(args, _config.OdataUser, _config.OdataPassword);
            return oDataClient;
        }

        //public static uniscan_inventory GetInventoryOdataClient(BydesignConfiguration _config)
        //{
        //    var oDataClient =
        //        new uniscan_inventory(new Uri(GetOdataServiceAdresse(_config.TenantId, _config.InventoryServiceName)));
        //    oDataClient.BuildingRequest += (sender, args) =>
        //        BydServiceHelper.BuildingOdataRequest(args, _config.OdataUser, _config.OdataPassword);
        //    return oDataClient;
        //}
    }
}