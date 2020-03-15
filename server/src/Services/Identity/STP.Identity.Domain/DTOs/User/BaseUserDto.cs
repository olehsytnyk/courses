using STP.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Identity.Domain.DTOs.User
{
    public class BaseUserDto
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public Gender? Gender { get; set; }

        public long? DateOfBirth { get; set; }
    }
}
