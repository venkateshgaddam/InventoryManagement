using Amazon.CognitoIdentityProvider.Model;
using System.Threading.Tasks;

namespace IM.UserManagement.Service
{
    public interface IAwsCognitoService
    {

        Task<AdminCreateUserResponse> AddUserInUserPool(AdminCreateUserRequest adminCreateUserRequest);


        Task<SetUserSettingsResponse> UpdateUserInUserPool(SetUserSettingsRequest setUserSettingsRequest);

    }
}
