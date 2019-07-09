using EfCoreSample.Doman;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreSample.Persistance.EntityConfiguration
{
    class EmployeeDepartmentEntityConfiguration : IEntityTypeConfiguration<EmployeeDepartment>
    {
        public void Configure(EntityTypeBuilder<EmployeeDepartment> employeeDepartmentBuilder)
        {
            employeeDepartmentBuilder.HasKey(ed => new { ed.EmployeeId, ed.DepartmentId });

            employeeDepartmentBuilder
                .HasOne(ed => ed.Department)
                .WithMany(e => e.EmployeeDepartments)
                .HasForeignKey(ed => ed.DepartmentId);

            employeeDepartmentBuilder
                .HasOne(ed => ed.Employee)
                .WithMany(e => e.EmployeeDepartments)
                .HasForeignKey(ed => ed.EmployeeId);
        }
    }
}
