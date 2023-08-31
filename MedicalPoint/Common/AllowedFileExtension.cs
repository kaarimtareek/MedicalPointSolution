using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Common
{
    public static class AllowedFileExtensions
    {
        private static readonly string[] _allowedExtensions = new string[3] { ".png", ".jpeg", ".jpg" };
        public static bool IsValid(
        IFormFile file, string[] extensions = null)
        {
            extensions ??= _allowedExtensions;
            
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!extensions.Contains(extension.ToLower()))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
