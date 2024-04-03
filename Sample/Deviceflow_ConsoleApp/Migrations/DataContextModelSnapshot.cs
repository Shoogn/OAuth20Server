﻿// <auto-generated />
using Deviceflow_ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Deviceflow_ConsoleApp.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Deviceflow_ConsoleApp.Models.DeviceFlowClientEntity", b =>
                {
                    b.Property<string>("DeviceCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("DeviceCode");

                    b.HasIndex(new[] { "DeviceCode" }, "IX_DeviceFlowClients_DeviceCode")
                        .IsUnique();

                    b.ToTable("DeviceFlowClients");
                });
#pragma warning restore 612, 618
        }
    }
}