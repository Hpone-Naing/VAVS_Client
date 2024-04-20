using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAVS_Client.Migrations
{
    public partial class addnrcfrontandnrcbackimagepathcolumninpersonaldetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NRCBackImagePath",
                table: "TB_PersonalDetail",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NRCFrontImagePath",
                table: "TB_PersonalDetail",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NRCBackImagePath",
                table: "TB_PersonalDetail");

            migrationBuilder.DropColumn(
                name: "NRCFrontImagePath",
                table: "TB_PersonalDetail");
        }
    }
}
