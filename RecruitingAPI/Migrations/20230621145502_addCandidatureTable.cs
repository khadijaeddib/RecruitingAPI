using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitingAPI.Migrations
{
    /// <inheritdoc />
    public partial class addCandidatureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidatures",
                columns: table => new
                {
                    idCandidature = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dateCand = table.Column<DateTime>(type: "datetime2", nullable: false),
                    motivation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    idCand = table.Column<int>(type: "int", nullable: false),
                    idOffer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatures", x => x.idCandidature);
                    table.ForeignKey(
                        name: "FK_Candidatures_Candidates_idCand",
                        column: x => x.idCand,
                        principalTable: "Candidates",
                        principalColumn: "idCand",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidatures_Offers_idOffer",
                        column: x => x.idOffer,
                        principalTable: "Offers",
                        principalColumn: "idOffer",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_idCand",
                table: "Candidatures",
                column: "idCand");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_idOffer",
                table: "Candidatures",
                column: "idOffer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidatures");
        }
    }
}
