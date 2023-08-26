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

        private static readonly Func<ApplicationDbContext, int, Patient> _getPatientById = EF.CompileQuery<ApplicationDbContext, int, Patient>((context, id) => context.Patients.AsNoTracking().FirstOrDefault(x => x.Id == id));

        private static readonly Func<ApplicationDbContext, DateTime, int> _getVisitsCountForDay = EF.CompileQuery<ApplicationDbContext, DateTime, int>((context, date) => context.Visits.AsNoTracking().Count(x => x.VisitTime.Date == date.Date));

        

    }
}
