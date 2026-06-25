using JobBoard.Api.Application.Interfaces;

namespace JobBoard.Api.Infrastructure.Services
{
    public class FileValidationService : IFileValidationService
    {
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".doc",
            ".docx"
        };

        private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        public (bool IsValid, string? ErrorMessage) ValidateResume(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "Resume file is required.");

            if (file.Length > MaxFileSize)
                return (false, $"File size exceeds the maximum allowed size of 5 MB.");

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                return (false, $"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}.");

            if (!AllowedMimeTypes.Contains(file.ContentType))
                return (false, $"Invalid MIME type.");

            return (true, null);
        }
    }
}
