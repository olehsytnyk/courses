﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using STP.Profile.Persistence.Context;

namespace STP.Profile.Persistence.Migrations
{
    [DbContext(typeof(ProfileDbContext))]
    [Migration("20190812134331_addSeedingTraderInfo")]
    partial class addSeedingTraderInfo
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("STP.Profile.Domain.Entities.OrderEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Action");

                    b.Property<long>("MarketId");

                    b.Property<double>("Price");

                    b.Property<int>("Quantity");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("orders");
                });

            modelBuilder.Entity("STP.Profile.Domain.Entities.PositionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("AveragePrice");

                    b.Property<long>("EntryOrderId");

                    b.Property<long>("ExitOrderId");

                    b.Property<int>("Kind");

                    b.Property<long>("MarketId");

                    b.Property<double>("ProfitLoss");

                    b.Property<long>("Quantity");

                    b.Property<DateTime>("Timestamp");

                    b.Property<double>("UnrealizedProfitLoss");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("position");
                });

            modelBuilder.Entity("STP.Profile.Domain.Entities.TraderInfoEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CopyCount");

                    b.Property<DateTime>("LastChanged")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)");

                    b.Property<double>("ProfitLoss");

                    b.HasKey("Id");

                    b.ToTable("traderinfo");

                    b.HasData(
                        new
                        {
                            Id = "UserID 1",
                            CopyCount = 5,
                            LastChanged = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ProfitLoss = 55555.0
                        },
                        new
                        {
                            Id = "UserID 2",
                            CopyCount = 6700,
                            LastChanged = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ProfitLoss = 1.0
                        },
                        new
                        {
                            Id = "UserID 3",
                            CopyCount = 0,
                            LastChanged = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ProfitLoss = 900.0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
