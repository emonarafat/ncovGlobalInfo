using Microsoft.EntityFrameworkCore.Migrations;

namespace NCovid.Service.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Config");

            migrationBuilder.CreateTable(
                name: "Country",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<int>(nullable: false),
                    Cases = table.Column<int>(nullable: false),
                    CasesPerOneMillion = table.Column<decimal>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    Critical = table.Column<int>(nullable: false),
                    Deaths = table.Column<int>(nullable: false),
                    DeathsPerOneMillion = table.Column<decimal>(nullable: false),
                    Recovered = table.Column<int>(nullable: false),
                    TodayCases = table.Column<int>(nullable: false),
                    TodayDeaths = table.Column<int>(nullable: false),
                    FirstCase = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalInfo",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cases = table.Column<int>(nullable: false),
                    Deaths = table.Column<int>(nullable: false),
                    Recovered = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Country",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "GlobalInfo",
                schema: "Config");
        }
    }
}
