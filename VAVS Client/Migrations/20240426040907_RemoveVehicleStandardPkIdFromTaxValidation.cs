using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAVS_Client.Migrations
{
    public partial class RemoveVehicleStandardPkIdFromTaxValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_TaxValidation_TB_VehicleStandardValue_VehiclePkid",
                table: "TB_TaxValidation");

            migrationBuilder.DropIndex(
                name: "IX_TB_TaxValidation_VehiclePkid",
                table: "TB_TaxValidation");

            migrationBuilder.DropColumn(
                name: "VehiclePkid",
                table: "TB_TaxValidation");

            

            migrationBuilder.AlterColumn<string>(
                name: "NRCFrontImagePath",
                table: "TB_PersonalDetail",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NRCBackImagePath",
                table: "TB_PersonalDetail",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<int>(
                name: "VehiclePkid",
                table: "TB_TaxValidation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TB_TaxValidation_VehiclePkid",
                table: "TB_TaxValidation",
                column: "VehiclePkid");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_TaxValidation_TB_VehicleStandardValue_VehiclePkid",
                table: "TB_TaxValidation",
                column: "VehiclePkid",
                principalTable: "TB_VehicleStandardValue",
                principalColumn: "VehicleStandardValuePkid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
