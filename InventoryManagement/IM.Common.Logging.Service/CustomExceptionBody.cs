using System;

namespace IM.Common.Logging.Service
{
    public class CustomExceptionBody
    {
        public int ExceptionCode { get; set; }

        public string ExceptionMessage { get; set; }

        public string StackSource { get; set; }

        public DateTime TimeOfException { get; set; }
    }
}