using IM.Common.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM.UserManagement.Service
{
    public interface IUserService
    {

        Task RegisterUserAsync(User user);
    }
}
