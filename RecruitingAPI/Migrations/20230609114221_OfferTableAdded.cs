using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitingAPI.Migrations
{
    /// <inheritdoc />
    public partial class OfferTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    idOffer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    studyDegree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    businessSector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expYears = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contractType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    availability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hiredNum = table.Column<int>(type: "int", nullable: false),
                    salary = table.Column<float>(type: "real", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    skills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    missions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pubDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    idRec = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.idOffer);
                    table.ForeignKey(
                        name: "FK_Offers_Recruiters_idRec",
                        column: x => x.idRec,
                        principalTable: "Recruiters",
                        principalColumn: "idRec",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_idRec",
                table: "Offers",
                column: "idRec");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");
        }
    }
}
