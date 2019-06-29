using Microsoft.EntityFrameworkCore.Migrations;

namespace OnIndustri.Data.Migrations
{
    public partial class UpdateSupplierContactMobilePhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModilePhone",
                table: "Contact",
                newName: "MobilePhone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MobilePhone",
                table: "Contact",
                newName: "ModilePhone");
        }
    }
}
