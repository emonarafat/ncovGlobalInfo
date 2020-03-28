using Microsoft.EntityFrameworkCore.Migrations;

namespace NCovid.Service.Migrations
{
    public partial class AddFirstCaseColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstCase",
                schema: "Config",
                table: "Country",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstCase",
                schema: "Config",
                table: "Country");
        }
    }
}
