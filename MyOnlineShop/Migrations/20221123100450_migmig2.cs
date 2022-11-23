using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyOnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class migmig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_Products_ProductId",
                table: "cart");

            migrationBuilder.DropIndex(
                name: "IX_cart_ProductId",
                table: "cart");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductId",
                table: "Products",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_cart_ProductId",
                table: "Products",
                column: "ProductId",
                principalTable: "cart",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_cart_ProductId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_cart_ProductId",
                table: "cart",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_Products_ProductId",
                table: "cart",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
