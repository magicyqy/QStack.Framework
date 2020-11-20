using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace QStack.Framework.AutoMigration.spiderdb.Migrations
{
    public partial class _20201104195608_spiderdbMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    processor_count = table.Column<int>(nullable: false),
                    total_memory = table.Column<int>(nullable: false),
                    last_modification_time = table.Column<DateTimeOffset>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    creation_time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agent_heartbeat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    agent_id = table.Column<string>(maxLength: 36, nullable: true),
                    agent_name = table.Column<string>(maxLength: 255, nullable: true),
                    free_memory = table.Column<int>(nullable: false),
                    cpu_load = table.Column<int>(nullable: false),
                    creation_time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_heartbeat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "spider",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    LastModifyDate = table.Column<DateTime>(nullable: true),
                    CreateUserId = table.Column<int>(nullable: true),
                    LastModifyUserId = table.Column<int>(nullable: true),
                    ENABLED = table.Column<bool>(nullable: false),
                    NAME = table.Column<string>(maxLength: 255, nullable: false),
                    IMAGE = table.Column<string>(maxLength: 255, nullable: false),
                    CRON = table.Column<string>(maxLength: 255, nullable: false),
                    ENVIRONMENT = table.Column<string>(maxLength: 2000, nullable: true),
                    VOLUME = table.Column<string>(maxLength: 2000, nullable: true),
                    CREATION_TIME = table.Column<DateTimeOffset>(nullable: false),
                    LAST_MODIFICATION_TIME = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SPIDER_HISTORIES",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    LastModifyDate = table.Column<DateTime>(nullable: true),
                    CreateUserId = table.Column<int>(nullable: true),
                    LastModifyUserId = table.Column<int>(nullable: true),
                    spider_id = table.Column<int>(nullable: false),
                    SPIDER_NAME = table.Column<string>(maxLength: 255, nullable: false),
                    container_id = table.Column<string>(maxLength: 100, nullable: true),
                    batch = table.Column<string>(maxLength: 100, nullable: true),
                    creation_time = table.Column<DateTimeOffset>(nullable: false),
                    status = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPIDER_HISTORIES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "statistics",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 36, nullable: false),
                    name = table.Column<string>(maxLength: 255, nullable: true),
                    start = table.Column<DateTimeOffset>(nullable: true),
                    exit = table.Column<DateTimeOffset>(nullable: true),
                    total = table.Column<long>(nullable: false),
                    success = table.Column<long>(nullable: false),
                    failure = table.Column<long>(nullable: false),
                    last_modification_time = table.Column<DateTimeOffset>(nullable: false),
                    creation_time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statistics", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent");

            migrationBuilder.DropTable(
                name: "agent_heartbeat");

            migrationBuilder.DropTable(
                name: "spider");

            migrationBuilder.DropTable(
                name: "SPIDER_HISTORIES");

            migrationBuilder.DropTable(
                name: "statistics");
        }
    }
}
