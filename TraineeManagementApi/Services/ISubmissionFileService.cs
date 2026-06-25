using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface ISubmissionFileService
    {
        Task<IResult> GetFileAsync(int id);        
        Task DeleteFileAsync(int id);
    } 
}