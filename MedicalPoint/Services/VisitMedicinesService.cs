﻿using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IVisitMedicinesService
    {
        Task<OperationResult<VisitMedicine>> Add(int visitId, int medicineId, int quantity, string? notes, CancellationToken cancellationToken = default);
        Task<OperationResult<VisitMedicine>> Edit(int visitMedicineId, int quantity, string? notes, bool forceChange = false, CancellationToken cancellationToken = default);
        Task<List<VisitMedicine>> GetMedicinesForVisit(int visitId, CancellationToken cancellationToken = default);
        Task<OperationResult<VisitMedicine>> Remove(int visitMedicineId, bool forceChange = false, CancellationToken cancellationToken = default);
    }

    public class VisitMedicinesService : IVisitMedicinesService
    {
        private readonly ApplicationDbContext _context;

        public VisitMedicinesService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<VisitMedicine>> GetMedicinesForVisit(int visitId, CancellationToken cancellationToken = default)
        {
            var result = await _context.VisitMedicines.AsNoTracking().Where(x => x.VisitId == visitId).ToListAsync(cancellationToken);
            return result;
        }
        public async Task<OperationResult<VisitMedicine>> Add(int visitId, int medicineId, int quantity, string? notes, CancellationToken cancellationToken = default)
        {
            //check if the medicine is already added to the visit
            //check if the visit status allows to add new medicine or not
            var visit = await _context.Visits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == visitId, cancellationToken);
            if (visit == null)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (visit.Status == ConstantVisitStatus.FINISHED)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (await _context.VisitMedicines.AnyAsync(x => x.VisitId == visitId && x.MedicineId == medicineId, cancellationToken))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            var visitMedicine = new VisitMedicine
            {
                CreatedAt = DateTime.Now,
                Notes = notes ?? string.Empty,
                MedicineId = medicineId,
                Quantity = quantity,
                VisitId = visitId,
            };
            await _context.VisitMedicines.AddAsync(visitMedicine, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitMedicine>.Succeeded(visitMedicine, "");
        }
        public async Task<OperationResult<VisitMedicine>> Edit(int visitMedicineId, int quantity, string? notes, bool forceChange = false, CancellationToken cancellationToken = default)
        {
            if (quantity < 1)
                return OperationResult<VisitMedicine>.Failed("");
            var visitMedicine = await _context.VisitMedicines.FirstOrDefaultAsync(x => x.Id == visitMedicineId, cancellationToken);
            if (visitMedicine == null)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            var visit = await _context.Visits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == visitMedicine.VisitId, cancellationToken);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (!visit.CanEditVisit(forceChange))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            visitMedicine.Quantity = quantity;
            visitMedicine.Notes = notes ?? string.Empty;

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitMedicine>.Succeeded(visitMedicine, "");
        }

        public async Task<OperationResult<VisitMedicine>> Remove(int visitMedicineId, bool forceChange = false, CancellationToken cancellationToken = default)
        {

            var visitMedicine = await _context.VisitMedicines.FirstOrDefaultAsync(x => x.Id == visitMedicineId, cancellationToken);
            if (visitMedicine == null)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            var visit = await _context.Visits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == visitMedicine.VisitId, cancellationToken);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (!visit.CanEditVisit(forceChange))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            _context.VisitMedicines.Remove(visitMedicine);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitMedicine>.Succeeded(visitMedicine, "");
        }

    }
}
