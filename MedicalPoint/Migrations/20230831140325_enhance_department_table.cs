using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class enhance_department_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBeds_DoctorId",
                table: "UnderObservationBeds",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnderObservationBeds_Users_DoctorId",
                table: "UnderObservationBeds",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnderObservationBeds_Users_DoctorId",
                table: "UnderObservationBeds");

            migrationBuilder.DropIndex(
                name: "IX_UnderObservationBeds_DoctorId",
                table: "UnderObservationBeds");
        }
    }
}
