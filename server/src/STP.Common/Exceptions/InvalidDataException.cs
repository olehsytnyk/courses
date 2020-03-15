using STP.Common.Extensions;
using STP.Common.Models;
using STP.Interfaces.Enums;
using System;


namespace STP.Common.Exceptions
{
    public class InvalidDataException : Exception
    {
        public readonly ErrorCode ErrorCode;
        private ErrorDTO _error;
        public ErrorDTO Error
        {
            get
            {
                return _error ??
                    new ErrorDTO(ErrorCode, ErrorCode.GetDescription());
            }
        }


        public InvalidDataException(ErrorCode errorCode, string message = null)
            : base(message)
        {
            this.ErrorCode = errorCode;
            _error = new ErrorDTO(this.ErrorCode, message ?? ErrorCode.GetDescription());
        }
    }
}
