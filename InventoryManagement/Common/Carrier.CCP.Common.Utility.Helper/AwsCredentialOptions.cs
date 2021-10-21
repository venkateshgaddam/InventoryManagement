
using Amazon.Runtime.CredentialManagement;

namespace Carrier.CCP.Common.Utility.Helper
{
    public class AwsCredentialOptions
    {
        private readonly string endPoint;
        public AwsCredentialOptions(string endPoint)
        {
            this.endPoint = endPoint;
        }
    }
}
