using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM.UserManagement.Service
{
    public class AwsCognitoService : IAwsCognitoService
    {

        private IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider;

        public AwsCognitoService(IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider)
        {
            this.amazonCognitoIdentityProvider = amazonCognitoIdentityProvider;
        }
        public async Task<AdminCreateUserResponse> AddUserInUserPool(AdminCreateUserRequest adminCreateUserRequest)
        {
            var result = await amazonCognitoIdentityProvider.AdminCreateUserAsync(adminCreateUserRequest);
            throw new NotImplementedException();
        }

        public Task<SetUserSettingsResponse> UpdateUserInUserPool(SetUserSettingsRequest setUserSettingsRequest)
        {
            throw new NotImplementedException();
        }
    }
}
