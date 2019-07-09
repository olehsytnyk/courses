using EfCoreSample.Doman.Entities;
using EfCoreSample.Doman.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EfCoreSample.Persistance.EntityConfiguration
{
    class ProjectEntityConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> projectBuilder)
        {
            projectBuilder.ToTable("Project", EfCoreSampleDbContext.SchemaName);

            projectBuilder.HasKey(p => p.Id);

            projectBuilder
                .HasOne(p => p.Employee)
                .WithMany(p => p.Projects);



            projectBuilder.Property(p => p.Title).HasMaxLength(128);
            projectBuilder.Property(p => p.Discription).HasMaxLength(128);

            projectBuilder.Property(p => p.Status)
            .HasMaxLength(50)
            .HasConversion(
                e => e.ToString(),
                e => (StatusType)StatusType.Parse(typeof(StatusType), e))
                .IsUnicode(false);

            projectBuilder.Property(ut => ut.LastUpdateTime)
                .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)")
                .ValueGeneratedOnUpdate();

            projectBuilder.Property(st => st.StartTime)
                .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)")
                .ValueGeneratedOnUpdate();

            projectBuilder.Property(et => et.EndTime)
                .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)")
                .ValueGeneratedOnUpdate();

            projectBuilder.HasIndex(p =>
                        new
                        {
                            p.Title,
                            p.Discription,
                        }).ForMySqlIsFullText();

            projectBuilder.HasData(
                new Project()
                {
                    Id = 1,
                    Title = "Title",
                    Discription = "Discription"
                },
                new Project()
                {
                    Id = 2,
                    Title = "Title2",
                    Discription = "Discription2"
                });

        }
    }
}