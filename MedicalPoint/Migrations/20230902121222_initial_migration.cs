using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Degrees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degrees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LookupVisitRestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupVisitRestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MinimumQuantityThreshold = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnderObservationDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BedsCount = table.Column<int>(type: "int", nullable: false),
                    AvailableBedsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderObservationDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccoutType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DegreeId = table.Column<int>(type: "int", nullable: false),
                    MilitaryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Degrees_DegreeId",
                        column: x => x.DegreeId,
                        principalTable: "Degrees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisitRests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestTypeId = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    VisitId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RestDaysNumber = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitRests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitRests_LookupVisitRestTypes_RestTypeId",
                        column: x => x.RestTypeId,
                        principalTable: "LookupVisitRestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicineHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MedicineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MedicineQuantity = table.Column<int>(type: "int", nullable: true),
                    MinimumQuantityThreshold = table.Column<int>(type: "int", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineHistories_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MilitaryNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DegreeId = table.Column<int>(type: "int", nullable: false),
                    SaryaNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GeneralNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Major = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegisteredUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastVisitAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Degrees_DegreeId",
                        column: x => x.DegreeId,
                        principalTable: "Degrees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patients_Users_RegisteredUserId",
                        column: x => x.RegisteredUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnderObservationBeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    VisitId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    BedNumber = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EnterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderObservationBeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnderObservationBeds_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnderObservationBeds_UnderObservationDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "UnderObservationDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnderObservationBeds_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClinicId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    RegisteredUserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HasFollowingVisit = table.Column<bool>(type: "bit", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    VisitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PreviousVisitId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false),
                    VisitTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FollowingVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMedicinesGiven = table.Column<bool>(type: "bit", nullable: false),
                    MedicineGivenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visits_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Visits_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visits_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Visits_Users_RegisteredUserId",
                        column: x => x.RegisteredUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Visits_Visits_PreviousVisitId",
                        column: x => x.PreviousVisitId,
                        principalTable: "Visits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UnderObservationBedHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BedId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    VisitId = table.Column<int>(type: "int", nullable: true),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnterDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderObservationBedHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnderObservationBedHistories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnderObservationBedHistories_UnderObservationBeds_BedId",
                        column: x => x.BedId,
                        principalTable: "UnderObservationBeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnderObservationBedHistories_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisitHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VisitId = table.Column<int>(type: "int", nullable: false),
                    ClinicId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HasFollowingVisit = table.Column<bool>(type: "bit", nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    VisitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreviousVisitId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMedicinesGiven = table.Column<bool>(type: "bit", nullable: true),
                    MedicineGivenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FollowingVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitHistories_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitImages_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitMedicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsGiven = table.Column<bool>(type: "bit", nullable: false),
                    GivenTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitMedicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitMedicines_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitMedicines_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "جلدية" },
                    { 2, true, "عظام" },
                    { 3, true, "باطنة" },
                    { 4, true, "أنف وأذن" },
                    { 5, true, "مسالك" },
                    { 6, true, "مخ وأعصاب" },
                    { 7, true, "أسنان" }
                });

            migrationBuilder.InsertData(
                table: "Degrees",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "طالب" },
                    { 2, "جندي" },
                    { 3, "ملازم" },
                    { 4, "ملازم أول" },
                    { 5, "نقيب" },
                    { 6, "رائد" },
                    { 7, "مقدم" },
                    { 8, "عقيد" },
                    { 9, "عميد" },
                    { 10, "لواء" },
                    { 11, "عريف" },
                    { 12, "رقيب" },
                    { 13, "رقيب أول" },
                    { 14, "مساعد" },
                    { 15, "مساعد أول" },
                    { 16, "مدني" }
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

            migrationBuilder.CreateIndex(
                name: "IX_MedicineHistories_MedicineId",
                table: "MedicineHistories",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineHistories_UserId",
                table: "MedicineHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DegreeId",
                table: "Patients",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_RegisteredUserId",
                table: "Patients",
                column: "RegisteredUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBedHistories_BedId",
                table: "UnderObservationBedHistories",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBedHistories_DoctorId",
                table: "UnderObservationBedHistories",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBedHistories_PatientId",
                table: "UnderObservationBedHistories",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBeds_DepartmentId",
                table: "UnderObservationBeds",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBeds_DoctorId",
                table: "UnderObservationBeds",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBeds_PatientId",
                table: "UnderObservationBeds",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DegreeId",
                table: "Users",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitHistories_VisitId",
                table: "VisitHistories",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitImages_VisitId",
                table: "VisitImages",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitMedicines_MedicineId",
                table: "VisitMedicines",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitMedicines_VisitId",
                table: "VisitMedicines",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRests_RestTypeId",
                table: "VisitRests",
                column: "RestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ClinicId",
                table: "Visits",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorId",
                table: "Visits",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PatientId",
                table: "Visits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PreviousVisitId",
                table: "Visits",
                column: "PreviousVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_RegisteredUserId",
                table: "Visits",
                column: "RegisteredUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineHistories");

            migrationBuilder.DropTable(
                name: "UnderObservationBedHistories");

            migrationBuilder.DropTable(
                name: "VisitHistories");

            migrationBuilder.DropTable(
                name: "VisitImages");

            migrationBuilder.DropTable(
                name: "VisitMedicines");

            migrationBuilder.DropTable(
                name: "VisitRests");

            migrationBuilder.DropTable(
                name: "UnderObservationBeds");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "LookupVisitRestTypes");

            migrationBuilder.DropTable(
                name: "UnderObservationDepartments");

            migrationBuilder.DropTable(
                name: "Clinics");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Degrees");
        }
    }
}
