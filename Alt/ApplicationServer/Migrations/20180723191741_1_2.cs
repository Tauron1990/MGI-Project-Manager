using Microsoft.EntityFrameworkCore.Migrations;

namespace Tauron.Application.ProjectManager.ApplicationServer.Migrations
{
    public partial class _1_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Runs_JobId",
                table: "Runs");

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Runs_JobId",
                table: "Runs",
                column: "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Runs_JobId",
                table: "Runs");

            migrationBuilder.CreateIndex(
                name: "IX_Runs_JobId",
                table: "Runs",
                column: "JobId",
                unique: true);
        }
    }
}
