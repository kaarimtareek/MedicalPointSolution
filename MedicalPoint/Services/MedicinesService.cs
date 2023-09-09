using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IMedicinesService
    {
        Task<OperationResult<Medicine>> Add(int userId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> AddQauntity(int userId, int medicineId, int quantity, CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> Delete(int userId, int medicineId, CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> Edit(int userId, int medicineId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default);
        Task<Medicine> Get(int medicineId, bool withAllHistory = false, CancellationToken cancellationToken = default);
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
            var query = _context.Medicines.AsNoTracking().Where(x=> !x.IsDeleted).AsQueryable();
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

        public async Task<Medicine> Get(int medicineId, bool withAllHistory = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Medicines.AsNoTracking();
            if(withAllHistory)
            {
                query = query.Include(x => x.History.OrderByDescending(x => x.CreatedAt))
                        .ThenInclude(x => x.User);
            }
            else
            {
                query = query.Include(x => x.History.OrderByDescending(x => x.CreatedAt).Take(5))
                        .ThenInclude(x => x.User);
            } 
            var result = await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == medicineId, cancellationToken);

            return result;
        }


        public async Task<OperationResult<Medicine>> Delete(int userId, int medicineId, CancellationToken cancellationToken = default)
        {
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == medicineId, cancellationToken);

             if(medicine == null)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.MedicineNotFound);    
            }

            medicine.IsDeleted = true;
            var history = new MedicineHistory
            {
                MedicineId = medicineId,
                ActionType = ConstantMedicineActionType.DELETE,
                MedicineName = medicine.Name,
                MedicineQuantity = medicine.Quantity,
                MinimumQuantityThreshold = medicine.MinimumQuantityThreshold,
                CreatedAt = DateTime.Now,
                UserId = userId,
            };
            await _context.MedicineHistories.AddAsync(history);
            await _context.SaveChangesAsync();
            return OperationResult<Medicine>.Succeeded(medicine);
        }
        public async Task<OperationResult<Medicine>> Add(int userId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default)
        {
            if (QueryValidator.IsMedicineNameExist(_context, name))
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.NameAlreadyExist);
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
                CreatedAt = DateTime.Now,
                ActionType = ConstantMedicineActionType.ADD,
            };
            await _context.Medicines.AddAsync(medicine, cancellationToken);
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Medicine>.Succeeded(medicine);
        }

        public async Task<OperationResult<Medicine>> AddQauntity(int userId, int medicineId, int quantity, CancellationToken cancellationToken = default)
        {
            if(quantity < 0)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.InvalidQuantity);
            }
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == medicineId && !x.IsDeleted);
            if(medicine == null)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.MedicineNotFound);

            }
            medicine.Quantity += quantity;
            var medicineHistory = new MedicineHistory
            {
                MedicineQuantity = quantity,
                UserId = userId,
                ActionType = ConstantMedicineActionType.ADD_QUANTITY,
                CreatedAt = DateTime.Now,
                MedicineId = medicineId,
                MedicineName = "",
            };
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Medicine>.Succeeded(medicine);
        }
        public async Task<OperationResult<Medicine>> Edit(int userId, int medicineId, string name, int quantity, int? quantityThreshold = null, CancellationToken cancellationToken = default)
        {
            if (QueryValidator.IsMedicineNameExist(_context, name, medicineId))
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.NameAlreadyExist);
            }
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == medicineId && !x.IsDeleted, cancellationToken);
            if (medicine == null)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.MedicineNotFound);

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
                CreatedAt = DateTime.Now,
                ActionType = ConstantMedicineActionType.EDIT,
            };
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Medicine>.Succeeded(medicine);
        }
    }
}
