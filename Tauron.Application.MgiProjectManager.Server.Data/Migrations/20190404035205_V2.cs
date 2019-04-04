using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tauron.Application.MgiProjectManager.Server.Data.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    OperationId = table.Column<string>(nullable: false),
                    OperationType = table.Column<string>(nullable: true),
                    Compled = table.Column<bool>(nullable: false),
                    Removed = table.Column<bool>(nullable: false),
                    CurrentOperation = table.Column<string>(nullable: true),
                    NextOperation = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.OperationId);
                });

            migrationBuilder.CreateTable(
                name: "TimedTasks",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    LastRun = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimedTasks", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "OperationContexts",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    OperationEntityOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationContexts", x => x.Name);
                    table.ForeignKey(
                        name: "FK_OperationContexts_Operations_OperationEntityOperationId",
                        column: x => x.OperationEntityOperationId,
                        principalTable: "Operations",
                        principalColumn: "OperationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationContexts_OperationEntityOperationId",
                table: "OperationContexts",
                column: "OperationEntityOperationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationContexts");

            migrationBuilder.DropTable(
                name: "TimedTasks");

            migrationBuilder.DropTable(
                name: "Operations");
        }
    }
}
