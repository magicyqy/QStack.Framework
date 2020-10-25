﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using QStack.Framework.Persistent.EFCore;


namespace QStack.Framework.AutoMigration.spiderdb.Migrations
{
    [DbContext(typeof(EFCoreDao))]
    [Migration("20201010134413_20201010214413_spiderdb.Migrations")]
    partial class _20201010214413_spiderdbMigrations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("QStack.Blog.Docker.Crawler.Models.AgentHeartbeat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AgentId")
                        .HasColumnName("agent_id")
                        .HasColumnType("character varying(36)")
                        .HasMaxLength(36);

                    b.Property<string>("AgentName")
                        .HasColumnName("agent_name")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<int>("CpuLoad")
                        .HasColumnName("cpu_load")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("CreationTime")
                        .HasColumnName("creation_time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FreeMemory")
                        .HasColumnName("free_memory")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("agent_heartbeat");
                });

            modelBuilder.Entity("QStack.Blog.Docker.Crawler.Models.AgentInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(36)")
                        .HasMaxLength(36);

                    b.Property<DateTimeOffset>("CreationTime")
                        .HasColumnName("creation_time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnName("is_deleted")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("LastModificationTime")
                        .HasColumnName("last_modification_time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<int>("ProcessorCount")
                        .HasColumnName("processor_count")
                        .HasColumnType("integer");

                    b.Property<int>("TotalMemory")
                        .HasColumnName("total_memory")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("agent");
                });

            modelBuilder.Entity("QStack.Blog.Docker.Crawler.Models.Spider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("CreationTime")
                        .HasColumnName("CREATION_TIME")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Cron")
                        .IsRequired()
                        .HasColumnName("CRON")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<bool>("Enabled")
                        .HasColumnName("ENABLED")
                        .HasColumnType("boolean");

                    b.Property<string>("Environment")
                        .HasColumnName("ENVIRONMENT")
                        .HasColumnType("character varying(2000)")
                        .HasMaxLength(2000);

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnName("IMAGE")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("LastModificationTime")
                        .HasColumnName("LAST_MODIFICATION_TIME")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NAME")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Volume")
                        .HasColumnName("VOLUME")
                        .HasColumnType("character varying(2000)")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.ToTable("spider");
                });

            modelBuilder.Entity("QStack.Blog.Docker.Crawler.Models.SpiderHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Batch")
                        .HasColumnName("batch")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("ContainerId")
                        .HasColumnName("container_id")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("CreationTime")
                        .HasColumnName("creation_time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<int>("SpiderId")
                        .HasColumnName("spider_id")
                        .HasColumnType("integer");

                    b.Property<string>("SpiderName")
                        .IsRequired()
                        .HasColumnName("SPIDER_NAME")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnName("status")
                        .HasColumnType("character varying(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("SPIDER_HISTORIES");
                });

            modelBuilder.Entity("QStack.Blog.Docker.Crawler.Models.SpiderStatistics", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("character varying(36)")
                        .HasMaxLength(36);

                    b.Property<DateTimeOffset>("CreationTime")
                        .HasColumnName("creation_time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("Exit")
                        .HasColumnName("exit")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Failure")
                        .HasColumnName("failure")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("LastModificationTime")
                        .HasColumnName("last_modification_time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset?>("Start")
                        .HasColumnName("start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Success")
                        .HasColumnName("success")
                        .HasColumnType("bigint");

                    b.Property<long>("Total")
                        .HasColumnName("total")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("statistics");
                });
#pragma warning restore 612, 618
        }
    }
}
