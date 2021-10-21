using Microsoft.AspNetCore.Mvc;
using IM.Common.API.Controllers;
using IM.Common.Model.EntityModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace IM.UserManagement.V1.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("aws/api/[Controller]")]
    [ApiController]
    [ApiVersion("1")]
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
        [MapToApiVersion("1")]
        [AllowAnonymous]
        public IActionResult RegisterUser([FromBody] RegisterUserModel userModel, [FromForm] IFormFile file)
        {
            return null;
            //var request = new HttpRequestMessage
            //{
            //    Content = httpContent,
            //    Method = HttpMethod.Post,
            //    RequestUri = new Uri(endPoint + "/" + methodUri),
            //    Headers = { { HttpRequestHeader.ContentType.ToString(), "application/json-patch+json" }, { HttpRequestHeader.Accept.ToString(), "application/json" } }
            //};
            //var response = await httpClient.SendAsync(request);
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public async Task<IActionResult> UploadDocument([FromHeader] string documentType, [FromForm] IFormFile file)
        {
            Console.WriteLine(file);
            Console.WriteLine(documentType);
            // TODO: handle file upload  
            return await Task.FromResult(Ok());
        }

        #endregion

    }
}
