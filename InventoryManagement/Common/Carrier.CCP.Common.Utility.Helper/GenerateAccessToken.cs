using Carrier.CCP.Common.Utils.Exception;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    public class GenerateAccessToken : IGenerateAccessToken
    {

        public async Task<string> GenerateCognitoTokenAsync(string cognitoDomain, string clientId, string clientSecret, string scope)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(cognitoDomain);
            string url = cognitoDomain + "/oauth2/token?grant_type=client_credentials&client_id=" + clientId + "&scope=" + scope + "";
            var keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("scope", scope)
            };
            string accessToken = "";
            try
            {

                string base64EncodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                var request =
                    new HttpRequestMessage(HttpMethod.Post, "/oauth2/token");
                request.Content = new FormUrlEncodedContent(keyValues);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedMessage);
                HttpResponseMessage httpResponse = await httpClient.SendAsync(request).ConfigureAwait(false);

                accessToken = await httpResponse.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(accessToken))
                    throw new CcpSecurityException(SecurityErrorCode.InvalidToken, CcpErrorStatus.Unauthenticated,
                        SecurityErrorCode.InvalidToken.GetErrorReferenceDescription());
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(accessToken)["access_token"];
            }
            catch (CcpSecurityException)
            {
                throw;
            }
            catch (NullReferenceException)
            {
                throw new CcpSecurityException(SecurityErrorCode.NullReferenceException, CcpErrorStatus.Internal,
                    "NullReferenceException occured in ValidateAccessToken");
            }
            catch (ArgumentNullException)
            {
                throw new CcpSecurityException(SecurityErrorCode.NullReferenceException, CcpErrorStatus.Internal,
                    "NullReferenceException occured in ValidateAccessToken");
            }
            catch (Exception e)
            {
                throw new CcpBaseException(CcpErrorStatus.Internal, e.Message, "0000");
            }

        }
        public async Task<string> GenerateAadAccessTokenAsync(string clientId, string clientSecret, string resourceId,
            string aadInstance, string tenant)
        {
            try
            {
                var authority = string.Concat(aadInstance, tenant);

                var authContext = new AuthenticationContext(authority);
                var clientCredential = new ClientCredential(clientId, clientSecret);

                var result = await authContext.AcquireTokenAsync(resourceId,
                    clientCredential).ConfigureAwait(false);

                var accessToken = result.AccessToken;
                if (string.IsNullOrWhiteSpace(accessToken))
                    throw new CcpSecurityException(SecurityErrorCode.InvalidToken, CcpErrorStatus.Unauthenticated,
                        SecurityErrorCode.InvalidToken.GetErrorReferenceDescription());

                return accessToken;
            }
            catch (CcpSecurityException)
            {
                throw;
            }
            catch (NullReferenceException)
            {
                throw new CcpSecurityException(SecurityErrorCode.NullReferenceException, CcpErrorStatus.Internal,
                    "NullReferenceException occured in ValidateAccessToken");
            }
            catch (ArgumentNullException)
            {
                throw new CcpSecurityException(SecurityErrorCode.NullReferenceException, CcpErrorStatus.Internal,
                    "NullReferenceException occured in ValidateAccessToken");
            }
            catch (AdalServiceException e)
            {
                throw new CcpSecurityException(SecurityErrorCode.AdalServiceException, CcpErrorStatus.Internal,
                    string.Format(CultureInfo.InvariantCulture, "Original Code and Message= {0}: {1}", e.ErrorCode,
                        e.ServiceErrorCodes));
            }
            catch (AdalException e)
            {
                throw new CcpSecurityException(SecurityErrorCode.AdalException, CcpErrorStatus.Internal,
                    string.Format(CultureInfo.InvariantCulture, "Original Code and Message= {0}: {1}", e.ErrorCode,
                        e.Message));
            }
            catch (Exception e)
            {
                throw new CcpBaseException(CcpErrorStatus.Internal, e.Message, "0000");
            }
        }
    }
}