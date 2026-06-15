using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class LearningTaskService(AppDbContext dbContext, ILogger<LearningTaskService> logger) : ILearningTaskService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<LearningTaskService> _logger = logger;
        

        public async Task<List<LearningTaskResponse>> GetAllAsync(string? search)
        {
            IQueryable<LearningTask> query = _dbContext.LearningTasks.AsQueryable();
            if(!string.IsNullOrEmpty(search))
                query = query.Where( l =>  
                    l.Title.Contains(search) || 
                    l.Description.Contains(search) || 
                    l.ExpectedTechStack.Contains(search)
                );
        
            List<LearningTask> learningTasks = await query.ToListAsync();
            _logger.LogInformation("GetAllAsnc Successfully returned Learning Tasks");
            return learningTasks.Select(MapToResponse).ToList();
        }

        public async Task<LearningTaskResponse?> GetByIdAsync(int id)
        {
            LearningTask? learningTask = await _dbContext.LearningTasks.FindAsync(id);
            if (learningTask is null)
            {
                _logger.LogError("GetByID : Learning Task Not found with {id}", id);
                return null;
            }
            _logger.LogInformation("GetByID : Learning Task found with {id}", id);
            return MapToResponse(learningTask);
        }

        public async Task<LearningTaskResponse> CreateAsync(LearningTaskRequest learningTaskRequest)
        {
            LearningTask learningTask = new()
            {
                Title = learningTaskRequest.Title,
                Description = learningTaskRequest.Description,
                ExpectedTechStack = learningTaskRequest.ExpectedTechStack, 
                DueDate = learningTaskRequest.DueDate, 
                LearningTaskStatus = learningTaskRequest.LearningTaskStatus,
                CreatedDate = DateTime.UtcNow, 
                UpdatedDate = DateTime.UtcNow
            };

            _dbContext.LearningTasks.Add(learningTask);
            await _dbContext.SaveChangesAsync();


            _logger.LogInformation("Create : New Learning Task created with id {id} at {DateTime}",learningTask.Id, learningTask.CreatedDate);
            return MapToResponse(learningTask);
        }

        public async Task<LearningTaskResponse?> UpdateAsync(int id, UpdateLearningTaskRequest updateLearningTaskRequest)
        {
            LearningTask? learningTask = await _dbContext.LearningTasks.FindAsync(id);
            if(learningTask is null)
            {
              _logger.LogError("Update : Learning Task with id {id} Could not be found for updation",id);
              return null;  
            } 
            learningTask.Title = updateLearningTaskRequest.Title;
            learningTask.Description = updateLearningTaskRequest.Description;
            learningTask.ExpectedTechStack = updateLearningTaskRequest.ExpectedTechStack;
            learningTask.DueDate = updateLearningTaskRequest.DueDate;
            learningTask.LearningTaskStatus = updateLearningTaskRequest.LearningTaskStatus;
            learningTask.UpdatedDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();


            _logger.LogInformation("Update : Learning Task with id {id} updated at {DateTime}",learningTask.Id,learningTask.UpdatedDate);
            return MapToResponse(learningTask);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            LearningTask? learningTask = await _dbContext.LearningTasks.FindAsync(id);
            if(learningTask is null)
            {
                _logger.LogError("Delete : Learning Task with id {id} could not be found for deletion",id);
                return false;
            }
                

            _dbContext.LearningTasks.Remove(learningTask);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Delete : Learning Task with id {id} deleted",id);
            return true;
        }

         public static LearningTaskResponse MapToResponse(LearningTask learningTask)
        {
            return new LearningTaskResponse
            {
                Id = learningTask.Id,
                Title = learningTask.Title,
                Description = learningTask.Description,
                ExpectedTechStack = learningTask.ExpectedTechStack, 
                DueDate = learningTask.DueDate, 
                LearningTaskStatus = learningTask.LearningTaskStatus,
                CreatedDate = learningTask.CreatedDate, 
                UpdatedDate = learningTask.UpdatedDate
            };
        }

        // public async Task<PaginationResponse<MentorResponse>> GetPagedDataAsync(PaginationRequest paginationRequest)
        // {
        //     var query = _dbContext.Mentors.AsQueryable();

        //     if (!string.IsNullOrEmpty(paginationRequest.Search))
        //     {
        //         _logger.LogInformation("Get : Searching {search} in Database",paginationRequest.Search);
        //         query = query.Where(m =>
        //           m.FirstName.Contains(paginationRequest.Search) ||   
        //           m.LastName.Contains(paginationRequest.Search) ||
        //           m.Email.Contains(paginationRequest.Search) ||
        //           m.Expertise.Contains(paginationRequest.Search)
        //         );
        //     }


        //     if (!string.IsNullOrEmpty(paginationRequest.Status.ToString()))
        //     {
        //         _logger.LogInformation("Get : Filter with status {Status} in Database",paginationRequest.Status.ToString());
        //         query = query.Where(t =>
        //         t.MentorStatus == paginationRequest.Status
        //         );
        //     }

        //     var totalRecords = await query.CountAsync();



        //     var data = (await query.AsNoTracking()
        //                     .Skip((paginationRequest.PageNumber - 1)* paginationRequest.PageSize)
        //                     .Take(paginationRequest.PageSize)
        //                     .ToListAsync())
        //                     .Select(MapToResponse)
        //                     .ToList(); 

        //     var result = new PaginationResponse<MentorResponse>
        //     {
        //         PageNumber = paginationRequest.PageNumber,
        //         PageSize = paginationRequest.PageSize,
        //         TotalRecords = totalRecords,
        //         Data = data
        //     };

        //     _logger.LogInformation("Get: Successfully returned Mentors with PageNumber {PageNUmber} and PageSize {PageSize}",paginationRequest.PageNumber,paginationRequest.PageNumber);

        //     return result;
        // }
    }
}