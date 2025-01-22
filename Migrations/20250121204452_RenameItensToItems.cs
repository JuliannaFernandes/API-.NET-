using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crud.Migrations
{
    /// <inheritdoc />
    public partial class RenameItensToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Itens_ItemId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Itens_Products_ProductId",
                table: "Itens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Itens",
                table: "Itens");

            migrationBuilder.RenameTable(
                name: "Itens",
                newName: "Items");

            migrationBuilder.RenameIndex(
                name: "IX_Itens_ProductId",
                table: "Items",
                newName: "IX_Items_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Items_ItemId",
                table: "Carts",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Products_ProductId",
                table: "Items",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Items_ItemId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Products_ProductId",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "Itens");

            migrationBuilder.RenameIndex(
                name: "IX_Items_ProductId",
                table: "Itens",
                newName: "IX_Itens_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Itens",
                table: "Itens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Itens_ItemId",
                table: "Carts",
                column: "ItemId",
                principalTable: "Itens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Itens_Products_ProductId",
                table: "Itens",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
