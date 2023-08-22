using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public class MedicinesService
    {
        private readonly ApplicationDbContext _context;

        public MedicinesService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Medicine>> GetMedicines(string name = "", int? quantityLessThan = null, bool medicinesAboutToFinish = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Medicines.AsNoTracking().AsQueryable();
            if(!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if(quantityLessThan != null)
            {
                query = query.Where(x=> x.Quantity <  quantityLessThan);
            }
            if(medicinesAboutToFinish)
            {
                query = query.Where(x=> x.MinimumQuantityThreshold.HasValue && x.MinimumQuantityThreshold.Value >= x.Quantity);
            }
            var result = await query.ToListAsync(cancellationToken);
            return result;
        }

        public async Task<OperationResult<Medicine>> Add(int userId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default)
        {
            //TODO: CHECK IF name already exist
            var medicine = new Medicine
            {
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,
                MinimumQuantityThreshold = quantityThreshold,
                Name = name,
                Quantity = quantity,
                IsDeleted = false,
            };
            var medicineHistory = new MedicineHistory
            {
                Medicine = medicine,
                MedicineName = name,
                MedicineQuantity = quantity,
                MinimumQuantityThreshold = quantityThreshold,
                UserId = userId,
                ActionType = ConstantMedicineActionType.ADD,
            };
            await _context.Medicines.AddAsync(medicine, cancellationToken);
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Medicine>.Succeeded(medicine);
        }
    }
}
