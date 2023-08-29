using MedicalPoint.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Common
{
    public static class QueryFinder
    {
        public static Patient GetPatientById(ApplicationDbContext context, int id)
        {
            return _getPatientById(context, id);
        }
        public static Visit GetVisitById(ApplicationDbContext context, int id)
        {
            return _getVisitById(context, id);
        }
        public static VisitImage GetImageVisitById(ApplicationDbContext context, int id)
        {
            return _getVisitImageById(context, id);
        }
        public static int GetVisitsCountForDay(ApplicationDbContext context, DateTime? day = null)
        {
            return _getVisitsCountForDay(context, day.HasValue ? day.Value.Date : DateTime.Today);
        }
        public static  List<Degree> GetDegrees(ApplicationDbContext context, bool activeOnly = true)
        {
            return  _getDegrees(context, activeOnly);
        }
        public static List<Clinic> GetClinics(ApplicationDbContext context, bool activeOnly = true)
        {
            return  _getClinics(context, activeOnly);
        }

        private static readonly Func<ApplicationDbContext, int, Patient> _getPatientById = EF.CompileQuery<ApplicationDbContext, int, Patient>((context, id) => context.Patients.AsNoTracking().FirstOrDefault(x => x.Id == id));

        private static readonly Func<ApplicationDbContext, int, Visit> _getVisitById = EF.CompileQuery<ApplicationDbContext, int, Visit>((context, id) => context.Visits.AsNoTracking().FirstOrDefault(x => x.Id == id));
         private static readonly Func<ApplicationDbContext, int, VisitImage> _getVisitImageById = EF.CompileQuery<ApplicationDbContext, int, VisitImage>((context, id) => context.VisitImages.AsNoTracking().FirstOrDefault(x => x.Id == id && !x.IsDeleted));


        private static readonly Func<ApplicationDbContext, DateTime, int> _getVisitsCountForDay = EF.CompileQuery<ApplicationDbContext, DateTime, int>((context, date) => context.Visits.AsNoTracking().Count(x => x.VisitTime.Date == date.Date));

        private static readonly Func<ApplicationDbContext, bool, List<Degree>> _getDegrees = EF.CompileQuery<ApplicationDbContext, bool, List<Degree>>(
            (context, activeOnly) => context.Degrees.AsNoTracking().ToList());

        private static readonly Func<ApplicationDbContext, bool, List<Clinic>> _getClinics = EF.CompileQuery<ApplicationDbContext, bool, List<Clinic>>(
            (context, activeOnly) => context.Clinics.AsNoTracking().Where(x=> !activeOnly ||  x.IsActive).ToList());



    }
}
