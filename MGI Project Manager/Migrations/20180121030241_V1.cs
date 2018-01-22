using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tauron.Application.MgiProjectManager.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         "JobEntities",
                                         table => new
                                                  {
                                                      Id         = table.Column<string>(nullable: false),
                                                      Importent  = table.Column<bool>(nullable: false),
                                                      IsActive   = table.Column<bool>(nullable: false),
                                                      LongName   = table.Column<string>(nullable: true),
                                                      Status     = table.Column<int>(nullable: false),
                                                      TargetDate = table.Column<DateTime>(nullable: false)
                                                  },
                                         constraints: table => { table.PrimaryKey("PK_JobEntities", x => x.Id); });

            migrationBuilder.CreateTable(
                                         "RunEntities",
                                         table => new
                                                  {
                                                      Id = table.Column<int>(nullable: false)
                                                                .Annotation("Sqlite:Autoincrement", true),
                                                      Amount        = table.Column<long>(nullable: false),
                                                      BigProblem    = table.Column<bool>(nullable: false),
                                                      EffectiveTime = table.Column<TimeSpan>(nullable: false),
                                                      IsValid       = table.Column<bool>(nullable: false),
                                                      IterationTime = table.Column<int>(nullable: true),
                                                      Iterations    = table.Column<long>(nullable: false),
                                                      JobId         = table.Column<string>(nullable: true),
                                                      Length        = table.Column<int>(nullable: false),
                                                      NormaizedTime = table.Column<TimeSpan>(nullable: false),
                                                      Problem       = table.Column<bool>(nullable: false),
                                                      SetupTime     = table.Column<int>(nullable: true),
                                                      Speed         = table.Column<double>(nullable: false),
                                                      StartTime     = table.Column<DateTime>(nullable: false),
                                                      Width         = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_RunEntities", x => x.Id);
                                                          table.ForeignKey(
                                                                           "FK_RunEntities_JobEntities_JobId",
                                                                           x => x.JobId,
                                                                           "JobEntities",
                                                                           "Id",
                                                                           onDelete: ReferentialAction.Restrict);
                                                      });

            migrationBuilder.CreateIndex(
                                         "IX_RunEntities_JobId",
                                         "RunEntities",
                                         "JobId",
                                         unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       "RunEntities");

            migrationBuilder.DropTable(
                                       "JobEntities");
        }
    }
}