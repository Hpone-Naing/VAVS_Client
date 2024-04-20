using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAVS_Client.Migrations
{
    public partial class CreatePersonalDetailAndStateDivisionAndTownshipTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_StateDivision",
                columns: table => new
                {
                    StateDivisionPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateDivisionCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    StateDivisionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CityOfRegion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EngShortCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MynShortCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_StateDivision", x => x.StateDivisionPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_Township",
                columns: table => new
                {
                    TownshipPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TownshipCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TownshipName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DistrictCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Township", x => x.TownshipPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_PersonalDetail",
                columns: table => new
                {
                    PersonalPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionID = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PersonTINNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NRCTownshipNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NRCTownshipInitial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NRCType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NRCNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quarter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HousingNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssessStateDivisionID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AssessTownshipID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    RegistrationStatus = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    StateDivisionPkid = table.Column<int>(type: "int", nullable: false),
                    TownshipPkid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PersonalDetail", x => x.PersonalPkid);
                    table.ForeignKey(
                        name: "FK_TB_PersonalDetail_TB_StateDivision_StateDivisionPkid",
                        column: x => x.StateDivisionPkid,
                        principalTable: "TB_StateDivision",
                        principalColumn: "StateDivisionPkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_PersonalDetail_TB_Township_TownshipPkid",
                        column: x => x.TownshipPkid,
                        principalTable: "TB_Township",
                        principalColumn: "TownshipPkid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_PersonalDetail_StateDivisionPkid",
                table: "TB_PersonalDetail",
                column: "StateDivisionPkid");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PersonalDetail_TownshipPkid",
                table: "TB_PersonalDetail",
                column: "TownshipPkid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_PersonalDetail");

            migrationBuilder.DropTable(
                name: "TB_StateDivision");

            migrationBuilder.DropTable(
                name: "TB_Township");
        }
    }
}
