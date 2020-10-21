using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceFramework.AutoMigration.sfdb.Migrations
{
    public partial class _20201009113320_sfdbMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IntallPath",
                table: "PluginInfo",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMigration",
                table: "PluginInfo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RouteArea",
                table: "PluginInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntallPath",
                table: "PluginInfo");

            migrationBuilder.DropColumn(
                name: "IsMigration",
                table: "PluginInfo");

            migrationBuilder.DropColumn(
                name: "RouteArea",
                table: "PluginInfo");
        }
    }
}
