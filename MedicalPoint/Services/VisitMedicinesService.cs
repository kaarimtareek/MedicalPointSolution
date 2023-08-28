using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public class VisitMedicinesService
    {
        private readonly ApplicationDbContext _context;

        public VisitMedicinesService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<VisitMedicine>> Add(int visitId, int medicineId, int quantity, string? notes, CancellationToken cancellationToken =  default)
        {
            //check if the medicine is already added to the visit
            //check if the visit status allows to add new medicine or not
            var visit = await _context.Visits.AsNoTracking().FirstOrDefaultAsync(x=> x.Id == visitId, cancellationToken);
            if(visit == null) 
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if(visit.Status == ConstantVisitStatus.FINISHED)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if(await _context.VisitMedicines.AnyAsync(x => x.VisitId == visitId && x.MedicineId == medicineId, cancellationToken))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            var visitMedicine = new VisitMedicine
            {
                CreatedAt = DateTime.Now,
                Notes = notes?? string.Empty,
                MedicineId = medicineId,
                Quantity = quantity,
                VisitId = visitId,
            };
            await _context.VisitMedicines.AddAsync(visitMedicine, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitMedicine>.Succeeded(visitMedicine,"");
        }
    }
}
