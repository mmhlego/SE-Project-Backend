using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyOnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_customer_CustomerId",
                table: "cart");

            migrationBuilder.DropIndex(
                name: "IX_cart_CustomerId",
                table: "cart");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "cart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "cart",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_cart_CustomerId",
                table: "cart",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_customer_CustomerId",
                table: "cart",
                column: "CustomerId",
                principalTable: "customer",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
