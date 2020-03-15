using STP.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Common.Models
{
    public class ErrorDTO
    {
        public ErrorCode StatusCode { get; set; }
        public string Message { get; set; }

        public ErrorDTO(ErrorCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
