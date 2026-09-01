using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_ecommerce_api.Migrations
{
    /// <inheritdoc />
    public partial class PriductsAndCategoryCreateChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Products",
                type: "text",
                nullable: true);
        }
    }
}
