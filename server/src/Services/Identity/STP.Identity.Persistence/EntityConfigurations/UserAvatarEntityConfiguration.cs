using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Identity.Persistence.EntityConfigurations
{
    class UserAvatarEntityConfiguration: IEntityTypeConfiguration<UserAvatar>
    {
        public void Configure(EntityTypeBuilder<UserAvatar> avatarBuilder)
        {
            avatarBuilder.ToTable("aspnetuseravatars");

            avatarBuilder
                .HasOne<User>(ad => ad.User)
    .            WithOne(s => s.UserAvatar)
                .HasForeignKey<UserAvatar>(ad => ad.UserId);

            avatarBuilder.Property(a => a.FileExtension).HasMaxLength(5).IsRequired();
            avatarBuilder.Property(a => a.FileName).HasMaxLength(254).IsRequired();  

            avatarBuilder.HasIndex(a =>
            new
            {
                a.FileName,
                a.FileExtension
            }).ForMySqlIsFullText();
        }
    }
}
