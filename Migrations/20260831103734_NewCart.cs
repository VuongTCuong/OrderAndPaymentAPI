using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderAndPaymentAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Cart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Cart",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
