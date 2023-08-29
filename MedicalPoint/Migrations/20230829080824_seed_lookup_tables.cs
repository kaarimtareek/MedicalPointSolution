using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class seed_lookup_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "جلدية" },
                    { 2, true, "عظام" }
                });

            migrationBuilder.InsertData(
                table: "Degrees",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "طالب" },
                    { 2, "ملازم" },
                    { 3, "نقيب" }
                });

            migrationBuilder.InsertData(
                table: "LookupVisitRestTypes",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, null, false, "راحة عنبر" },
                    { 2, null, false, "راحة تحت المظلة" },
                    { 3, null, false, "حجز عيادة" },
                    { 4, null, false, "حجز مست خارجي" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Degrees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Degrees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Degrees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LookupVisitRestTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LookupVisitRestTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LookupVisitRestTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LookupVisitRestTypes",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
