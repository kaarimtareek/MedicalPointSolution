using Microsoft.EntityFrameworkCore;
using MedicalPoint.ViewModels.Users;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Medicines;

namespace MedicalPoint.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MedicalPointUser>().HasOne(x=> x.Degree).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Visit>().HasOne(x => x.RegisteredUser).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UnderObservationBedHistory>().HasOne(x => x.Doctor).WithMany(x=> x.UnderObservationBedHistories).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UnderObservationBedHistory>().HasOne(x => x.Bed).WithMany(x=> x.History).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MedicalPointUser>().HasMany(x => x.UnderObservationBedHistories).WithOne(x=>x.Doctor).OnDelete(DeleteBehavior.NoAction);
            

            #region DATA SEEDING
            List<Clinic> clinics = new List<Clinic>
            {
                new Clinic
                {
                    Id = 1,
                    IsActive = true,
                    Name = "جلدية"
                },
                new Clinic
                {
                    Id = 2,
                    IsActive = true,
                    Name = "عظام"
                },
                new Clinic
                {
                    Id = 3,
                    IsActive = true,
                    Name = "باطنة"
                },
                new Clinic
                {
                    Id =4 ,
                    IsActive = true,
                    Name = "أنف وأذن"
                },
                new Clinic
                {
                    Id =5 ,
                    IsActive = true,
                    Name = "مسالك"
                },
                new Clinic
                {
                    Id = 6 ,
                    IsActive = true,
                    Name = "مخ وأعصاب"
                },
                new Clinic
                {
                    Id = 7 ,
                    IsActive = true,
                    Name = "أسنان"
                }

            };
            modelBuilder.Entity<Clinic>().HasData(clinics);

            List<Degree> degrees = new List<Degree>
            {
                new Degree
                {
                    Id = 1,
                    Name = "طالب"
                },
                new Degree
                {
                    Id = 2,
                    Name = "جندي"
                },
                new Degree
                {
                    Id = 3,
                    Name = "ملازم"
                },
                
                new Degree
                {
                    Id = 4,
                    Name = "ملازم أول"
                },
                 new Degree
                {
                    Id = 5,
                    Name = "نقيب"
                },
                 new Degree
                {
                    Id = 6,
                    Name = "رائد"
                },
                new Degree
                {
                    Id = 7,
                    Name = "مقدم"
                },
                new Degree
                {
                    Id = 8,
                    Name = "عقيد"
                },
                new Degree
                {
                    Id = 9,
                    Name = "عميد"
                },
                new Degree
                {
                    Id =10,
                    Name = "لواء"
                },
                new Degree
                {
                    Id = 11,
                    Name = "عريف"
                },
                new Degree
                {
                    Id = 12,
                    Name = "رقيب"
                },
                new Degree
                {
                    Id = 13,
                    Name = "رقيب أول"
                },
                new Degree
                {
                    Id = 14,
                    Name = "مساعد"
                },
                new Degree
                {
                    Id = 15,
                    Name = "مساعد أول"
                },
                new Degree
                {
                    Id = 16,
                    Name = "مدني"
                },
            };
            modelBuilder.Entity<Degree>().HasData(degrees);

            List<LookupVisitRestType> visitRestTypes = new List<LookupVisitRestType>
            {
                new LookupVisitRestType
                {
                    Id = 1,
                    Name = "راحة عنبر"
                },
                new LookupVisitRestType
                {
                    Id = 2,
                    Name = "راحة تحت المظلة"
                },
                 new LookupVisitRestType
                {
                    Id = 3,
                    Name = "حجز عيادة"
                },
                new LookupVisitRestType
                {
                    Id = 4,
                    Name = "حجز مست خارجي"
                },
            };
            modelBuilder.Entity<LookupVisitRestType>().HasData(visitRestTypes);
            #endregion

            modelBuilder.Entity<MedicineBatch>().Property(x => x.IsFinished).HasComputedColumnSql($"CAST(CASE {nameof(MedicineBatch.Quantity)}  WHEN 0 THEN 1 ELSE 0 END AS bit)", true);
            modelBuilder.Entity<MedicineBatch>().Property(x => x.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Medicine>().Property(x => x.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MedicineHistory>().Property(x => x.Price).HasColumnType("decimal(18,2)");
        }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<LookupVisitRestType> LookupVisitRestTypes { get; set; }
        public DbSet<MedicalPointUser> Users { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineBatch> MedicineBatches { get; set; }
        public DbSet<MedicineHistory> MedicineHistories { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<UnderObservationBed> UnderObservationBeds { get; set; }
        public DbSet<UnderObservationBedHistory> UnderObservationBedHistories { get; set; }
        public DbSet<UnderObservationDepartment> UnderObservationDepartments { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitHistory> VisitHistories { get; set; }
        public DbSet<VisitImage> VisitImages { get; set; }
        public DbSet<VisitMedicine> VisitMedicines { get; set; }
        public DbSet<VisitRest> VisitRests { get; set; }

    }
}