using STP.Identity.Domain.DTOs.User;
using STP.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Identity.Domain.DTOs.User
{
    public class CreateUserDto
    {
        public string UserName { get; set; }
           
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }
    }
}
