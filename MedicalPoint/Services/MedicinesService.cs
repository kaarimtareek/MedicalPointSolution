using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static Azure.Core.HttpHeader;

namespace MedicalPoint.Services
{
    public interface IMedicinesService
    {
        Task<OperationResult<Medicine>> Add(int userId, string name, int quantity, DateTime expirationDate, int? quantityThreshold = null, decimal price = 0, string notes = "", CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> AddBatch(int userId, int medicineId, int quantity, DateTime expirationDate, string notes ="" , decimal price = 0, CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> Delete(int userId, int medicineId, CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> DeleteBatch(int userId, int batchId, CancellationToken cancellationToken = default);
        Task<OperationResult<Medicine>> Edit(int userId, int medicineId, string name,  int? quantityThreshold = null, decimal price = 0, CancellationToken cancellationToken = default);  
        Task<OperationResult<Medicine>> EditBatch(int userId, int batchId, DateTime expirationDate, int quantity, string notes ="", decimal price =0 , CancellationToken cancellationToken = default);
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
            var result = await query.OrderBy(x=> x.Name).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Medicine> Get(int medicineId, bool withAllHistory = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Medicines.Include(x=> x.Batches.Where(x=> !x.IsDeleted).OrderBy(x=> x.ExpirationDate)).AsNoTracking();
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
        public async Task<OperationResult<Medicine>> Add(int userId, string name, int quantity, DateTime expirationDate,  int? quantityThreshold = null, decimal price = 0, string notes = "", CancellationToken cancellationToken = default)
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
            var medicineBatch = new MedicineBatch
            {
                Quantity = quantity,
                CreatedAt = DateTime.Now,
                ExpirationDate = expirationDate,
                Medicine = medicine,
                Notes = notes,
                UserId = userId,
                Price = price,
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
            await _context.MedicineBatches.AddAsync(medicineBatch, cancellationToken);
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Medicine>.Succeeded(medicine);
        }

        public async Task<OperationResult<Medicine>> AddBatch(int userId, int medicineId, int quantity, DateTime expirationDate, string notes = "", decimal price = 0, CancellationToken cancellationToken = default)
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
            if(medicine.OldestExpirationDate > expirationDate)
            {
                medicine.OldestExpirationDate = expirationDate;
            }
            var medicineBatch = new MedicineBatch
            {
                Quantity = quantity,
                CreatedAt = DateTime.Now,
                ExpirationDate = expirationDate,
                MedicineId = medicineId,
                Notes = notes,
                Price = price,
                UserId = userId
            };
            var medicineHistory = new MedicineHistory
            {
                MedicineQuantity = quantity,
                UserId = userId,
                ActionType = ConstantMedicineActionType.ADD_BATCH,
                CreatedAt = DateTime.Now,
                MedicineId = medicineId,
                MedicineName = "",
            };
            await _context.MedicineBatches.AddAsync(medicineBatch, cancellationToken);
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await UpdateMedicineExpirationDateAndQuantity(medicine.Id);
            return OperationResult<Medicine>.Succeeded(medicine);
        }
        public async Task<OperationResult<Medicine>> Edit(int userId, int medicineId, string name, int? quantityThreshold = null, decimal price = 0, CancellationToken cancellationToken = default)
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
            medicine.MinimumQuantityThreshold = quantityThreshold;
            medicine.Price = price;
            var medicineHistory = new MedicineHistory
            {
                MedicineId = medicineId,
                MedicineName = name,
                Price = price,
                MinimumQuantityThreshold = quantityThreshold,
                UserId = userId,
                CreatedAt = DateTime.Now,
                ActionType = ConstantMedicineActionType.EDIT,
            };
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Medicine>.Succeeded(medicine);
        }

        public async Task<OperationResult<Medicine>> EditBatch(int userId, int batchId, DateTime expirationDate, int quantity, string notes = "", decimal price = 0, CancellationToken cancellationToken = default)
        {
            if (quantity < 0)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.InvalidQuantity);
            }
            var medicineBatch = await _context.MedicineBatches.FirstOrDefaultAsync(x => x.Id == batchId);
            if(medicineBatch == null || medicineBatch.IsDeleted)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.MedicineNotFound);
            }
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == medicineBatch.MedicineId && !x.IsDeleted);
            if (medicine == null)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.MedicineNotFound);

            }
           
           medicineBatch.ExpirationDate = expirationDate;
            medicineBatch.Price = price;
            medicineBatch.Quantity = quantity;
            medicineBatch.Notes = notes;
            var medicineHistory = new MedicineHistory
            {
                MedicineQuantity = quantity,
                UserId = userId,
                ActionType = ConstantMedicineActionType.EDIT_BATCH,
                ExpirationDate = expirationDate,
                MedicineId = medicineBatch.MedicineId,
                CreatedAt = DateTime.Now,
                MedicineName = "",
            };
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await UpdateMedicineExpirationDateAndQuantity(medicine.Id);

            return OperationResult<Medicine>.Succeeded(medicine);
        }

        public async Task<OperationResult<Medicine>> DeleteBatch(int userId, int batchId, CancellationToken cancellationToken = default)
        {
            var medicineBatch = await _context.MedicineBatches.FirstOrDefaultAsync(x => x.Id == batchId);
            if (medicineBatch == null || medicineBatch.IsDeleted)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.MedicineNotFound);
            }
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == medicineBatch.MedicineId && !x.IsDeleted);
            if (medicine == null)
            {
                return OperationResult<Medicine>.Failed(ConstantMessageCodes.MedicineNotFound);

            }

            medicineBatch.IsDeleted = true;
            var medicineHistory = new MedicineHistory
            {
                MedicineQuantity = medicineBatch.Quantity,
                UserId = userId,
                ExpirationDate = medicineBatch.ExpirationDate,
                ActionType = ConstantMedicineActionType.DELETE_BATCH,
                MedicineId = medicineBatch.MedicineId,
                CreatedAt = DateTime.Now,
                MedicineName = "",
            };
            await _context.MedicineHistories.AddAsync(medicineHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await UpdateMedicineExpirationDateAndQuantity(medicine.Id);
            return OperationResult<Medicine>.Succeeded(medicine);
        }
        public async Task UpdateMedicineExpirationDateAndQuantity(int medicineId)
        {
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x=> x.Id == medicineId);
            if (medicine == null || medicine.IsDeleted)
                return;
            var quantity = await _context.MedicineBatches.Where(x => x.MedicineId == medicineId && !x.IsDeleted).SumAsync(x => x.Quantity);
            var oldestExpirationDate = await _context.MedicineBatches.Where(x => x.MedicineId == medicineId && !x.IsDeleted).OrderBy(x => x.ExpirationDate).Select(x => x.ExpirationDate).FirstOrDefaultAsync();
            medicine.Quantity = quantity;
            medicine.OldestExpirationDate = oldestExpirationDate;
            await _context.SaveChangesAsync();
        }
    }
}
