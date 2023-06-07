using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitingAPI.Migrations
{
    /// <inheritdoc />
    public partial class Rec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recruiters",
                columns: table => new
                {
                    idRec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    age = table.Column<int>(type: "int", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    idCo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiters", x => x.idRec);
                    table.ForeignKey(
                        name: "FK_Recruiters_Companies_idCo",
                        column: x => x.idCo,
                        principalTable: "Companies",
                        principalColumn: "idCo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recruiters_idCo",
                table: "Recruiters",
                column: "idCo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recruiters");
        }
    }
}
