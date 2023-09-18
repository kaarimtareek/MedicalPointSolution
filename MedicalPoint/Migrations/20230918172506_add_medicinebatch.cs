using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class add_medicinebatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OldestExpirationDate",
                table: "Medicines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "MedicineHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicineBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false, computedColumnSql: "CAST(CASE Quantity  WHEN 0 THEN 0 ELSE 1 END AS bit)", stored: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineBatches_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineBatches_MedicineId",
                table: "MedicineBatches",
                column: "MedicineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineBatches");

            migrationBuilder.DropColumn(
                name: "OldestExpirationDate",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "MedicineHistories");
        }
    }
}
