using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "RentalLocations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "RentalLocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 1,
                column: "LocationId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "RentalLocations",
                keyColumn: "Id",
                keyValue: 2,
                column: "LocationId",
                value: 0);
        }
    }
}
