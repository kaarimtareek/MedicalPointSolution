﻿// <auto-generated />
using System;
using MedicalPoint.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MedicalPoint.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MedicalPoint.Data.Clinic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Clinics");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsActive = true,
                            Name = "جلدية"
                        },
                        new
                        {
                            Id = 2,
                            IsActive = true,
                            Name = "عظام"
                        },
                        new
                        {
                            Id = 3,
                            IsActive = true,
                            Name = "باطنة"
                        },
                        new
                        {
                            Id = 4,
                            IsActive = true,
                            Name = "أنف وأذن"
                        },
                        new
                        {
                            Id = 5,
                            IsActive = true,
                            Name = "مسالك"
                        },
                        new
                        {
                            Id = 6,
                            IsActive = true,
                            Name = "مخ وأعصاب"
                        },
                        new
                        {
                            Id = 7,
                            IsActive = true,
                            Name = "أسنان"
                        });
                });

            modelBuilder.Entity("MedicalPoint.Data.Degree", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Degrees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "طالب"
                        },
                        new
                        {
                            Id = 2,
                            Name = "جندي"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ملازم"
                        },
                        new
                        {
                            Id = 4,
                            Name = "ملازم أول"
                        },
                        new
                        {
                            Id = 5,
                            Name = "نقيب"
                        },
                        new
                        {
                            Id = 6,
                            Name = "رائد"
                        },
                        new
                        {
                            Id = 7,
                            Name = "مقدم"
                        },
                        new
                        {
                            Id = 8,
                            Name = "عقيد"
                        },
                        new
                        {
                            Id = 9,
                            Name = "عميد"
                        },
                        new
                        {
                            Id = 10,
                            Name = "لواء"
                        },
                        new
                        {
                            Id = 11,
                            Name = "عريف"
                        },
                        new
                        {
                            Id = 12,
                            Name = "رقيب"
                        },
                        new
                        {
                            Id = 13,
                            Name = "رقيب أول"
                        },
                        new
                        {
                            Id = 14,
                            Name = "مساعد"
                        },
                        new
                        {
                            Id = 15,
                            Name = "مساعد أول"
                        },
                        new
                        {
                            Id = 16,
                            Name = "مدني"
                        });
                });

            modelBuilder.Entity("MedicalPoint.Data.LookupVisitRestType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("LookupVisitRestTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsActive = false,
                            Name = "راحة عنبر"
                        },
                        new
                        {
                            Id = 2,
                            IsActive = false,
                            Name = "راحة تحت المظلة"
                        },
                        new
                        {
                            Id = 3,
                            IsActive = false,
                            Name = "حجز عيادة"
                        },
                        new
                        {
                            Id = 4,
                            IsActive = false,
                            Name = "حجز مست خارجي"
                        });
                });

            modelBuilder.Entity("MedicalPoint.Data.MedicalPointUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccoutType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("DegreeId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("MilitaryNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("DegreeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MedicalPoint.Data.Medicine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MinimumQuantityThreshold")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Medicines");
                });

            modelBuilder.Entity("MedicalPoint.Data.MedicineHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<string>("MedicineName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("MedicineQuantity")
                        .HasColumnType("int");

                    b.Property<int?>("MinimumQuantityThreshold")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MedicineId");

                    b.HasIndex("UserId");

                    b.ToTable("MedicineHistories");
                });

            modelBuilder.Entity("MedicalPoint.Data.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DegreeId")
                        .HasColumnType("int");

                    b.Property<string>("GeneralNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastVisitAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Major")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MilitaryNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NationalNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("RegisteredUserId")
                        .HasColumnType("int");

                    b.Property<string>("SaryaNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("DegreeId");

                    b.HasIndex("RegisteredUserId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("MedicalPoint.Data.UnderObservationBed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BedNumber")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<int?>("DoctorId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EnterDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int");

                    b.Property<int?>("VisitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("UnderObservationBeds");
                });

            modelBuilder.Entity("MedicalPoint.Data.UnderObservationBedHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ActionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("BedId")
                        .HasColumnType("int");

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EnterDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<int?>("VisitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BedId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId")
                        .IsUnique();

                    b.ToTable("UnderObservationBedHistories");
                });

            modelBuilder.Entity("MedicalPoint.Data.UnderObservationDepartment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AvailableBedsCount")
                        .HasColumnType("int");

                    b.Property<int>("BedsCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("UnderObservationDepartments");
                });

            modelBuilder.Entity("MedicalPoint.Data.Visit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ClinicId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<string>("Diagnosis")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("DoctorId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExitTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FollowingVisitDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasFollowingVisit")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMedicinesGiven")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("MedicineGivenTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<int?>("PreviousVisitId")
                        .HasColumnType("int");

                    b.Property<int>("RegisteredUserId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("VisitNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("VisitTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ClinicId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.HasIndex("PreviousVisitId");

                    b.HasIndex("RegisteredUserId");

                    b.ToTable("Visits");
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ClinicId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Diagnosis")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("DoctorId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExitTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FollowingVisitDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("HasFollowingVisit")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsMedicinesGiven")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("MedicineGivenTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int");

                    b.Property<int?>("PreviousVisitId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("VisitId")
                        .HasColumnType("int");

                    b.Property<string>("VisitNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("VisitTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("VisitId");

                    b.ToTable("VisitHistories");
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("VisitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VisitId");

                    b.ToTable("VisitImages");
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitMedicine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("GivenTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsGiven")
                        .HasColumnType("bit");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("VisitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MedicineId");

                    b.HasIndex("VisitId");

                    b.ToTable("VisitMedicines");
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitRest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<int>("RestDaysNumber")
                        .HasColumnType("int");

                    b.Property<int>("RestTypeId")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("VisitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RestTypeId");

                    b.ToTable("VisitRests");
                });

            modelBuilder.Entity("MedicalPoint.Data.MedicalPointUser", b =>
                {
                    b.HasOne("MedicalPoint.Data.Degree", "Degree")
                        .WithMany()
                        .HasForeignKey("DegreeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Degree");
                });

            modelBuilder.Entity("MedicalPoint.Data.MedicineHistory", b =>
                {
                    b.HasOne("MedicalPoint.Data.Medicine", "Medicine")
                        .WithMany("History")
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedicalPoint.Data.MedicalPointUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medicine");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MedicalPoint.Data.Patient", b =>
                {
                    b.HasOne("MedicalPoint.Data.Degree", "Degree")
                        .WithMany()
                        .HasForeignKey("DegreeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedicalPoint.Data.MedicalPointUser", "RegisteredUser")
                        .WithMany()
                        .HasForeignKey("RegisteredUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Degree");

                    b.Navigation("RegisteredUser");
                });

            modelBuilder.Entity("MedicalPoint.Data.UnderObservationBed", b =>
                {
                    b.HasOne("MedicalPoint.Data.UnderObservationDepartment", "Department")
                        .WithMany("Beds")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedicalPoint.Data.MedicalPointUser", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId");

                    b.HasOne("MedicalPoint.Data.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId");

                    b.Navigation("Department");

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("MedicalPoint.Data.UnderObservationBedHistory", b =>
                {
                    b.HasOne("MedicalPoint.Data.UnderObservationBed", "Bed")
                        .WithMany("History")
                        .HasForeignKey("BedId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MedicalPoint.Data.MedicalPointUser", "Doctor")
                        .WithMany("UnderObservationBedHistories")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MedicalPoint.Data.Patient", "Patient")
                        .WithOne()
                        .HasForeignKey("MedicalPoint.Data.UnderObservationBedHistory", "PatientId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Bed");

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("MedicalPoint.Data.Visit", b =>
                {
                    b.HasOne("MedicalPoint.Data.Clinic", "Clinic")
                        .WithMany()
                        .HasForeignKey("ClinicId");

                    b.HasOne("MedicalPoint.Data.MedicalPointUser", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId");

                    b.HasOne("MedicalPoint.Data.Patient", "Patient")
                        .WithMany("Visits")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedicalPoint.Data.Visit", "PreviousVisit")
                        .WithMany("FollowingVisits")
                        .HasForeignKey("PreviousVisitId");

                    b.HasOne("MedicalPoint.Data.MedicalPointUser", "RegisteredUser")
                        .WithMany()
                        .HasForeignKey("RegisteredUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Clinic");

                    b.Navigation("Doctor");

                    b.Navigation("Patient");

                    b.Navigation("PreviousVisit");

                    b.Navigation("RegisteredUser");
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitHistory", b =>
                {
                    b.HasOne("MedicalPoint.Data.Visit", null)
                        .WithMany("History")
                        .HasForeignKey("VisitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitImage", b =>
                {
                    b.HasOne("MedicalPoint.Data.Visit", "Visit")
                        .WithMany("Images")
                        .HasForeignKey("VisitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Visit");
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitMedicine", b =>
                {
                    b.HasOne("MedicalPoint.Data.Medicine", "Medicine")
                        .WithMany()
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedicalPoint.Data.Visit", "Visit")
                        .WithMany("Medicines")
                        .HasForeignKey("VisitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medicine");

                    b.Navigation("Visit");
                });

            modelBuilder.Entity("MedicalPoint.Data.VisitRest", b =>
                {
                    b.HasOne("MedicalPoint.Data.LookupVisitRestType", "RestType")
                        .WithMany()
                        .HasForeignKey("RestTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RestType");
                });

            modelBuilder.Entity("MedicalPoint.Data.MedicalPointUser", b =>
                {
                    b.Navigation("UnderObservationBedHistories");
                });

            modelBuilder.Entity("MedicalPoint.Data.Medicine", b =>
                {
                    b.Navigation("History");
                });

            modelBuilder.Entity("MedicalPoint.Data.Patient", b =>
                {
                    b.Navigation("Visits");
                });

            modelBuilder.Entity("MedicalPoint.Data.UnderObservationBed", b =>
                {
                    b.Navigation("History");
                });

            modelBuilder.Entity("MedicalPoint.Data.UnderObservationDepartment", b =>
                {
                    b.Navigation("Beds");
                });

            modelBuilder.Entity("MedicalPoint.Data.Visit", b =>
                {
                    b.Navigation("FollowingVisits");

                    b.Navigation("History");

                    b.Navigation("Images");

                    b.Navigation("Medicines");
                });
#pragma warning restore 612, 618
        }
    }
}
