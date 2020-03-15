
using Microsoft.AspNetCore.Identity;
using STP.Interfaces;
using STP.Interfaces.Enums;
using System;

namespace STP.Identity.Domain.Entities
{
    public class User : IdentityUser, IEntity<string>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public UserAvatar UserAvatar { get; set; }
    }
}
