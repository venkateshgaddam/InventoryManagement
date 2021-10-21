using System;
using System.Json;

namespace Carrier.CCP.Common.Utils.Validation
{
    public static class ValidateJson
    {
        public static bool IsValid(string jsonString)
        {
            try
            {
                var obj = JsonValue.Parse(jsonString);
                return true;
            }
            catch (FormatException)
            {
                //Invalid json format
                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}