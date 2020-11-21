using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace QStack.Framework.AutoMigration.spiderdb.Migrations
{
    public partial class _20201121112534_spiderdbMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    creation_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_modification_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    processor_count = table.Column<int>(type: "integer", nullable: false),
                    total_memory = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "agent_heartbeat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    agent_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    agent_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    cpu_load = table.Column<int>(type: "integer", nullable: false),
                    creation_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    free_memory = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_heartbeat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "spider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreateUserId = table.Column<int>(type: "integer", nullable: true),
                    CREATION_TIME = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CRON = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ENABLED = table.Column<bool>(type: "boolean", nullable: false),
                    ENVIRONMENT = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IMAGE = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LAST_MODIFICATION_TIME = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifyUserId = table.Column<int>(type: "integer", nullable: true),
                    NAME = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    VOLUME = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SPIDER_HISTORIES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    batch = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    container_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreateUserId = table.Column<int>(type: "integer", nullable: true),
                    creation_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifyUserId = table.Column<int>(type: "integer", nullable: true),
                    spider_id = table.Column<int>(type: "integer", nullable: false),
                    SPIDER_NAME = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPIDER_HISTORIES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "statistics",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    creation_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    exit = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failure = table.Column<long>(type: "bigint", nullable: false),
                    last_modification_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    success = table.Column<long>(type: "bigint", nullable: false),
                    total = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statistics", x => x.id);
                });
        }
    }
}
