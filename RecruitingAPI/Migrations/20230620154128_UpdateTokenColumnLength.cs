using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitingAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTokenColumnLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "Recruiters",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

           /* migrationBuilder.AddColumn<string>(
                name: "diploma",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "diploma",
                table: "Offers");*/

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "Recruiters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
