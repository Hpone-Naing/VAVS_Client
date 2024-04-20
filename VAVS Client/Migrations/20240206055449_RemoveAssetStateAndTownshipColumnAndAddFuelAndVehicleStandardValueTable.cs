using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAVS_Client.Migrations
{
    public partial class RemoveAssetStateAndTownshipColumnAndAddFuelAndVehicleStandardValueTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessStateDivisionID",
                table: "TB_PersonalDetail");

            migrationBuilder.DropColumn(
                name: "AssessTownshipID",
                table: "TB_PersonalDetail");

            migrationBuilder.CreateTable(
                name: "TB_FuelType",
                columns: table => new
                {
                    FuelTypePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FuelType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_FuelType", x => x.FuelTypePkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_VehicleStandardValue",
                columns: table => new
                {
                    VehicleStandardValuePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CountryOfMade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleBrand = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BuildType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModelYear = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    EnginePower = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StandardValue = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OfficeLetterNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AttachFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    StateDivisionPkid = table.Column<int>(type: "int", nullable: false),
                    FuelTypePkid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VehicleStandardValue", x => x.VehicleStandardValuePkid);
                    table.ForeignKey(
                        name: "FK_TB_VehicleStandardValue_TB_FuelType_FuelTypePkid",
                        column: x => x.FuelTypePkid,
                        principalTable: "TB_FuelType",
                        principalColumn: "FuelTypePkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_VehicleStandardValue_TB_StateDivision_StateDivisionPkid",
                        column: x => x.StateDivisionPkid,
                        principalTable: "TB_StateDivision",
                        principalColumn: "StateDivisionPkid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_VehicleStandardValue_FuelTypePkid",
                table: "TB_VehicleStandardValue",
                column: "FuelTypePkid");

            migrationBuilder.CreateIndex(
                name: "IX_TB_VehicleStandardValue_StateDivisionPkid",
                table: "TB_VehicleStandardValue",
                column: "StateDivisionPkid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_VehicleStandardValue");

            migrationBuilder.DropTable(
                name: "TB_FuelType");

            migrationBuilder.AddColumn<string>(
                name: "AssessStateDivisionID",
                table: "TB_PersonalDetail",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssessTownshipID",
                table: "TB_PersonalDetail",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                defaultValue: "");
        }
    }
}
