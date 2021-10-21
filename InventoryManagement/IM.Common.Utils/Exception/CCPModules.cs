namespace IM.Common.Utils.Exception
{
    public class CCPModules
    {
        public static readonly string SiteCommandManagement = "10";
        public static readonly string SiteQueryManagement = "11";
        public static readonly string BuildingCommandManagement = "12";
        public static readonly string BuildingQueryManagement = "13";
        public static readonly string DeviceTenantCommandManagement = "14";
        public static readonly string DeviceTenantQueryManagement = "15";
        public static readonly string UserAccessCommandManagement = "16";
        public static readonly string DeviceUserCommand = "17";
        public static readonly string DeviceUserQuery = "18";
        public static readonly string TenantCommand = "19";
        public static readonly string TenantQuery = "20";
        public static readonly string DeviceSiteCommand = "21";
        public static readonly string DeviceSiteQuery = "22";
        public static readonly string PolicyCommand = "23";
        public static readonly string PolicyQuery = "24";
        public static readonly string RolesCommand = "25";
        public static readonly string RolesQuery = "26";
        public static readonly string UserAccessQuery = "27";
        public static readonly string PolicyRoleAssociationCommand = "28";
        public static readonly string DataPolicyCommand = "29";
        public static readonly string DataPolicyQuery = "30";
        public static readonly string TenantDataPolicyQuery = "31";
        public static readonly string TenantDataPolicyCommand = "32";
        public static readonly string UserCommandManagement = "33";
        public static readonly string UserQueryManagement = "34";
        public static readonly string IdentityManagement = "35";
        public static readonly string PolicyRoleAssociationQuery = "36";
        public static readonly string DataProfileManagementCommand = "37";
        public static readonly string DataProfileManagementQuery = "38";
    }

    public class Operation
    {
        public static readonly string RegisterEntity = "01";
        public static readonly string UpdateEntity = "02";
        public static readonly string DeleteEntity = "03";
        public static readonly string ListEntities = "04";
        public static readonly string GetEntity = "05";
    }

    public class Level
    {
        public static readonly string BALError = "01";
        public static readonly string DALError = "02";
        public static readonly string APIDependecyError = "03";
    }
}