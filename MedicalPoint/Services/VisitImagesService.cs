using System.IO.Compression;

using MedicalPoint.Common;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IVisitImagesService
    {
        Task<OperationResult<VisitImage>> Add(IFormFile image, int visitId, CancellationToken cancellationToken = default);
        Task<VisitImage> Get(int imageId, CancellationToken cancellationToken = default);
        Task<List<VisitImage>> GetImagesForVisit(int visitId, CancellationToken cancellationToken = default);
        Task<OperationResult<VisitImage>> Remove(int visitImageId, CancellationToken cancellationToken = default);
        Task<OperationResult<VisitImage>> Replace(IFormFile image, int visitImageId, CancellationToken cancellationToken = default);
    }

    public class VisitImagesService : IVisitImagesService
    {
        private readonly ApplicationDbContext _context;

        public VisitImagesService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<VisitImage>> GetImagesForVisit(int visitId, CancellationToken cancellationToken = default)
        {
            var images = await _context.VisitImages.AsNoTracking().Where(x => x.VisitId == visitId && !x.IsDeleted).ToListAsync(cancellationToken);
            return images;
        }
        public async Task<VisitImage> Get(int imageId, CancellationToken cancellationToken = default)
        {
            var image = await _context.VisitImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == imageId && !x.IsDeleted, cancellationToken);
            return image;
        }
        public async Task<OperationResult<VisitImage>> Add(IFormFile image, int visitId, CancellationToken cancellationToken = default)
        {
            var visit = QueryFinder.GetVisitById(_context, visitId);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitImage>.Failed("");
            }
            if (!visit.CanEditVisit())
            {
                return OperationResult<VisitImage>.Failed("");
            }
            if (image == null || image.Length == 0)
            {
                return OperationResult<VisitImage>.Failed("");

            }

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream, cancellationToken);

                // Upload the file if less than 2 MB
                if (memoryStream.Length >= 2097152)
                {
                    return OperationResult<VisitImage>.Failed("");
                }
                var visitImage = new VisitImage()
                {
                    Format = image.ContentType,
                    Name = image.Name,
                    VisitId = visitId,
                    IsDeleted = false,
                    Content = memoryStream.ToArray(),
                    Path = string.Empty,
                };
                await _context.VisitImages.AddAsync(visitImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<VisitImage>.Succeeded(visitImage);

            }
        }
        public async Task<OperationResult<VisitImage>> Replace(IFormFile image, int visitImageId, CancellationToken cancellationToken = default)
        {

            var visitImage = await _context.VisitImages.FirstOrDefaultAsync(x => x.Id == visitImageId, cancellationToken);
            if (visitImage == null)
            {
                return OperationResult<VisitImage>.Failed("");
            }
            var visit = QueryFinder.GetVisitById(_context, visitImage.VisitId);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitImage>.Failed("");
            }
            if (!visit.CanEditVisit())
            {
                return OperationResult<VisitImage>.Failed("");
            }
            if (image == null || image.Length > 0)
            {
                return OperationResult<VisitImage>.Failed("");

            }
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream, cancellationToken);

                // Upload the file if less than 2 MB
                if (memoryStream.Length >= 2097152)
                {
                    return OperationResult<VisitImage>.Failed("");
                }
                visitImage.Format = image.ContentType;
                visitImage.Name = image.Name;
                visitImage.Content = memoryStream.ToArray();

                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<VisitImage>.Succeeded(visitImage);

            }
        }
        public async Task<OperationResult<VisitImage>> Remove(int visitImageId, CancellationToken cancellationToken = default)
        {

            var visitImage = await _context.VisitImages.FirstOrDefaultAsync(x => x.Id == visitImageId, cancellationToken);
            if (visitImage == null)
            {
                return OperationResult<VisitImage>.Failed("");
            }
            var visit = QueryFinder.GetVisitById(_context, visitImage.VisitId);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitImage>.Failed("");
            }
            if (!visit.CanEditVisit())
            {
                return OperationResult<VisitImage>.Failed("");
            }

            _context.VisitImages.Remove(visitImage);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitImage>.Succeeded(visitImage);

        }

    }
}
