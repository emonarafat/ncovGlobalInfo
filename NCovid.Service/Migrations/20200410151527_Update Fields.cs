using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NCovid.Service.Migrations
{
    public partial class UpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoronaInfoId",
                schema: "Config",
                table: "Country",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestPerOneMillion",
                schema: "Config",
                table: "Country",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalTest",
                schema: "Config",
                table: "Country",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoronaInfo",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoronaInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Country_CoronaInfoId",
                schema: "Config",
                table: "Country",
                column: "CoronaInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Country_CoronaInfo_CoronaInfoId",
                schema: "Config",
                table: "Country",
                column: "CoronaInfoId",
                principalSchema: "Config",
                principalTable: "CoronaInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Country_CoronaInfo_CoronaInfoId",
                schema: "Config",
                table: "Country");

            migrationBuilder.DropTable(
                name: "CoronaInfo",
                schema: "Config");

            migrationBuilder.DropIndex(
                name: "IX_Country_CoronaInfoId",
                schema: "Config",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CoronaInfoId",
                schema: "Config",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "TestPerOneMillion",
                schema: "Config",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "TotalTest",
                schema: "Config",
                table: "Country");
        }
    }
}
