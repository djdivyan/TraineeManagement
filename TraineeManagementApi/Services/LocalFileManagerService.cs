using System.IO.Pipelines;
using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace TraineeManagementApi.Services
{
    public class LocalFileManagerService(IWebHostEnvironment environment, ILogger<LocalFileManagerService> logger) : IFileStorageService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly ILogger<LocalFileManagerService> _logger = logger;
        private readonly string[] allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        private readonly IWebHostEnvironment _env = environment;

        public async Task<string> SaveAsync(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is null or empty.");

                if (file.Length > _maxFileSize)
                    throw new InvalidDataException($"File exceeds maximum size of {_maxFileSize / (1024 * 1024)} MB.");

                string contentPath = _env.ContentRootPath;
                string path = Path.Combine(contentPath, "Uploads");
                _logger.LogInformation("FileStorage:SaveAsync - File stored at path {path} with root as {root}", path, contentPath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check the allowed extenstions
                string extension = Path.GetExtension(file.FileName);
                if (!allowedFileExtensions.Contains(extension))
                {
                    throw new InvalidDataException($"Unsupported file extension: {extension}. Allowed: {string.Join(", ", allowedFileExtensions)}.");
                }

                // Generate a unique filename to avoid overwrites
                string fileName = $"{Guid.NewGuid()}{extension}";
                string fileNameWithPath = Path.Combine(path, fileName);

                using (FileStream stream = new FileStream(fileNameWithPath, FileMode.Create))
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

                string contentPath = _env.ContentRootPath;
                string path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Invalid file path");
                }

                await Task.Run(() => File.Delete(path));

            }

        public async Task<FileStream> OpenReadAsync(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
                {
                    throw new ArgumentNullException(nameof(fileNameWithExtension));
                }

                string contentPath = _env.ContentRootPath;
                string path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Invalid file path");
                }


                FileStream stream = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 81920,
                    useAsync: true // Enables async operations
                );
                return stream;
                // return Results.File(stream, "application/octet-stream", Path.GetFileName(path));
        }

        public async Task<bool> ExistsAsync(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }

            string contentPath = _env.ContentRootPath;
            string path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);
            return await Task.FromResult(File.Exists(path));
        }
    }
}