using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Repository.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    SizeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrunkCapacity = table.Column<int>(type: "int", nullable: false),
                    BatteryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatteryDuration = table.Column<int>(type: "int", nullable: false),
                    RentPricePerDay = table.Column<double>(type: "float", nullable: false),
                    RentPricePerHour = table.Column<double>(type: "float", nullable: false),
                    RentPricePerDayWithDriver = table.Column<double>(type: "float", nullable: false),
                    RentPricePerHourWithDriver = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CitizenIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CitizenIdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentalOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenIds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriverLicenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentalOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverLicenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RentalLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Coordinates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarRentalLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarRentalLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarRentalLocations_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarRentalLocations_RentalLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmEmailToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetPasswordTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RentalLocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_RentalLocations_RentalLocationId",
                        column: x => x.RentalLocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RentalOrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RentalPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminationClause = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RentalOrderId = table.Column<int>(type: "int", nullable: true),
                    LesseeId = table.Column<int>(type: "int", nullable: false),
                    LessorId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalContacts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RentalOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickupTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedReturnTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualReturnTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubTotal = table.Column<double>(type: "float", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    ExtraFee = table.Column<double>(type: "float", nullable: true),
                    DamageFee = table.Column<double>(type: "float", nullable: true),
                    DamageNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WithDriver = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RentalLocationId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    RentalContactId = table.Column<int>(type: "int", nullable: true),
                    CitizenId = table.Column<int>(type: "int", nullable: true),
                    DriverLicenseId = table.Column<int>(type: "int", nullable: true),
                    PaymentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalOrders_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalOrders_CitizenIds_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "CitizenIds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_DriverLicenses_DriverLicenseId",
                        column: x => x.DriverLicenseId,
                        principalTable: "DriverLicenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_RentalContacts_RentalContactId",
                        column: x => x.RentalContactId,
                        principalTable: "RentalContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalOrders_RentalLocations_RentalLocationId",
                        column: x => x.RentalLocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalOrders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarDeliveryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerStart = table.Column<int>(type: "int", nullable: false),
                    BatteryLevelStart = table.Column<int>(type: "int", nullable: false),
                    VehicleConditionStart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarDeliveryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_RentalLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_RentalOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDeliveryHistories_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarReturnHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerEnd = table.Column<int>(type: "int", nullable: false),
                    BatteryLevelEnd = table.Column<int>(type: "int", nullable: false),
                    VehicleConditionEnd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarReturnHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_RentalLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "RentalLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_RentalOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReturnHistories_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RentalOrderId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_RentalOrders_RentalOrderId",
                        column: x => x.RentalOrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "BatteryDuration", "BatteryType", "CreatedAt", "ImageUrl", "ImageUrl2", "ImageUrl3", "IsActive", "IsDeleted", "Model", "Name", "RentPricePerDay", "RentPricePerDayWithDriver", "RentPricePerHour", "RentPricePerHourWithDriver", "Seats", "SizeType", "Status", "TrunkCapacity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 350, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/tesla_model_3.jpg", "https://example.com/tesla_model_3.jpg", "https://example.com/tesla_model_3.jpg", true, false, "Tesla Model 3", "Model 3", 1000000.0, 1400000.0, 45000.0, 60000.0, 5, "Sedan", 1, 425, null },
                    { 2, 240, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/nissan_leaf.jpg", "https://example.com/nissan_leaf.jpg", "https://example.com/nissan_leaf.jpg", true, false, "Nissan Leaf", "Leaf", 800000.0, 1200000.0, 35000.0, 50000.0, 5, "Hatchback", 1, 435, null },
                    { 3, 259, "Lithium-Ion", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/chevrolet_bolt_ev.jpg", "https://example.com/chevrolet_bolt_ev.jpg", "https://example.com/chevrolet_bolt_ev.jpg", true, false, "Chevrolet Bolt EV", "Bolt EV", 900000.0, 1300000.0, 40000.0, 55000.0, 5, "Hatchback", 1, 478, null }
                });

            migrationBuilder.InsertData(
                table: "RentalLocations",
                columns: new[] { "Id", "Address", "Coordinates", "CreatedAt", "IsActive", "IsDeleted", "LocationId", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Tran Hung Dao St, Ho Chi Minh City", "10.7769,106.7009", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Downtown Rental Location", null },
                    { 2, "456 Nguyen Cuu Phuc St, Ho Chi Minh City", "10.7950,106.6540", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, 0, "Airport Rental Location", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConfirmEmailToken", "CreatedAt", "Email", "FullName", "IsActive", "IsEmailConfirmed", "Password", "PasswordHash", "RentalLocationId", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", "Admin User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", null, null, null, "Admin", null },
                    { 3, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer@gmail.com", "Customer User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", null, null, null, "Customer", null }
                });

            migrationBuilder.InsertData(
                table: "CarRentalLocations",
                columns: new[] { "Id", "CarId", "IsDeleted", "LocationId", "Quantity" },
                values: new object[] { 1, 1, false, 1, 5 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConfirmEmailToken", "CreatedAt", "Email", "FullName", "IsActive", "IsEmailConfirmed", "Password", "PasswordHash", "RentalLocationId", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[] { 2, null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff@gmail.com", "Staff User", true, true, "1", "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm", 1, null, null, "Staff", null });

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_CarId",
                table: "CarDeliveryHistories",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_CustomerId",
                table: "CarDeliveryHistories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_LocationId",
                table: "CarDeliveryHistories",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_OrderId",
                table: "CarDeliveryHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDeliveryHistories_StaffId",
                table: "CarDeliveryHistories",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalLocations_CarId",
                table: "CarRentalLocations",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalLocations_LocationId",
                table: "CarRentalLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_CarId",
                table: "CarReturnHistories",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_CustomerId",
                table: "CarReturnHistories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_LocationId",
                table: "CarReturnHistories",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_OrderId",
                table: "CarReturnHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarReturnHistories_StaffId",
                table: "CarReturnHistories",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RentalOrderId",
                table: "Feedbacks",
                column: "RentalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalContacts_UserId",
                table: "RentalContacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_CarId",
                table: "RentalOrders",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_CitizenId",
                table: "RentalOrders",
                column: "CitizenId",
                unique: true,
                filter: "[CitizenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_DriverLicenseId",
                table: "RentalOrders",
                column: "DriverLicenseId",
                unique: true,
                filter: "[DriverLicenseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_PaymentId",
                table: "RentalOrders",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_RentalContactId",
                table: "RentalOrders",
                column: "RentalContactId",
                unique: true,
                filter: "[RentalContactId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_RentalLocationId",
                table: "RentalOrders",
                column: "RentalLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_UserId",
                table: "RentalOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RentalLocationId",
                table: "Users",
                column: "RentalLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarDeliveryHistories");

            migrationBuilder.DropTable(
                name: "CarRentalLocations");

            migrationBuilder.DropTable(
                name: "CarReturnHistories");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "RentalOrders");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "CitizenIds");

            migrationBuilder.DropTable(
                name: "DriverLicenses");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RentalContacts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RentalLocations");
        }
    }
}
