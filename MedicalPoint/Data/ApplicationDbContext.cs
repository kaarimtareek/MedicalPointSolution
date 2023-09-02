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
                    Name = "ملازم"
                },
                 new Degree
                {
                    Id = 3,
                    Name = "نقيب"
                },
                new Degree
                {
                    Id = 4,
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
        }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<LookupVisitRestType> LookupVisitRestTypes { get; set; }
        public DbSet<MedicalPointUser> Users { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
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