using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitingAPI.Migrations
{
    /// <inheritdoc />
    public partial class addVisitorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "statut",
                table: "Candidatures",
                newName: "status");

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    IdVisitor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.IdVisitor);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Candidatures",
                newName: "statut");
        }
    }
}
