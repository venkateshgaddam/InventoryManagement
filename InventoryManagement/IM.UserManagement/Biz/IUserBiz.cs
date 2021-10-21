using IM.Common.Model.EntityModels;
using System.Threading.Tasks;

namespace IM.UserManagement.Biz
{
    public interface IUserBiz
    {

        Task RegisterUserAsync(RegisterUserModel registerUserModel);

    }
}
