namespace EfCoreSample.Doman
{
    public class EmployeeDepartment
    {
        public long EmployeeId { get; set; }
        public long DepartmentId { get; set; }
        public Employee Employee { get; set; }
        public Department Department { get; set; }
    }
}
