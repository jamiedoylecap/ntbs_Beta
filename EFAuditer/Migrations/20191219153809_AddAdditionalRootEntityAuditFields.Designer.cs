﻿// <auto-generated />
using System;
using EFAuditer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFAuditer.Migrations
{
    [DbContext(typeof(AuditDatabaseContext))]
    [Migration("20191219153809_AddAdditionalRootEntityAuditFields")]
    partial class AddAdditionalRootEntityAuditFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EFAuditer.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuditData");

                    b.Property<DateTime>("AuditDateTime");

                    b.Property<string>("AuditDetails");

                    b.Property<string>("AuditUser");

                    b.Property<string>("EntityType");

                    b.Property<string>("EventType");

                    b.Property<int>("OriginalId");

                    b.Property<string>("RootEntity");

                    b.Property<int?>("RootId");

                    b.HasKey("Id");

                    b.ToTable("AuditLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
