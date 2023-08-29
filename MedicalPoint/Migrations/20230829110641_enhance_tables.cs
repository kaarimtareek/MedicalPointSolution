using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class enhance_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "VisitImages",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "RegisteredUserId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VisitHistories_VisitId",
                table: "VisitHistories",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBeds_PatientId",
                table: "UnderObservationBeds",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_RegisteredUserId",
                table: "Patients",
                column: "RegisteredUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_RegisteredUserId",
                table: "Patients",
                column: "RegisteredUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnderObservationBeds_Patients_PatientId",
                table: "UnderObservationBeds",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitHistories_Visits_VisitId",
                table: "VisitHistories",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_RegisteredUserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_UnderObservationBeds_Patients_PatientId",
                table: "UnderObservationBeds");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitHistories_Visits_VisitId",
                table: "VisitHistories");

            migrationBuilder.DropIndex(
                name: "IX_VisitHistories_VisitId",
                table: "VisitHistories");

            migrationBuilder.DropIndex(
                name: "IX_UnderObservationBeds_PatientId",
                table: "UnderObservationBeds");

            migrationBuilder.DropIndex(
                name: "IX_Patients_RegisteredUserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "VisitImages");

            migrationBuilder.DropColumn(
                name: "RegisteredUserId",
                table: "Patients");
        }
    }
}
