﻿using MedicalPoint.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Common
{
    public static class QueryFinder
    {
        public static Patient GetPatientById(ApplicationDbContext context, int id)
        {
            return _getPatientById(context, id);
        }

        private static readonly Func<ApplicationDbContext, int, Patient> _getPatientById = EF.CompileQuery<ApplicationDbContext, int, Patient>((context, id) => context.Patients.AsNoTracking().FirstOrDefault(x => x.Id == id));


    }
}
