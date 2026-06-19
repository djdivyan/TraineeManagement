namespace TraineeManagementApi.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(IFormFile formFile);
        Task<IResult> OpenReadAsync(string fileNameWithExtension);
        Task<bool> ExistsAsync(string fileNameWithExtension);
        Task DeleteAsync(string fileNameWithExtension);
    }
}