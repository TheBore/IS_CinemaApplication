using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinema.Repository.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketInOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketInOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketInOrders_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketInShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: false),
                    ShoppingCartId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketInShoppingCarts_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketInShoppingCarts_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_OwnerId",
                table: "ShoppingCarts",
                column: "OwnerId",
                unique: true,
                filter: "[OwnerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInOrders_OrderId",
                table: "TicketInOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInOrders_TicketId",
                table: "TicketInOrders",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInShoppingCarts_ShoppingCartId",
                table: "TicketInShoppingCarts",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInShoppingCarts_TicketId",
                table: "TicketInShoppingCarts",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketInOrders");

            migrationBuilder.DropTable(
                name: "TicketInShoppingCarts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");
        }
    }
}
