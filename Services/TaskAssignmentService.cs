using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;
using TraineeManagementApi.Utilities;

namespace TraineeManagementApi.Services
{
    class TaskAssignmentService(AppDbContext dbContext, ILogger<TaskAssignmentService> logger, IDistributedCache distributedCache) : ITaskAssignmentService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<TaskAssignmentService> _logger = logger;
        private readonly IDistributedCache _cache = distributedCache;
        private const string AllTaskAssignmentCacheKey = "task_assignment";

        public async Task<List<TaskAssignmentResponse>> GetAllAsync()
        {
            _logger.LogInformation("GetAllAsync:TaskAssignment : Entering the Function");
            IQueryable<TaskAssignment> query = _dbContext.TaskAssignments.AsNoTracking();
                                                        // .Include(ta => ta.Trainee)
                                                        // .Include(ta => ta.Mentor)
                                                        // .Include(ta => ta.LearningTask);

            List<TaskAssignment> taskAssignments = await query.ToListAsync();
            _logger.LogInformation("GetAllAsync:TaskAssignment : Successfully returned all Task Assignments");
            return taskAssignments.Select(MapToResponse).ToList();
        }

        public async Task<TaskAssignmentResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GetByIdAsync:TaskAssignment : Entering the Function");

            TaskAssignment? taskAssignment;
            //cache
            try
            {
                string cacheKey = $"task_assignment:{id}";
                _logger.LogInformation("Fetching data for key: {CacheKey}.", cacheKey);
                
                taskAssignment = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from database.", cacheKey);
                    return await _dbContext.TaskAssignments.AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
                },
                cancellationToken: cancellationToken,
                logger: _logger);

                if (taskAssignment != null)
                {
                    return MapToResponse(taskAssignment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis is unavailable, Fetching from Database {ex}",ex);
            }
            
            taskAssignment = await _dbContext.TaskAssignments.FindAsync(id);

            if (taskAssignment is null)
            {
                _logger.LogError("GetByIdAsync:TaskAssignment : Task Assignments Not found with {id}", id);
                throw new NotFoundException("Task Assignment",id);
            }
            _logger.LogInformation("GetByIdAsync:TaskAssignment : Task Assignments found with {id}", id);
            return MapToResponse(taskAssignment);
        }

        public async Task<TaskAssignmentResponse> CreateAsync(TaskAssignmentRequest taskAssignmentRequest)
        {
            _logger.LogInformation("CreateAsync:TaskAssignment : Entering the Function");

            if (await _dbContext.Trainees.FindAsync(taskAssignmentRequest.TraineeId) == null)
            {
                throw new BadRequestException($"Foreign Key - TraineeId : {taskAssignmentRequest.TraineeId} Does not Exist");
            }
            if (await _dbContext.Mentors.FindAsync(taskAssignmentRequest.MentorId) == null)
            {
                throw new BadRequestException($"Foreign Key - MentorId : {taskAssignmentRequest.MentorId} Does not Exist");
            }
            if (await _dbContext.LearningTasks.FindAsync(taskAssignmentRequest.LearningTaskId) == null)
            {
                throw new BadRequestException($"Foreign Key - LearningTaskId : {taskAssignmentRequest.LearningTaskId} Does not Exist");                
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

            _logger.LogInformation("CreateAsync:TaskAssignment : New Task Assignment created with id {id} ", taskAssignment.Id);
            return MapToResponse(taskAssignment);
        }

        public async Task<TaskAssignmentResponse> UpdateAsync(int id, UpdateTaskAssignmentRequest updateTaskAssignmentRequest, CancellationToken cancellationToken=default)
        {
            _logger.LogInformation("UpdateAsync:TaskAssignment : Entering the Function");
            TaskAssignment? taskAssignment = await _dbContext.TaskAssignments.FindAsync(id);
            if (taskAssignment is null)
            {
                _logger.LogError("UpdateAsync:TaskAssignment : Task Assignment with id {id} Could not be found for updation", id);
                throw new NotFoundException("Task Assignment",id);
            }

            taskAssignment.TaskAssignmentStatus = updateTaskAssignmentRequest.TaskAssignmentStatus;
            await _dbContext.SaveChangesAsync();


            //Cache
            try
            {
                _logger.LogInformation("UpdateAsync:TaskAssignment : Task Assignment Cache with id {id} updated with status {status}", taskAssignment.Id, taskAssignment.TaskAssignmentStatus);
                string cacheKey = $"task_assignment:{id}";
                await _cache.SetAsync(cacheKey,taskAssignment ,cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Redis is unavailable, Fetching from Database {ex}",ex);
            }

           

            _logger.LogInformation("UpdateAsync:TaskAssignment : Task Assignment with id {id} updated with status {status}", taskAssignment.Id, taskAssignment.TaskAssignmentStatus);
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
    }
}