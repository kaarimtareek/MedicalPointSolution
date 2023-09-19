using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface ICorrectionService
    {
        Task CorrectMedicinesBatch(int userId);
    }

    public class CorrectionService : ICorrectionService
    {
        private readonly ApplicationDbContext _context;

        public CorrectionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CorrectMedicinesBatch(int userId)
        {
            //get all medicines that doesn't have any batches
            var medicines = await _context.Medicines.Where(x => !x.IsDeleted && x.Batches.Count() == 0).ToListAsync(default);
            var batches = new List<MedicineBatch>();
            var histories = new List<MedicineHistory>();
            //add batch for each one
            foreach (var medicine in medicines)
            {
                medicine.OldestExpirationDate = DateTime.Now;
                var medicineBatch = new MedicineBatch
                {
                    MedicineId = medicine.Id,
                    CreatedAt = DateTime.Now,
                    ExpirationDate = DateTime.Now,
                    Price = medicine.Price,
                    Quantity = medicine.Quantity,
                    UserId = userId,
                    Notes = "",
                };
                var history = new MedicineHistory
                {
                    ActionType = ConstantMedicineActionType.ADD_BATCH,
                    CreatedAt = DateTime.Now,
                    ExpirationDate = DateTime.Now,
                    MedicineId = medicine.Id,
                    MedicineQuantity = medicine.Quantity,
                    UserId = userId,
                    Price = medicine.Price,
                };
                histories.Add(history);
                batches.Add(medicineBatch);

            }
            await _context.MedicineBatches.AddRangeAsync(batches);
            await _context.MedicineHistories.AddRangeAsync(histories);
            await _context.SaveChangesAsync();
        }
    }
}
