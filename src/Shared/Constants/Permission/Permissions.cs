using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Uni.Scan.Shared.Constants.Permission
{
    public static class Permissions
    {
        public const string ClaimType = "Permission";

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Export = "Permissions.Users.Export";
            public const string Search = "Permissions.Users.Search";
        }

        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
            public const string Search = "Permissions.Roles.Search";
        }

        public static class RoleClaims
        {
            public const string View = "Permissions.RoleClaims.View";
            public const string Create = "Permissions.RoleClaims.Create";
            public const string Edit = "Permissions.RoleClaims.Edit";
            public const string Delete = "Permissions.RoleClaims.Delete";
            public const string Search = "Permissions.RoleClaims.Search";
        }

        public static class Logistics
        {
            public const string Home = "Permissions.Logistics.Home";
            public const string Inbound = "Permissions.Logistics.Inbound";
            public const string Outbound = "Permissions.Logistics.Outbound";
            public const string Production = "Permissions.Logistics.Production";
            public const string Internal = "Permissions.Logistics.Internal";
            public const string Inventory = "Permissions.Logistics.Inventory";
        }
        public static class Parametres
        {
            public const string View = "Permissions.Parametres.View";
            public const string Create = "Permissions.Parametres.Create";
            public const string Edit = "Permissions.Parametres.Edit";
            public const string Delete = "Permissions.Parametres.Delete";
            public const string Search = "Permissions.Parametres.Search";
        }
		public static class LogisticArea
		{
			public const string View = "Permissions.LogisticArea.View";
			public const string Search = "Permissions.LogisticArea.Search";
            public const string Print = "Permissions.LogisticArea.Print";

        }
        public static class Labels
        {
            public const string View = "Permissions.Labels.View";
            public const string Create = "Permissions.Labels.Create";
            public const string Edit = "Permissions.Labels.Edit";
            public const string Delete = "Permissions.Labels.Delete";
            public const string Search = "Permissions.Labels.Search";
            public const string Print = "Permissions.Labels.Print";


        }
        public static class StockAnomalies
        {
            public const string View = "Permissions.StockAnomalies.View";
            public const string Validate = "Permissions.StockAnomalies.Validate";
            public const string Reject = "Permissions.StockAnomalies.Reject";
            public const string Cancel = "Permissions.StockAnomalies.Cancel";
            public const string Create = "Permissions.StockAnomalies.Create";
            public const string Edit = "Permissions.StockAnomalies.Edit";
            public const string Delete = "Permissions.StockAnomalies.Delete";
            public const string Search = "Permissions.StockAnomalies.Search";
        }
        public static class Preferences
        {
            public const string ChangeLanguage = "Permissions.Preferences.ChangeLanguage";
            //TODO - add permissions
        }


        public static class Infrastructure
        {
            public const string Jobs = "Permissions.Infrastructure.Jobs";
            public const string Swagger = "Permissions.Infrastructure.Swagger";
        }

        public static class AuditTrails
        {
            public const string View = "Permissions.AuditTrails.View";
            public const string Export = "Permissions.AuditTrails.Export";
            public const string Search = "Permissions.AuditTrails.Search";
        }
        /// <summary>
        /// Returns a list of Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRegisteredPermissions()
        {
            var permssions = new List<string>();
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                    permssions.Add(propertyValue.ToString());
            }
            return permssions;
        }
    }
}