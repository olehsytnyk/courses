using STP.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Identity.Domain.Entities
{
    public class UserAvatar : IEntity<long>
    {
        public long Id { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

    }
}
