using Microsoft.EntityFrameworkCore.Migrations;

namespace OnIndustri.Data.Migrations
{
    public partial class ChangePartnerDisabledField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Disable",
                table: "Partner",
                newName: "IsDisabled");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDisabled",
                table: "Partner",
                newName: "Disable");
        }
    }
}
