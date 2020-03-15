using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Profile.Domain.Entities;

namespace STP.Profile.Persistence.EntityConfiguration
{
    public class TraderInfoEntityConfiguration : IEntityTypeConfiguration<TraderInfoEntity>
    {
        public void Configure(EntityTypeBuilder<TraderInfoEntity> builder)
        {
            builder.ToTable("traderinfo");

            builder.HasKey(ti => ti.Id);

            builder.Property(ti => ti.LastChanged)
                .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)")
                .ValueGeneratedOnAddOrUpdate();

            builder.HasData(
                new TraderInfoEntity()
                {
                    Id = "UserID 1",
                    CopyCount = 5,
                    ProfitLoss = 55555
                },
                new TraderInfoEntity()
                {
                    Id = "UserID 2",
                    CopyCount = 6700,
                    ProfitLoss = 1
                },
                new TraderInfoEntity()
                {
                    Id = "UserID 3",
                    CopyCount = 0,
                    ProfitLoss = 900
                }
                );
        }
    }
}
