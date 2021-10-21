namespace Carrier.CCP.Common.Utils.Validation
{
    public static class CommonValidationConstants
    {
        #region regular expressions

        public static readonly string NameRegExpression = @"^[A-Za-z0-9][ A-Za-z0-9',&()$@#\\/-]*$";
        public static readonly string CityRegExpression = @"^[A-Za-z0-9][ A-Za-z0-9]*$";
        public static readonly string StateRegExpression = @"^[A-Za-z0-9][ A-Za-z0-9]*$";
        public static readonly string MobileRegExpression = @"^([0-9]+)$";

        public static readonly string EmailRegExpression =
            @"^(([^<>()[\]*{}\\.,;:\s@""]+(\.[^<>()[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

        public static readonly string PostalRegExpression = @"^[A-Za-z0-9][ A-Za-z0-9'-]*$";
        public static readonly string AddressRegExpression = @"^[A-Za-z0-9][ A-Za-z0-9#\-@$,.\/\\]*$";
        public static readonly string AlphaNumericWithSpaceRegExpression = @"^[A-Za-z0-9][ A-Za-z0-9]*$";

        #endregion
    }
}