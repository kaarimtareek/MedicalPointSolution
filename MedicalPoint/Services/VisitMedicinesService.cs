using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IVisitMedicinesService
    {
        Task<OperationResult<VisitMedicine>> Add(int userId, int visitId, int medicineId, int quantity, string? notes, CancellationToken cancellationToken = default);
        Task<OperationResult<VisitMedicine>> Edit(int userId, int visitMedicineId, int quantity, string? notes, bool forceChange = false, CancellationToken cancellationToken = default);
        Task<List<Medicine>> GetAvailableMedicinesForVisit(int visitId, CancellationToken cancellationToken = default);
        Task<List<VisitMedicine>> GetMedicinesForVisit(int visitId, CancellationToken cancellationToken = default);
        Task<OperationResult> GiveMedicines(int visitId, int userId, CancellationToken cancellationToken = default);
        Task<OperationResult<VisitMedicine>> Remove(int userId, int visitMedicineId, bool forceChange = false, CancellationToken cancellationToken = default);
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
            var result = await _context.VisitMedicines.Include(x=> x.Medicine).AsNoTracking().Where(x => x.VisitId == visitId).ToListAsync(cancellationToken);
            return result;
        }
        public async Task<List<Medicine>> GetAvailableMedicinesForVisit(int visitId, CancellationToken cancellationToken = default)
        {
            var visitMedicinesIds = await _context.VisitMedicines.Include(x=> x.Medicine).AsNoTracking().Where(x => x.VisitId == visitId).Select(x=> x.MedicineId).ToListAsync(cancellationToken);
            var availableMedicines = await _context.Medicines.AsNoTracking().Where(x => !visitMedicinesIds.Contains(x.Id) && x.Quantity > 0 && !x.IsDeleted).ToListAsync();
            return availableMedicines;
        }
        public async Task<OperationResult<VisitMedicine>> Add(int userId, int visitId, int medicineId, int quantity, string? notes, CancellationToken cancellationToken = default)
        {
            //check if the medicine is already added to the visit
            //check if the visit status allows to add new medicine or not
            var visit = await _context.Visits.FirstOrDefaultAsync(x => x.Id == visitId, cancellationToken);
            if (visit == null)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (!visit.CanEditVisit())
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if(string.IsNullOrEmpty(visit.Diagnosis))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (await _context.VisitMedicines.AnyAsync(x => x.VisitId == visitId && x.MedicineId == medicineId, cancellationToken))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            visit.DoctorId ??= userId;
            if(visit.Status == ConstantVisitStatus.IN_RECIEPTION  || visit.Status == ConstantVisitStatus.IN_CLINIC_DIAGNOSIS)
            {
                visit.Status = ConstantVisitStatus.IN_CLINIC_MEDICINES;
            }
            var visitHistory = new VisitHistory
            {
                Status = visit.Status,
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                UserId = userId,
                Type = "",
                Diagnosis = "",
                Notes = "",
                VisitNumber="",
            };
            var visitMedicine = new VisitMedicine
            {
                CreatedAt = DateTime.Now,
                Notes = notes ?? string.Empty,
                MedicineId = medicineId,
                Quantity = quantity,
                VisitId = visitId,
            };
            await _context.VisitMedicines.AddAsync(visitMedicine, cancellationToken);
            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitMedicine>.Succeeded(visitMedicine, "");
        }
        public async Task<OperationResult<VisitMedicine>> Edit(int userId, int visitMedicineId, int quantity, string? notes, bool forceChange = false, CancellationToken cancellationToken = default)
        {
            if (quantity < 1)
                return OperationResult<VisitMedicine>.Failed("");
            var visitMedicine = await _context.VisitMedicines.FirstOrDefaultAsync(x => x.Id == visitMedicineId, cancellationToken);
            if (visitMedicine == null)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            var visit = await _context.Visits.FirstOrDefaultAsync(x => x.Id == visitMedicine.VisitId, cancellationToken);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (!visit.CanEditVisit(forceChange))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            visit.DoctorId ??= userId;

            visitMedicine.Quantity = quantity;
            visitMedicine.Notes = notes ?? string.Empty;

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitMedicine>.Succeeded(visitMedicine, "");
        }

        public async Task<OperationResult<VisitMedicine>> Remove(int userId, int visitMedicineId, bool forceChange = false, CancellationToken cancellationToken = default)
        {

            var visitMedicine = await _context.VisitMedicines.FirstOrDefaultAsync(x => x.Id == visitMedicineId, cancellationToken);
            if (visitMedicine == null)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            var visit = await _context.Visits.FirstOrDefaultAsync(x => x.Id == visitMedicine.VisitId, cancellationToken);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            if (!visit.CanEditVisit(forceChange))
            {
                return OperationResult<VisitMedicine>.Failed("");
            }
            visit.DoctorId ??= userId;
            _context.VisitMedicines.Remove(visitMedicine);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitMedicine>.Succeeded(visitMedicine, "");
        }
        public async Task<OperationResult> GiveMedicines(int visitId, int userId, CancellationToken cancellationToken = default)
        {
            var visit = await _context.Visits.FirstOrDefaultAsync(x=> x.Id == visitId, cancellationToken);
            if(visit == null || visit.IsDeleted)
            {
                return OperationResult.Failed("");
            }
            var visitMedicines = await _context.VisitMedicines.Where(x=> x.VisitId == visitId ).ToListAsync(cancellationToken);
            if (visitMedicines.Count ==0 && visitMedicines.All(x=> x.IsGiven))
            {
                return OperationResult.Failed("");
            }
            var medicinesIds = visitMedicines.Select(x=> x.MedicineId).ToList();
            var medicines = await _context.Medicines.Where(x => medicinesIds.Contains(x.Id)).ToListAsync(cancellationToken);

            foreach (var visitMedicine in visitMedicines)
            {
                var medicine = medicines.FirstOrDefault(x => x.Id == visitMedicine.MedicineId);
                if (medicine == null || medicine.IsDeleted)
                {
                    return OperationResult.Failed("");
                }
                if(medicine.Quantity < visitMedicine.Quantity)
                {
                    return OperationResult.Failed("");
                }
                medicine.Quantity -= visitMedicine.Quantity;
                var medicineHistory = new MedicineHistory
                {
                    ActionType = ConstantMedicineActionType.EXPORT,
                    CreatedAt = DateTime.Now,
                    MedicineId = medicine.Id,
                    MedicineName = "",
                    MedicineQuantity = visitMedicine.Quantity,
                    UserId = userId,
                };
                await _context.AddAsync(medicineHistory, cancellationToken);
            }
            var visitHistory = new VisitHistory
            {
                UserId = userId,
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                Notes = "",
                Diagnosis = "",
                Status = "",
                VisitNumber = "",
                IsMedicinesGiven = true,
                MedicineGivenTime = DateTime.Now,
                Type = "",
            };
            visit.Status = ConstantVisitStatus.FINISHED;
            visit.IsMedicinesGiven = true;
            visit.MedicineGivenTime = DateTime.Now;
            await _context.VisitHistories.AddAsync(visitHistory);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult.Succeeded();
        }
    }
}
