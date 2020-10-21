using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceFramework.AutoMigration.sfdb.Migrations
{
    public partial class _20200901103538_sfdbMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PluginId",
                table: "PluginInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PluginId",
                table: "PluginInfo",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
