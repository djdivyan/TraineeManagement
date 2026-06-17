using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
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

        public async Task<TaskAssignmentResponse> GetByIdAsync(int id)
        {
            TaskAssignment? taskAssignment = await _dbContext.TaskAssignments.FindAsync(id);
            if (taskAssignment is null)
            {
                _logger.LogError("GetByID : Task Assignments Not found with {id}", id);
                throw new NotFoundException("Task Assignment",id);
            }
            _logger.LogInformation("GetByID : Task Assignments found with {id}", id);
            return MapToResponse(taskAssignment);
        }

        public async Task<TaskAssignmentResponse> CreateAsync(TaskAssignmentRequest taskAssignmentRequest)
        {
            if (await _dbContext.Trainees.FindAsync(taskAssignmentRequest.TraineeId) == null || await _dbContext.Mentors.FindAsync(taskAssignmentRequest.MentorId) == null)
            {
                throw new BadRequestException($"Foreign Key - TraineeId : {taskAssignmentRequest.TraineeId} Does not Exist");
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

            _logger.LogInformation("Create : New Task Assignment created with id {id} ", taskAssignment.Id);
            return MapToResponse(taskAssignment);
        }

        public async Task<TaskAssignmentResponse> UpdateAsync(int id, UpdateTaskAssignmentRequest updateTaskAssignmentRequest)
        {
            TaskAssignment? taskAssignment = await _dbContext.TaskAssignments.FindAsync(id);
            if (taskAssignment is null)
            {
                _logger.LogError("Update : Task Assignment with id {id} Could not be found for updation", id);
                throw new NotFoundException("Task Assignment",id);
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
    }
}