using Microsoft.EntityFrameworkCore.Migrations;

namespace Verdict.DataAccess.Migrations
{
    public partial class AddSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Search",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "Search");
        }
    }
}
