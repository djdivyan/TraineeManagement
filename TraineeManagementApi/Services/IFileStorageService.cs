namespace TraineeManagementApi.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(IFormFile formFile);
        Task<FileStream> OpenReadAsync(string fileNameWithExtension);
        Task<bool> ExistsAsync(string fileNameWithExtension);
        Task DeleteAsync(string fileNameWithExtension);
    }
}