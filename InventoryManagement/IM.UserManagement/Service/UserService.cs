using IM.Common.Data.Entity;
using IM.Common.Repository.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM.UserManagement.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class UserService : IUserService
    {

       

        private readonly IGenericRepository<User> userRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amazonS3Service"></param>
        /// <param name="userRepository"></param>
        public UserService(IGenericRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }


        public Task RegisterUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
