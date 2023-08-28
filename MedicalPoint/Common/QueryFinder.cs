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
        public static int GetVisitsCountForDay(ApplicationDbContext context, DateTime? day = null)
        {
            return _getVisitsCountForDay(context, day.HasValue ? day.Value.Date : DateTime.Today);
        }
        public static async Task<List<Degree>> GetDegrees(ApplicationDbContext context, bool activeOnly = true)
        {
            return await _getDegrees(context, activeOnly);
        }
        public static async Task<List<Clinic>> GetClinics(ApplicationDbContext context, bool activeOnly = true)
        {
            return await _getClinics(context, activeOnly);
        }

        private static readonly Func<ApplicationDbContext, int, Patient> _getPatientById = EF.CompileQuery<ApplicationDbContext, int, Patient>((context, id) => context.Patients.AsNoTracking().FirstOrDefault(x => x.Id == id));

        private static readonly Func<ApplicationDbContext, DateTime, int> _getVisitsCountForDay = EF.CompileQuery<ApplicationDbContext, DateTime, int>((context, date) => context.Visits.AsNoTracking().Count(x => x.VisitTime.Date == date.Date));

        private static readonly Func<ApplicationDbContext, bool, Task<List<Degree>>> _getDegrees = EF.CompileAsyncQuery<ApplicationDbContext, bool, List<Degree>>(
            (context, activeOnly) => context.Degrees.AsNoTracking().ToList());

        private static readonly Func<ApplicationDbContext, bool, Task<List<Clinic>>> _getClinics = EF.CompileAsyncQuery<ApplicationDbContext, bool, List<Clinic>>(
            (context, activeOnly) => context.Clinics.AsNoTracking().Where(x=> !activeOnly ||  x.IsActive).ToList());



    }
}
