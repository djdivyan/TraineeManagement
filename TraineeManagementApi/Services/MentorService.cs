using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.Services
{
    class MentorService(AppDbContext dbContext, ILogger<MentorService> logger) : IMentorService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<MentorService> _logger = logger;
        
        public async Task<List<MentorResponse>> GetAllAsync(string? search)
        {
            _logger.LogInformation("GetAllAsync:Mentor : Entering the Function");
            IQueryable<Mentor> query = _dbContext.Mentors.AsQueryable();
            if(!string.IsNullOrEmpty(search))
                query = query.Where( m =>  
                    m.FirstName.Contains(search) || 
                    m.LastName.Contains(search) || 
                    m.Email.Contains(search) || 
                    m.Expertise.Contains(search)
                );
        
            List<Mentor> mentors = await query.ToListAsync();
            _logger.LogInformation("GetAllAsync:Mentor : Successfully returned mentors");
            return mentors.Select(MapToResponse).ToList();
        }

        public async Task<MentorResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation("GetByIdAsync:Mentor : Entering the Function");
            Mentor? mentor = await _dbContext.Mentors.FindAsync(id);
            if (mentor is null)
            {
                _logger.LogError("GetByIdAsync:Mentor : Mentor Not found with {id}", id);
                throw new NotFoundException("Mentor",id);
            }
            _logger.LogInformation("GetByIdAsync:Mentor : Mentor found with {id}", id);
            return MapToResponse(mentor);
        }

        public async Task<MentorResponse> CreateAsync(MentorRequest mentorRequest)
        {
            _logger.LogInformation("CreateAsync:Mentor : Entering the Function");
            Mentor mentor = new()
            {
                FirstName = mentorRequest.FirstName,
                LastName = mentorRequest.LastName,
                Email = mentorRequest.Email, 
                Expertise = mentorRequest.Expertise, 
                MentorStatus = mentorRequest.MentorStatus,
                CreatedDate = DateTime.Now, 
                UpdatedDate = DateTime.Now
            };

            _dbContext.Mentors.Add(mentor);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("CreateAsync:Mentor : New Mentor created with id {id} at {DateTime}",mentor.Id, mentor.CreatedDate);
            return MapToResponse(mentor);
        }

        public async Task<MentorResponse?> UpdateAsync(int id, UpdateMentorRequest updateMentoreRequest)
        {
            _logger.LogInformation("UpdateAsync:Mentor : Entering the Function");
            Mentor? mentor = await _dbContext.Mentors.FindAsync(id);
            if(mentor is null)
            {
                _logger.LogError("UpdateAsync:Mentor : Mentor with id {id} Could not be found for updation",id);
                throw new NotFoundException("Mentor",id);
            } 
            mentor.FirstName = updateMentoreRequest.FirstName;
            mentor.LastName = updateMentoreRequest.LastName;
            mentor.Email = updateMentoreRequest.Email;
            mentor.Expertise = updateMentoreRequest.Expertise; 
            mentor.MentorStatus = updateMentoreRequest.MentorStatus;
            mentor.UpdatedDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("UpdateAsync:Mentor : Mentor with id {id} updated at {DateTime}",mentor.Id,mentor.UpdatedDate);
            return MapToResponse(mentor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("DeleteAsync:Mentor : Entering the Function");
            Mentor? mentor = await _dbContext.Mentors.FindAsync(id);
            if(mentor is null)
            {
                _logger.LogError("DeleteAsync:Mentor : Mentor with id {id} could not be found for deletion",id);
                throw new NotFoundException("Mentor",id);
            }

            _dbContext.Mentors.Remove(mentor);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("DeleteAsync:Mentor : Mentor with id {id} deleted",id);
            return true;
        }

         public static MentorResponse MapToResponse(Mentor mentor)
        {
            return new MentorResponse
            {
                Id = mentor.Id,
                FirstName = mentor.FirstName,
                LastName = mentor.LastName,
                Email = mentor.Email, 
                Expertise = mentor.Expertise, 
                MentorStatus = mentor.MentorStatus,
                CreatedDate = mentor.CreatedDate, 
                UpdatedDate = mentor.UpdatedDate
            };
        }
    }
}