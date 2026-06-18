using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class TraineeService(AppDbContext trainees, ILogger<TraineeService> logger) : ITraineeService
    {
        private readonly AppDbContext _traineeContext = trainees;
        private readonly ILogger<TraineeService> _logger = logger;
        
        public async Task<IEnumerable<TraineeResponse>> GetAllAsync(string? search)
        {
            _logger.LogInformation("GetAllAsync:Trainee : Entering the Function");

            IQueryable<Trainee> query = _traineeContext.Trainees.AsQueryable();
            if(!string.IsNullOrEmpty(search))
                query = query.Where(
                    t =>  t.FirstName.Contains(search) || 
                    t.LastName.Contains(search)        || 
                    t.Email.Contains(search)           || 
                    t.TechStack.Contains(search));
        
            List<Trainee> trainees = await query.ToListAsync();
            _logger.LogInformation("GetAllAsync:Trainee : Successfully returned trainees");
            return trainees.Select(MapToResponse).ToList();
        }

        public async Task<TraineeResponse> GetByIdAsync(int id)
        {
            _logger.LogInformation("GetByIdAsync:Trainee : Entering the Function");

            Trainee? trainee = await _traineeContext.Trainees.FindAsync(id);
            if (trainee is null)
            {
                _logger.LogWarning("GetByIdAsync:Trainee : Trainee Not found with {id}", id);
                throw new NotFoundException("Trainee",id);
            }
            _logger.LogInformation("GetByIdAsync:Trainee : Trainee found with {id}", id);
            return MapToResponse(trainee);
        }

        public async Task<TraineeResponse> CreateAsync(CreateTraineeRequest createTraineeRequest)
        {
            _logger.LogInformation("CreateAsync:Trainee : Entering the Function");

            Trainee trainee = new Trainee
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

            _logger.LogInformation("CreateAsync:Trainee : New Trainee created with id {id} at {DateTime}",trainee.Id, trainee.CreatedDate);
            return MapToResponse(trainee);
        }

        public async Task<TraineeResponse> UpdateAsync(int id, UpdateTraineeRequest updateTraineeRequest)
        {
            _logger.LogInformation("UpdateAsync:Trainee : Entering the Function");

            Trainee? trainee = await _traineeContext.Trainees.FindAsync(id);
            if(trainee is null)
            {
                _logger.LogError("UpdateAsync:Trainee : Trainee with id {id} Could not be found for updation",id);
                throw new NotFoundException("Trainee",id);
            } 

            trainee.FirstName = updateTraineeRequest.FirstName;
            trainee.LastName = updateTraineeRequest.LastName;
            trainee.Email = updateTraineeRequest.Email;
            trainee.TechStack = updateTraineeRequest.TechStack; 
            trainee.Status = updateTraineeRequest.Status;
            trainee.UpdatedDate = DateTime.Now;

            await _traineeContext.SaveChangesAsync();

            _logger.LogInformation("UpdateAsync:Trainee : Trainee with id {id} updated at {DateTime}",trainee.Id,trainee.UpdatedDate);
            return MapToResponse(trainee);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("DeleteAsync:Trainee : Entering the Function");

            Trainee? trainee = await _traineeContext.Trainees.FindAsync(id);
            
            if(trainee is null)
            {
                _logger.LogError("DeleteAsync:Trainee : Trainee with id {id} could not be found for deletion",id);
                throw new NotFoundException("Trainee",id);
            }
                
            _traineeContext.Trainees.Remove(trainee);
            await _traineeContext.SaveChangesAsync();

            _logger.LogInformation("DeleteAsync:Trainee : Trainee with id {id} deleted",id);
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

        public async Task<PaginationResponse<TraineeResponse>> GetPagedDataAsync(PaginationRequest paginationRequest)
        {
            _logger.LogInformation("GetPagedDataAsync:Trainee : Entering the Function");

            IQueryable<Trainee> query = _traineeContext.Trainees.AsQueryable();

            if (!string.IsNullOrEmpty(paginationRequest.Search))
            {
                _logger.LogInformation("GetPagedDataAsync:Trainee : Searching {search} in Database",paginationRequest.Search);
                query = query.Where(t =>
                  t.FirstName.Contains(paginationRequest.Search) ||   
                  t.LastName.Contains(paginationRequest.Search) ||
                  t.Email.Contains(paginationRequest.Search) ||
                  t.TechStack.Contains(paginationRequest.Search)
                );
            }


            if (!string.IsNullOrEmpty(paginationRequest.Status.ToString()))
            {
                _logger.LogInformation("GetPagedDataAsync:Trainee : Filtering with status {Status} in Database",paginationRequest.Status.ToString());
                query = query.Where(t =>
                t.Status == paginationRequest.Status
                );
            }

            int totalRecords = await query.CountAsync();

            List<TraineeResponse> data = (await query.AsNoTracking()
                            .Skip((paginationRequest.PageNumber - 1)* paginationRequest.PageSize)
                            .Take(paginationRequest.PageSize)
                            .ToListAsync())
                            .Select(MapToResponse)
                            .ToList(); 

            PaginationResponse<TraineeResponse> result = new PaginationResponse<TraineeResponse>
            {
                PageNumber = paginationRequest.PageNumber,
                PageSize = paginationRequest.PageSize,
                TotalRecords = totalRecords,
                Data = data
            };

            _logger.LogInformation("GetPagedDataAsync:Trainee : Successfully returned trainees with PageNumber {PageNUmber} and PageSize {PageSize}",paginationRequest.PageNumber,paginationRequest.PageNumber);

            return result;
        }
    }
}