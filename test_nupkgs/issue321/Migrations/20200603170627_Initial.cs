using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WpfAppSqliteTesting.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<long>(nullable: false),
                    ClassId = table.Column<int>(nullable: false),
                    ClassName = table.Column<string>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    GroupName = table.Column<string>(nullable: true),
                    TechnicalComponentId = table.Column<string>(nullable: true),
                    StateId = table.Column<int>(nullable: false),
                    StateName = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Parameter1 = table.Column<string>(nullable: true),
                    Parameter2 = table.Column<string>(nullable: true),
                    Parameter3 = table.Column<string>(nullable: true),
                    Parameter4 = table.Column<string>(nullable: true),
                    EventDateTime = table.Column<DateTime>(nullable: false),
                    EventDateTimeUtc = table.Column<DateTime>(nullable: false),
                    CreatedDateTimeUtc = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    SourceMessageId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
}
