using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motax.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryUnitAndServiceUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "TaxAmount",
                table: "CarRegistrations",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "RegistrationFee",
                table: "CarRegistrations",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseNumber",
                table: "CarRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId",
                table: "CarRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "CarRegistrations",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Accounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    PickupDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceUnits_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceUnits_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    ServiceUnitId = table.Column<int>(type: "int", nullable: false),
                    ScheduledPickupDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickupLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryUnits_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryUnits_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryUnits_ServiceUnits_ServiceUnitId",
                        column: x => x.ServiceUnitId,
                        principalTable: "ServiceUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CarRegistrations_OrderStatusId",
                table: "CarRegistrations",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryUnits_CarId",
                table: "DeliveryUnits",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryUnits_DealerId",
                table: "DeliveryUnits",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryUnits_ServiceUnitId",
                table: "DeliveryUnits",
                column: "ServiceUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceUnits_CarId",
                table: "ServiceUnits",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceUnits_DealerId",
                table: "ServiceUnits",
                column: "DealerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarRegistrations_OrderStatus_OrderStatusId",
                table: "CarRegistrations",
                column: "OrderStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatus_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarRegistrations_OrderStatus_OrderStatusId",
                table: "CarRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatus_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DeliveryUnits");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "ServiceUnits");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_CarRegistrations_OrderStatusId",
                table: "CarRegistrations");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DriverLicenseNumber",
                table: "CarRegistrations");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "CarRegistrations");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "CarRegistrations");

            migrationBuilder.AlterColumn<double>(
                name: "TaxAmount",
                table: "CarRegistrations",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RegistrationFee",
                table: "CarRegistrations",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
