using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoLimit.Migrations
{
    public partial class VariationCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Variacao",
                table: "Produtos",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Variacao",
                table: "Produtos");
        }
    }
}
