using EfCoreSample.Doman.Enums;
using System;

namespace EfCoreSample.Doman.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Discription { get; set; }

        public StatusType Status { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}