using IM.Common.API.Controllers;
using IM.Common.Model.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IM.UserManagement.Controllers.V2
{
    /// <summary>
    /// 
    /// </summary>
    [Route("aws/api/[controller]")]
    [ApiController]
    [ApiVersion("2")]
    public class UserController : BaseController
    {
        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region ActionMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [MapToApiVersion("2")]
        [AllowAnonymous]
        public IActionResult RegisterUser([FromBody] RegisterUserModel userModel, [FromForm] IFormFile formfile)
        {
            return null;
        }


        #endregion


    }
}