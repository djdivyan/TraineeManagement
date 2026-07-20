using Models;
using TraineeManagementApi.Exceptions;
using TraineeManagement.Shared.Models;
using System.Security.Claims;

namespace TraineeManagementApi.Services
{
    class SubmissionFileService(AppDbContext dbContext, ILogger<SubmissionFileService> logger, IFileStorageService fileStorageService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : ISubmissionFileService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<SubmissionFileService> _logger = logger;
        private readonly IWebHostEnvironment _env = environment;
        private readonly IFileStorageService _fileManager = fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task DeleteFileAsync(int id)
        {
            _logger.LogInformation("DeleteFileAsync:SubmissionFile : Entering the Function");            
            SubmissionFile? submissionFile = await _dbContext.SubmissionFiles.FindAsync(id);
            if (submissionFile is null)
            {
                _logger.LogError("DeleteFileAsync:SubmissionFile : SubmissionFile Not found with {id}", id);
                throw new NotFoundException("SubmissionFile",id);
            }
            //Authorization
            if (!CheckAuthorization(submissionFile))
            {
                _logger.LogWarning("Forbidden Access");
                throw new ForbiddenException();
            }
            string generatedStorageName = submissionFile.GeneratedStorageName;
            
            _logger.LogInformation("DeleteFileAsync:SubmissionFile : SubmissionFile Deleted with {id}", id);
            await _fileManager.DeleteAsync(generatedStorageName);
            _dbContext.SubmissionFiles.Remove(submissionFile);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IResult> GetFileAsync(int id)
        {
            _logger.LogInformation("GetByIdAsync:SubmissionFile : Entering the Function");            
            SubmissionFile? submissionFile = await _dbContext.SubmissionFiles.FindAsync(id);
            if (submissionFile is null)
            {
                _logger.LogError("GetByIdAsync:SubmissionFile : SubmissionFile Not found with {id}", id);
                throw new NotFoundException("SubmissionFile",id);
            }
            //Authorization
            // Throw if the user is neither the owner or admin/mentor
            if (!CheckAuthorization(submissionFile))
            {
                _logger.LogWarning("Forbidden Access");
                throw new ForbiddenException();
            }

            string generatedStorageName = submissionFile.GeneratedStorageName;
            
            _logger.LogInformation("GetByIdAsync:SubmissionFile : SubmissionFile returned with {id}", id);
            FileStream stream = await _fileManager.OpenReadAsync(generatedStorageName);

            return Results.File(stream, "application/octet-stream", submissionFile.OriginalFileName);
        }


        private bool CheckAuthorization(SubmissionFile submissionFile)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            _ = int.TryParse(user?.GetUserId(), out int uid);            
            string role = user?.GetRole() ?? string.Empty;

            bool isOwner = (uid == submissionFile.UploadedByUser);
            bool isAdminOrMentor = string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) || 
                                string.Equals(role, "mentor", StringComparison.OrdinalIgnoreCase);

            // Throw if the user is neither the owner or admin/mentor
            if (!isOwner && !isAdminOrMentor)
            {
                return false;
            }

            return true;
        }
    }
}
