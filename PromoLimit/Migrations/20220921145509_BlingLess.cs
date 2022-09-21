using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoLimit.Migrations
{
    public partial class BlingLess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paridades");

            migrationBuilder.RenameColumn(
                name: "CodigoBling",
                table: "Produtos",
                newName: "MLB");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MLB",
                table: "Produtos",
                newName: "CodigoBling");

            migrationBuilder.CreateTable(
                name: "Paridades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false),
                    MLB = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paridades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paridades_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paridades_ProdutoId",
                table: "Paridades",
                column: "ProdutoId");
        }
    }
}
