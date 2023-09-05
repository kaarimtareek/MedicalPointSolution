using System.Threading;
using Azure;

using ClosedXML.Excel;

using MedicalPoint.Common;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IUploadService
    {
        Task<byte[]> ExportPatients();
        Task UploadPatients(IFormFile file);
    }

    public class UploadService : IUploadService
    {
        private readonly ApplicationDbContext _context;

        public UploadService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task UploadPatients(IFormFile file)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);


                    using (var workbook = new XLWorkbook(memoryStream))
                    {
                        var worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return;
                        }
                        foreach (var row in worksheet.Rows())
                        {
                            
                            Console.WriteLine(row.ToString());
                            var cell = row.Cell(1);

                            Console.WriteLine(row.Cell(1)?.Value);  
                        }
                    }
                }

            }catch (Exception ex) 
            { 
                Console.WriteLine(ex);
            }

        }
        
        public async Task<byte[]> ExportPatients()
        {
            try
            {
               
                   
                var patients = await _context.Patients.AsNoTracking().ToListAsync();
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Patients");
                        
                        var currentRow = 1;
                        worksheet.Cell(currentRow, 1).Value = "Name";
                        worksheet.Cell(currentRow, 2).Value = "GeneralNumber";
                        worksheet.Cell(currentRow, 3).Value = "saray number";
                        worksheet.Cell(currentRow, 4).Value = "military number";

                        for (int i = 0; i < patients.Count; i++)
                        {
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Value = patients[i].Name;
                                worksheet.Cell(currentRow, 2).Value = patients[i].GeneralNumber;
                                worksheet.Cell(currentRow, 3).Value = patients[i].SaryaNumber;
                                worksheet.Cell(currentRow, 4).Value = patients[i].MilitaryNumber;

                            }
                        }
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                    return content;
                    }
                

            }catch (Exception ex) 
            { 
                Console.WriteLine(ex);
                return Array.Empty<byte>();
            }

        }

    }
}
