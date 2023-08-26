using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Common
{
    public static class QueryValidator
    {

        public static bool IsPatientNameExist(ApplicationDbContext context, string patientName, int? patientId = null)
        {
            return _isPatientNameExist(context, patientName, patientId);
        }

        public static bool IsPatientNationalNumberExist(ApplicationDbContext context, string nationalNumber, int? patientId = null)
        {
            return _isPatientNationalNumberExist(context, nationalNumber, patientId);
        }

        public static bool IsPatientGeneralNumberExist(ApplicationDbContext context, string generalNumber, int? patientId = null)
        {
            return _isPatientGeneralNumberExist(context, generalNumber, patientId);
        }

        public static bool IsPatientMilitaryNumberExist(ApplicationDbContext context, string militaryNumber, int? patientId = null)
        {
            return _isPatientMilitaryNumberExist(context, militaryNumber, patientId);
        }
          public static bool IsPatientExist(ApplicationDbContext context, int id)
        {
            return _isPatientExist(context, id);
        }

        public static bool PatientHasAlreadyActiveVisit(ApplicationDbContext context, int id)
        {
            return _isPatientHasAlreadyActiveVisit(context, id);
        }




        #region Compiled Queries
        private static readonly Func<ApplicationDbContext, string, int?,  bool> _isPatientNameExist = EF.CompileQuery<ApplicationDbContext, string,  int?, bool>( (context, patientName, patientId) =>  context.Patients.AsNoTracking().Any(x => x.Name == patientName && (!patientId.HasValue || x.Id != patientId)));

        private static readonly Func<ApplicationDbContext, string, int?, bool> _isPatientNationalNumberExist = EF.CompileQuery<ApplicationDbContext, string, int?, bool>((context, nationalNumber, patientId) => context.Patients.AsNoTracking().Any(x => x.NationalNumber == nationalNumber && (!patientId.HasValue || x.Id != patientId)));

        private static readonly Func<ApplicationDbContext, string, int?, bool> _isPatientGeneralNumberExist = EF.CompileQuery<ApplicationDbContext, string, int?, bool>((context, generalNumber, patientId) => context.Patients.AsNoTracking().Any(x => x.GeneralNumber == generalNumber && (!patientId.HasValue || x.Id != patientId)));

        private static readonly Func<ApplicationDbContext, string, int?, bool> _isPatientMilitaryNumberExist = EF.CompileQuery<ApplicationDbContext, string, int?, bool>((context, militaryNumber,  patientId) => context.Patients.AsNoTracking().Any(x => x.MilitaryNumber == militaryNumber && (!patientId.HasValue || x.Id != patientId)));

        private static readonly Func<ApplicationDbContext, int, bool> _isPatientExist = EF.CompileQuery<ApplicationDbContext, int, bool>((context, id) => context.Patients.AsNoTracking().Any(x => x.Id == id));

        private static readonly Func<ApplicationDbContext, int, bool> _isPatientHasAlreadyActiveVisit = EF.CompileQuery<ApplicationDbContext, int, bool>((context, id) => context.Visits.AsNoTracking().Any(x => x.PatientId == id && x.Status != Constants.ConstantVisitStatus.FINISHED && !x.IsDeleted));

       


        #endregion

    }
}
