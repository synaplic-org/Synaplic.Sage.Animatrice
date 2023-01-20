namespace Uni.Scan.Shared.Constants.Application
{
    public static class ApplicationConstants
    {
        public static class SignalR
        {
            public const string HubUrl = "/signalRHub";
            public const string SendUpdateDashboard = "UpdateDashboardAsync";
            public const string ReceiveUpdateDashboard = "UpdateDashboard";
            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";


            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUser = "ConnectUser";
            public const string OnDisconnect = "OnDisconnectAsync";
            public const string DisconnectUser = "DisconnectUser";
            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRole = "LogoutUsersByRole";
        }

        public static class Cache
        {
            public const string GetAllDocumentTypesCacheKey = "all-document-types";
            public const string GetAllTaskScanssCacheKey = "all-task-scans";
            public const string GetAllLabelsCachekey = "all-labels-types";
            public const string GetAllParametresCachekey = "all-parametres-types";
            public const string GetAllStockAnomalyCacheKey = "all-stock-anomalies";
            public const string GetAllScanningCodeCacheKey = "all-scanning-codes";


        }

        public static class MimeTypes
        {
            public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            public const string Text = "application/text";
        }
    }
}