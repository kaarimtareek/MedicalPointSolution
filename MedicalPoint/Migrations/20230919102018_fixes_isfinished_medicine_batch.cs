using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class fixes_isfinished_medicine_batch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "MedicineBatches",
                type: "bit",
                nullable: false,
                computedColumnSql: "CAST(CASE Quantity  WHEN 0 THEN 1 ELSE 0 END AS bit)",
                stored: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComputedColumnSql: "CAST(CASE Quantity  WHEN 0 THEN 0 ELSE 1 END AS bit)",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "MedicineBatches",
                type: "bit",
                nullable: false,
                computedColumnSql: "CAST(CASE Quantity  WHEN 0 THEN 0 ELSE 1 END AS bit)",
                stored: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComputedColumnSql: "CAST(CASE Quantity  WHEN 0 THEN 1 ELSE 0 END AS bit)",
                oldStored: true);
        }
    }
}
