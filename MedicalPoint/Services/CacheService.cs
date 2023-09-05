using MedicalPoint.Common;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface ICacheService
    {
        List<Clinic> GetClinics();
        List<Degree> GetDegrees();
        List<LookupVisitRestType> GetVisitRestTypes();
        void UpdateClinics();
        void UpdateDegrees();
        void UpdateVisitRestTypes();
    }

    public class CacheService : ICacheService
    {

        private readonly ApplicationDbContext _context;

        public CacheService(ApplicationDbContext context)
        {

            _context = context;
        }
        public List<Clinic> GetClinics()
        {
            if (CacheData.NeedClinicUpdate)
            {
                UpdateClinics();
            }
            return CacheData.Clinics;
        }
        public List<Degree> GetDegrees()
        {
            if (CacheData.NeedDegreeUpdate)
            {
                lock (CacheData.Degrees)
                {
                    CacheData.Update(QueryFinder.GetDegrees(_context));
                }
            }
            return CacheData.Degrees;
        }
        public List<LookupVisitRestType> GetVisitRestTypes()
        {
            if (CacheData.NeedClinicUpdate)
            {
                lock (CacheData.VisitRestTypes)
                {
                    CacheData.Update(QueryFinder.GetVisitRestTypes(_context));
                }
            }
            return CacheData.VisitRestTypes;
        }
        public void UpdateClinics()
        {
            lock (CacheData.Clinics)
            {
                CacheData.Update(QueryFinder.GetClinics(_context));
            }
        }
        public void UpdateDegrees()
        {
            lock (CacheData.Degrees)
            {
                CacheData.Update(QueryFinder.GetDegrees(_context));
            }
        }
        public void UpdateVisitRestTypes()
        {
            lock (CacheData.VisitRestTypes)
            {
                CacheData.Update(QueryFinder.GetVisitRestTypes(_context));
            }
        }
    }
    public class CacheData
    {
        private static List<Clinic> _clinics = new List<Clinic>();
        private static List<Degree> _degrees = new List<Degree>();
        private static List<LookupVisitRestType> _visitRestTypes = new List<LookupVisitRestType>();
        public static List<Clinic> Clinics { get { return _clinics; } }
        public static List<Degree> Degrees { get { return _degrees; } }
        public static List<LookupVisitRestType> VisitRestTypes { get { return _visitRestTypes; } }
        private static bool _needClinicUpdate = true;
        private static bool _needDegreeUpdate = true;
        private static bool _needVisitRestTypeUpdate = true;
        public static bool NeedClinicUpdate { get { return _needClinicUpdate; } }
        public static bool NeedDegreeUpdate { get { return _needDegreeUpdate; } }
        public static bool NeedVisitRestTypeUpdate { get { return _needVisitRestTypeUpdate; } }
        public static void Update( List<Degree> degrees)
        {
            
            _degrees = degrees;

            _needDegreeUpdate = false;
        }
        public static void Update(List<LookupVisitRestType> visitRestTypes)
        {
           
            _visitRestTypes = visitRestTypes;
            _needVisitRestTypeUpdate = false;
        }
        public static void Update(List<Clinic> clinics)
        {
            _clinics = clinics;

            _needClinicUpdate = false;
        }
    }
}
