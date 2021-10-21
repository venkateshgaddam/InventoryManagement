using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM.Common.API.Controllers
{
    [Route("aws/api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
        protected JsonResult PackageData<T>(T data, System.Net.HttpStatusCode httpStatusCode)
        {
            var result = Json(data);
            result.StatusCode = (int)httpStatusCode;
            return result;
        }
    }
}
