using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogInDetails",
                columns: table => new
                {
                    LoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogInDetails", x => x.LoginId);
                    table.ForeignKey(
                        name: "FK_LogInDetails_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_LogInDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReasonToChange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_History_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "CompanyId", "CompanyName" },
                values: new object[,]
                {
                    { 1, "Paintball" },
                    { 2, "Bowling" },
                    { 3, "Go Cart" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "CustomerEmail", "CustomerName", "CustomerPhoneNumber" },
                values: new object[,]
                {
                    { 1, "Hannes@test.se", "Hannes Dahlberg", "123456" },
                    { 2, "Börje@test.se", "Börje Svensson", "12356887" },
                    { 3, "Twei@test.se", "Twei Twot", "456622345" }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "AppointmentId", "AppointmentTime", "CompanyId", "CustomerId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 5, 23, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(418), 1, 1 },
                    { 2, new DateTime(2024, 6, 3, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(473), 2, 2 },
                    { 3, new DateTime(2024, 5, 25, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(476), 3, 3 },
                    { 4, new DateTime(2024, 6, 6, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(478), 1, 2 },
                    { 5, new DateTime(2024, 5, 27, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(489), 2, 3 },
                    { 6, new DateTime(2024, 5, 28, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(491), 3, 1 },
                    { 7, new DateTime(2024, 6, 3, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(493), 1, 3 },
                    { 8, new DateTime(2024, 5, 30, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(496), 2, 1 },
                    { 9, new DateTime(2024, 5, 31, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(498), 3, 2 },
                    { 10, new DateTime(2024, 6, 8, 14, 22, 17, 566, DateTimeKind.Local).AddTicks(503), 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "LogInDetails",
                columns: new[] { "LoginId", "CompanyId", "CustomerId", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, null, 1, "Password1", "User", "User1" },
                    { 2, null, 2, "Password2", "User", "User2" },
                    { 3, null, 3, "Password3", "User", "User3" },
                    { 4, 1, null, "Comp1", "Company", "Company1" },
                    { 5, 2, null, "Comp2", "Company", "Company2" },
                    { 6, 3, null, "Comp3", "Company", "Company3" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CompanyId",
                table: "Appointments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerId",
                table: "Appointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_History_AppointmentId",
                table: "History",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LogInDetails_CompanyId",
                table: "LogInDetails",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LogInDetails_CustomerId",
                table: "LogInDetails",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "LogInDetails");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
