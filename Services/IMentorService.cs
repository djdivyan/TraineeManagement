using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface IMentorService
    {
        Task<List<MentorResponse>> GetAllAsync(string? search);
        Task<MentorResponse?> GetByIdAsync(int id);
        
        Task<MentorResponse> CreateAsync(MentorRequest mentorRequest);
        Task<MentorResponse?> UpdateAsync(int id, UpdateMentorRequest updateMentoreRequest);
        Task<bool> DeleteAsync(int id);
        // Task<PaginationResponse<MentorResponse>> GetPagedDataAsync(PaginationRequest paginationRequest);
    } 
}