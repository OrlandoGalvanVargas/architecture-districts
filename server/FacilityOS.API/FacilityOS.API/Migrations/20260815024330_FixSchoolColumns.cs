using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityOS.API.Migrations
{
    /// <inheritdoc />
    public partial class FixSchoolColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Schools",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "StudenCapacity",
                table: "Schools",
                newName: "StudentCapacity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Schools",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "StudentCapacity",
                table: "Schools",
                newName: "StudenCapacity");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Role", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@livefree.com", "Admin user", "$2a$11$mC3I0b.rD21E1NfFfKxWeO7B76MhW6o7wsh.D7M6G59RBy5H67.2i", "Admin", null });
        }
    }
}
