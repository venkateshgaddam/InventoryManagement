using System;
using System.Net;

namespace IM.Common.logging.Service.Models
{
    public class BaseCustomException : Exception
    {
        public BaseCustomException(string message, string description, int code) : base(message)
        {
            Code = code;
            Description = description;
        }

        public int Code { get; }

        public string Description { get; }
    }

    public class CustomErrorResponse
    {
        public string Message { get; set; }
        public string Description { get; set; }
    }

    public class NotFoundCustomException : BaseCustomException
    {
        public NotFoundCustomException(string message, string description) : base(message, description,
            (int)HttpStatusCode.NotFound)
        {
        }
    }
}