using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class SubmissionFileService(AppDbContext dbContext, ILogger<SubmissionFileService> logger, IFileStorageService fileStorageService, IWebHostEnvironment environment) : ISubmissionFileService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<SubmissionFileService> _logger = logger;
        private readonly IWebHostEnvironment _env = environment;
        private readonly IFileStorageService _fileManager = fileStorageService;

        public async Task DeleteFileAsync(int id)
        {
            _logger.LogInformation("DeleteFileAsync:SubmissionFile : Entering the Function");            
            SubmissionFile? submissionFile = await _dbContext.SubmissionFiles.FindAsync(id);
            if (submissionFile is null)
            {
                _logger.LogError("DeleteFileAsync:SubmissionFile : SubmissionFile Not found with {id}", id);
                throw new NotFoundException("SubmissionFile",id);
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
            string generatedStorageName = submissionFile.GeneratedStorageName;
            
            _logger.LogInformation("GetByIdAsync:SubmissionFile : SubmissionFile returned with {id}", id);
            return await _fileManager.OpenReadAsync(generatedStorageName);
        }
    }
}
