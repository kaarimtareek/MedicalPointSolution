using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Degree> Degrees { get; set; }
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