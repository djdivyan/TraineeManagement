using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class TaskAssignmentService(AppDbContext dbContext, ILogger<TaskAssignmentService> logger) : ITaskAssignmentService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<TaskAssignmentService> _logger = logger;


        public async Task<List<TaskAssignmentResponse>> GetAllAsync()
        {
            IQueryable<TaskAssignment> query = _dbContext.TaskAssignments.AsNoTracking();
                                                        // .Include(ta => ta.Trainee)
                                                        // .Include(ta => ta.Mentor)
                                                        // .Include(ta => ta.LearningTask);

            List<TaskAssignment> taskAssignments = await query.ToListAsync();
            _logger.LogInformation("GetAllAsnc Successfully returned all Task Assignments");
            return taskAssignments.Select(MapToResponse).ToList();
        }

        public async Task<TaskAssignmentResponse?> GetByIdAsync(int id)
        {
            TaskAssignment? taskAssignment = await _dbContext.TaskAssignments.FindAsync(id);
            if (taskAssignment is null)
            {
                _logger.LogError("GetByID : Task Assignments Not found with {id}", id);
                return null;
            }
            _logger.LogInformation("GetByID : Task Assignments found with {id}", id);
            return MapToResponse(taskAssignment);
        }

        public async Task<TaskAssignmentResponse?> CreateAsync(TaskAssignmentRequest taskAssignmentRequest)
        {
            if (await _dbContext.Trainees.FindAsync(taskAssignmentRequest.TraineeId) == null || await _dbContext.Mentors.FindAsync(taskAssignmentRequest.MentorId) == null || await _dbContext.LearningTasks.FindAsync(taskAssignmentRequest.LearningTaskId) == null )
            {
                return null;
            }
            TaskAssignment taskAssignment = new()
            {
                TraineeId = taskAssignmentRequest.TraineeId,
                MentorId = taskAssignmentRequest.MentorId,
                LearningTaskId = taskAssignmentRequest.LearningTaskId,
                AssignedDate = taskAssignmentRequest.AssignedDate,
                DueDate = taskAssignmentRequest.DueDate,
                TaskAssignmentStatus = taskAssignmentRequest.TaskAssignmentStatus,
                Remarks = taskAssignmentRequest.Remarks
            };

            _dbContext.TaskAssignments.Add(taskAssignment);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Create : New Task Assignment created with id {id} ", taskAssignment.Id);
            return MapToResponse(taskAssignment);
        }

        public async Task<TaskAssignmentResponse?> UpdateAsync(int id, UpdateTaskAssignmentRequest updateTaskAssignmentRequest)
        {
            TaskAssignment? taskAssignment = await _dbContext.TaskAssignments.FindAsync(id);
            if (taskAssignment is null)
            {
                _logger.LogError("Update : Task Assignment with id {id} Could not be found for updation", id);
                return null;
            }

            taskAssignment.TaskAssignmentStatus = updateTaskAssignmentRequest.TaskAssignmentStatus;
            await _dbContext.SaveChangesAsync();


            _logger.LogInformation("Update : Task Assignment with id {id} updated with status {status}", taskAssignment.Id, taskAssignment.TaskAssignmentStatus);
            return MapToResponse(taskAssignment);
        }


        public static TaskAssignmentResponse MapToResponse(TaskAssignment taskAssignment)
        {
            return new TaskAssignmentResponse
            {
                Id = taskAssignment.Id,
                TraineeId = taskAssignment.TraineeId,
                Trainee = taskAssignment.Trainee,
                MentorId = taskAssignment.MentorId,
                Mentor = taskAssignment.Mentor,
                LearningTaskId = taskAssignment.LearningTaskId,
                LearningTask = taskAssignment.LearningTask,
                AssignedDate = taskAssignment.AssignedDate,
                DueDate = taskAssignment.DueDate,
                TaskAssignmentStatus = taskAssignment.TaskAssignmentStatus,
                Remarks = taskAssignment.Remarks
            };
        }

        // public async Task<bool> DeleteAsync(int id)
        // {
        //     TaskAssignment? taskAssignment = await _dbContext.TaskAssignments.FindAsync(id);
        //     if (taskAssignment is null)
        //     {
        //         _logger.LogError("Delete : Task Assignment with id {id} could not be found for deletion", id);
        //         return false;
        //     }


        //     _dbContext.TaskAssignments.Remove(taskAssignment);
        //     await _dbContext.SaveChangesAsync();

        //     _logger.LogInformation("Delete : Task Assignment with id {id} deleted", id);
        //     return true;
        // }


        
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