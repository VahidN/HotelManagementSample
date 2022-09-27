using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.DataAccess.Migrations.ParbadData
{
    public partial class P2022_09_21_1218 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "parbad");

            migrationBuilder.CreateTable(
                name: "payment",
                schema: "parbad",
                columns: table => new
                {
                    payment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tracking_number = table.Column<long>(type: "bigint", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    transaction_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gateway_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    gateway_account_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_completed = table.Column<bool>(type: "bit", nullable: false),
                    is_paid = table.Column<bool>(type: "bit", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_on = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_id", x => x.payment_id);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                schema: "parbad",
                columns: table => new
                {
                    transaction_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    type = table.Column<byte>(type: "tinyint", nullable: false),
                    is_succeed = table.Column<bool>(type: "bit", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    additional_data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_on = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transaction_id", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_transaction_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "parbad",
                        principalTable: "payment",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payment_token",
                schema: "parbad",
                table: "payment",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_tracking_number",
                schema: "parbad",
                table: "payment",
                column: "tracking_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_PaymentId",
                schema: "parbad",
                table: "transaction",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction",
                schema: "parbad");

            migrationBuilder.DropTable(
                name: "payment",
                schema: "parbad");
        }
    }
}
