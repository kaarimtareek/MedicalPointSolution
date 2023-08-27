using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IMedicinesService
    {
        Task<OperationResult<Medicine>> Add(int userId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> Edit(int userId, int medicineId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default);
        Task<Medicine> Get(int medicineId, CancellationToken cancellationToken = default);
        Task<List<Medicine>> GetAll(string name = "", int? quantityLessThan = null, bool medicinesAboutToFinish = false, CancellationToken cancellationToken = default);
    }

    public class MedicinesService : IMedicinesService
    {
        private readonly ApplicationDbContext _context;

        public MedicinesService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Medicine>> GetAll(string name = "", int? quantityLessThan = null, bool medicinesAboutToFinish = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Medicines.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (quantityLessThan != null)
            {
                query = query.Where(x => x.Quantity < quantityLessThan);
            }
            if (medicinesAboutToFinish)
            {
                query = query.Where(x => x.MinimumQuantityThreshold.HasValue && x.MinimumQuantityThreshold.Value >= x.Quantity);
            }
            var result = await query.ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Medicine> Get(int medicineId, CancellationToken cancellationToken = default)
        {
            var result = await _context.Medicines.Include(x => x.History).AsNoTracking().FirstOrDefaultAsync(x => x.Id == medicineId, cancellationToken);

            return result;
        }

        public async Task<OperationResult<Medicine>> Add(int userId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default)
        {
            if (QueryValidator.IsMedicineNameExist(_context, name))
            {
                return OperationResult<Medicine>.Failed("");
            }
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
        public async Task<OperationResult<Medicine>> Edit(int userId, int medicineId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default)
        {
            if (QueryValidator.IsMedicineNameExist(_context, name, medicineId))
            {
                return OperationResult<Medicine>.Failed("");
            }
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == medicineId && !x.IsDeleted, cancellationToken);
            if (medicine == null)
            {
                return OperationResult<Medicine>.Failed("");

            }
            medicine.Name = name;
            medicine.LastUpdatedAt = DateTime.Now;
            medicine.Quantity = quantity;
            medicine.MinimumQuantityThreshold = quantityThreshold;
            var medicineHistory = new MedicineHistory
            {
                MedicineId = medicineId,
                MedicineName = name,
                MedicineQuantity = quantity,
                MinimumQuantityThreshold = quantityThreshold,
                UserId = userId,
                ActionType = ConstantMedicineActionType.EDIT,
            };
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Medicine>.Succeeded(medicine);
        }
    }
}
