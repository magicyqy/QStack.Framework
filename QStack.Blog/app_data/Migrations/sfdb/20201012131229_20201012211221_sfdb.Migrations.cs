using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceFramework.AutoMigration.sfdb.Migrations
{
    public partial class _20201012211221_sfdbMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "Function",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "Function");
        }
    }
}
