using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAVS_Client.Migrations
{
    public partial class AddTaxValidationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StateDivisionPkid",
                table: "TB_Township",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TB_Payment",
                columns: table => new
                {
                    PaymentPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PaymentAmount = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Payment", x => x.PaymentPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_TaxValidation",
                columns: table => new
                {
                    TaxValidationPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonTINNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PersonNRC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CountryOfMade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleBrand = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BuildType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModelYear = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    EnginePower = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OfficeLetterNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AttachFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StandardValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ContractValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentRefID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QRCodeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DemandNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FormNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PersonalPkid = table.Column<int>(type: "int", nullable: false),
                    PersonTownshipPkid = table.Column<int>(type: "int", nullable: false),
                    VehiclePkid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TaxValidation", x => x.TaxValidationPkid);
                    table.ForeignKey(
                        name: "FK_TB_TaxValidation_TB_PersonalDetail_PersonalPkid",
                        column: x => x.PersonalPkid,
                        principalTable: "TB_PersonalDetail",
                        principalColumn: "PersonalPkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_TaxValidation_TB_Township_PersonTownshipPkid",
                        column: x => x.PersonTownshipPkid,
                        principalTable: "TB_Township",
                        principalColumn: "TownshipPkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_TaxValidation_TB_VehicleStandardValue_VehiclePkid",
                        column: x => x.VehiclePkid,
                        principalTable: "TB_VehicleStandardValue",
                        principalColumn: "VehicleStandardValuePkid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_Township_StateDivisionPkid",
                table: "TB_Township",
                column: "StateDivisionPkid");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TaxValidation_PersonalPkid",
                table: "TB_TaxValidation",
                column: "PersonalPkid");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TaxValidation_PersonTownshipPkid",
                table: "TB_TaxValidation",
                column: "PersonTownshipPkid");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TaxValidation_VehiclePkid",
                table: "TB_TaxValidation",
                column: "VehiclePkid");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_Township_TB_StateDivision_StateDivisionPkid",
                table: "TB_Township",
                column: "StateDivisionPkid",
                principalTable: "TB_StateDivision",
                principalColumn: "StateDivisionPkid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_Township_TB_StateDivision_StateDivisionPkid",
                table: "TB_Township");

            migrationBuilder.DropTable(
                name: "TB_Payment");

            migrationBuilder.DropTable(
                name: "TB_TaxValidation");

            migrationBuilder.DropIndex(
                name: "IX_TB_Township_StateDivisionPkid",
                table: "TB_Township");

            migrationBuilder.DropColumn(
                name: "StateDivisionPkid",
                table: "TB_Township");
        }
    }
}
