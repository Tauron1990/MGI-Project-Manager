using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tauron.Application.ProjectManager.ApplicationServer.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         "Jobs",
                                         table => new
                                                  {
                                                      Id         = table.Column<string>(nullable: false),
                                                      TargetDate = table.Column<DateTime>(nullable: false),
                                                      LongName   = table.Column<string>(nullable: true),
                                                      Status     = table.Column<int>(nullable: false),
                                                      Importent  = table.Column<bool>(nullable: false),
                                                      IsActive   = table.Column<bool>(nullable: false)
                                                  },
                                         constraints: table => { table.PrimaryKey("PK_Jobs", x => x.Id); });

            migrationBuilder.CreateTable(
                                         "Setups",
                                         table => new
                                                  {
                                                      Id = table.Column<int>(nullable: false)
                                                                .Annotation("Sqlite:Autoincrement", true),
                                                      Value     = table.Column<int>(nullable: false),
                                                      SetupType = table.Column<int>(nullable: false),
                                                      StartTime = table.Column<DateTime>(nullable: false)
                                                  },
                                         constraints: table => { table.PrimaryKey("PK_Setups", x => x.Id); });

            migrationBuilder.CreateTable(
                                         "Users",
                                         table => new
                                                  {
                                                      Id       = table.Column<string>(nullable: false),
                                                      Password = table.Column<string>(nullable: true),
                                                      Salt     = table.Column<string>(nullable: true)
                                                  },
                                         constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); });

            migrationBuilder.CreateTable(
                                         "Runs",
                                         table => new
                                                  {
                                                      Id = table.Column<int>(nullable: false)
                                                                .Annotation("Sqlite:Autoincrement", true),
                                                      JobId         = table.Column<string>(nullable: true),
                                                      Problem       = table.Column<bool>(nullable: false),
                                                      BigProblem    = table.Column<bool>(nullable: false),
                                                      Iterations    = table.Column<long>(nullable: false),
                                                      Amount        = table.Column<long>(nullable: false),
                                                      Length        = table.Column<int>(nullable: false),
                                                      Width         = table.Column<int>(nullable: false),
                                                      Speed         = table.Column<double>(nullable: false),
                                                      StartTime     = table.Column<DateTime>(nullable: false),
                                                      NormaizedTime = table.Column<TimeSpan>(nullable: false),
                                                      EffectiveTime = table.Column<TimeSpan>(nullable: false),
                                                      SetupTime     = table.Column<int>(nullable: true),
                                                      IterationTime = table.Column<int>(nullable: true),
                                                      IsSaved       = table.Column<bool>(nullable: false),
                                                      IsCompleted   = table.Column<bool>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_Runs", x => x.Id);
                                                          table.ForeignKey(
                                                                           "FK_Runs_Jobs_JobId",
                                                                           x => x.JobId,
                                                                           "Jobs",
                                                                           "Id",
                                                                           onDelete: ReferentialAction.Restrict);
                                                      });

            migrationBuilder.CreateIndex(
                                         "IX_Runs_JobId",
                                         "Runs",
                                         "JobId",
                                         unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       "Runs");

            migrationBuilder.DropTable(
                                       "Setups");

            migrationBuilder.DropTable(
                                       "Users");

            migrationBuilder.DropTable(
                                       "Jobs");
        }
    }
}