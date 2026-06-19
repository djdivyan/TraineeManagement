using System.IO.Pipelines;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace TraineeManagementApi.Services
{
    public class LocalFileManagerService(IWebHostEnvironment environment) : IFileStorageService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        private readonly IWebHostEnvironment _env = environment;

        public async Task<string> SaveAsync(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is null or empty.");

                if (file.Length > _maxFileSize)
                    throw new InvalidDataException($"File exceeds maximum size of {_maxFileSize / (1024 * 1024)} MB.");

                var contentPath = _env.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads");
                // path = "c://projects/ImageManipulation.Ap/uploads" ,not exactly, but something like that

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check the allowed extenstions
                var extension = Path.GetExtension(file.FileName);
                if (!allowedFileExtensions.Contains(extension))
                {
                    throw new InvalidDataException($"Unsupported file extension: {extension}. Allowed: {string.Join(", ", allowedFileExtensions)}.");
                }

                // Generate a unique filename to avoid overwrites
                var fileName = $"{Guid.NewGuid()}{extension}";
                var fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return fileName;
            }


            public async Task DeleteAsync(string fileNameWithExtension)
            {
                if (string.IsNullOrEmpty(fileNameWithExtension))
                {
                    throw new ArgumentNullException(nameof(fileNameWithExtension));
                }

                var contentPath = _env.ContentRootPath;
                var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Invalid file path");
                }

                await Task.Run(() => File.Delete(path));

            }

        public async Task<IResult> OpenReadAsync(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
                {
                    throw new ArgumentNullException(nameof(fileNameWithExtension));
                }

                var contentPath = _env.ContentRootPath;
                var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Invalid file path");
                }

                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(path,out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                byte[]? bytes = await File.ReadAllBytesAsync(path);
                return Results.File(bytes,contentType, Path.GetFileName(path));
        }

        public async Task<bool> ExistsAsync(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }

            var contentPath = _env.ContentRootPath;
            var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);
            return await Task.FromResult(File.Exists(path));
        }
    }
}