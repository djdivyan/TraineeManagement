using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class TraineeService(AppDbContext trainees) : ITraineeService
    {
        private readonly AppDbContext _traineeContext = trainees;
        

        public async Task<IEnumerable<TraineeResponse>> GetAllAsync(string? search)
        {
            var query = _traineeContext.Trainees.AsQueryable();
            if(!string.IsNullOrEmpty(search))
                query = query.Where(
                    t =>  t.FirstName.Contains(search) || 
                    t.LastName.Contains(search)        || 
                    t.Email.Contains(search)           || 
                    t.TechStack.Contains(search));
        
            var trainees = await query.ToListAsync();
            return trainees.Select(MapToResponse).ToList();
        }

        public async Task<TraineeResponse?> GetByIdAsync(int id)
        {
            var trainee = await _traineeContext.Trainees.FindAsync(id);
            if (trainee is null)
            {
                return null;
            }
            return MapToResponse(trainee);
        }

        public async Task<TraineeResponse> CreateAsync(CreateTraineeRequest createTraineeRequest)
        {
            var trainee = new Trainee
            {
            FirstName = createTraineeRequest.FirstName,
            LastName = createTraineeRequest.LastName,
            Email = createTraineeRequest.Email, 
            TechStack = createTraineeRequest.TechStack, 
            Status = createTraineeRequest.Status,
            CreatedDate = DateTime.Now, 
            UpdatedDate = DateTime.Now
            };

            _traineeContext.Trainees.Add(trainee);
            await _traineeContext.SaveChangesAsync();
            return MapToResponse(trainee);
        }

        public async Task<TraineeResponse?> UpdateAsync(int id, UpdateTraineeRequest updateTraineeRequest)
        {
            var trainee = await _traineeContext.Trainees.FindAsync(id);
            if(trainee is null) return null;
            trainee.FirstName = updateTraineeRequest.FirstName;
            trainee.LastName = updateTraineeRequest.LastName;
            trainee.Email = updateTraineeRequest.Email;
            trainee.TechStack = updateTraineeRequest.TechStack; 
            trainee.Status = updateTraineeRequest.Status;
            trainee.UpdatedDate = DateTime.Now;

            await _traineeContext.SaveChangesAsync();
            return MapToResponse(trainee);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var trainee = await _traineeContext.Trainees.FindAsync(id);
            if(trainee is null)
                return false;

            _traineeContext.Trainees.Remove(trainee);
            await _traineeContext.SaveChangesAsync();
            return true;
        }

         public static TraineeResponse MapToResponse(Trainee trainee)
        {
            return new TraineeResponse
            {
            Id = trainee.Id,
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email, 
            TechStack = trainee.TechStack, 
            Status = trainee.Status,
            CreatedDate = trainee.CreatedDate, 
            UpdatedDate = trainee.UpdatedDate
            };
        }
    }
}