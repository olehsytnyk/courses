using EfCoreSample.Doman.Entities;
using System;
using System.Collections.Generic;

namespace EfCoreSample.Doman
{
    public class Employee
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long? ReportsToId { get; set; }

        public Employee ReportsTo { get; set; }

        public DateTime LastModified { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public ICollection<Employee> ReportsToEmployees { get; set; }

        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
