﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using QStack.Framework.Persistent.EFCore;

namespace QStack.Framework.AutoMigration.spiderdb.Migrations
{
    [DbContext(typeof(EFCoreDao))]
    [Migration("20201010134127_20201010214122_spiderdb.Migrations")]
    partial class _20201010214122_spiderdbMigrations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);
#pragma warning restore 612, 618
        }
    }
}
