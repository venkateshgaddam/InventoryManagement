using IM.Common.Logging.Service;
using IM.Common.Model.EntityModels;
using IM.UserManagement.Service;
using System;
using System.Threading.Tasks;

namespace IM.UserManagement.Biz
{
    /// <summary>
    /// 
    /// </summary>
    public class UserBiz : IUserBiz
    {
        private readonly IAmazonS3Service amazonS3Service;

        private readonly IUserService userService;

        private readonly IAwsCognitoService awsCognitoService;

        private readonly IPlatformLogger platformLogger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="amazonS3Service"></param>
        /// <param name="awsCognitoService"></param>
        /// <param name="platformLogger"></param>
        public UserBiz(IUserService userService, IAmazonS3Service amazonS3Service, IAwsCognitoService awsCognitoService, IPlatformLogger platformLogger)
        {
            this.amazonS3Service = amazonS3Service;
            this.awsCognitoService = awsCognitoService;
            this.userService = userService;
            this.platformLogger = platformLogger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerUserModel"></param>
        /// <returns></returns>
        public Task RegisterUserAsync(RegisterUserModel registerUserModel)
        {
            try
            {
                platformLogger.InstanceLogger.Information($"{nameof(RegisterUserAsync)} Started");

            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }
    }
}
