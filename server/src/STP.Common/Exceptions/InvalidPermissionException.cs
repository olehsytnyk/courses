using STP.Common.Extensions;
using STP.Common.Models;
using STP.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Common.Exceptions
{
    public class InvalidPermissionException : Exception
    {
        private readonly ErrorCode ErrorCode;

        private ErrorDTO _error;
        public ErrorDTO Error
        {
            get
            {
                return _error ??
                    new ErrorDTO(ErrorCode, ErrorCode.GetDescription());
            }
        }
       
        public InvalidPermissionException(ErrorCode errorCode, string message = null)
             : base(message)
        {
            this.ErrorCode = errorCode;
            _error = new ErrorDTO(this.ErrorCode, message ?? ErrorCode.GetDescription());
        }
    }
}
