using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    public interface IResolver
    {
        Task<string> GetBusinessSchemaAsync(string businessId);
        string GetBusinessSchema(string businessId);
    }
}