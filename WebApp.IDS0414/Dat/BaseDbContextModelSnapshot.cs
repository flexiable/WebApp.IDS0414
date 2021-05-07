﻿// <auto-generated />
using System;
using IDS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebApp.IDS0414.Dat
{
    [DbContext(typeof(BaseDbContext))]
    partial class BaseDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("IDS.Entity.ClientTokenLog", b =>
                {
                    b.Property<int>("ZId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AccessTokenLifetime")
                        .HasColumnType("int");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<string>("ClientName")
                        .HasColumnType("text");

                    b.Property<string>("ClientRequestParam")
                        .HasColumnType("text");

                    b.Property<string>("ClientResponseBody")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DeleteTime")
                        .HasColumnType("datetime");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("ModifyDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("ZId");

                    b.ToTable("ClientTokenLog");
                });
#pragma warning restore 612, 618
        }
    }
}