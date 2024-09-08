using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smallShop.Migrations
{
    /// <inheritdoc />
    public partial class dtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "urlImg",
                table: "Products",
                newName: "UrlImg");

            migrationBuilder.RenameColumn(
                name: "urlImg",
                table: "Categories",
                newName: "UrlImg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlImg",
                table: "Products",
                newName: "urlImg");

            migrationBuilder.RenameColumn(
                name: "UrlImg",
                table: "Categories",
                newName: "urlImg");
        }
    }
}
