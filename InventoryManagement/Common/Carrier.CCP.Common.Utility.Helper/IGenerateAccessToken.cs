using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    public interface IGenerateAccessToken
    {
        Task<string> GenerateAadAccessTokenAsync(string clientId, string clientSecret, string resourceId, string aadInstance,
            string tenant);
        Task<string> GenerateCognitoTokenAsync(string cognitoDomain, string clientId, string clientSecret, string scope);
    }
}