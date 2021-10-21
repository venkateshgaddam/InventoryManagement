using System.Collections.Generic;

namespace Carrier.CCP.Common.Validation.Services
{
    public interface IValidationCacheService
    {
        void SetDefaultValidationFilePath(string filePath);
        IDictionary<string, ValidatorsValidatorProperty> TryGetValidationData(string dtoName, string businessCode = null);
    }
}