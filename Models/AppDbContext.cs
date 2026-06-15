using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Models;

namespace TraineeManagementApi.Models
{
    class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<LearningTask> LearningTasks { get; set; }

        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Review> Reviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trainee>()
            .HasMany(e => e.TaskAssignments)
            .WithOne(e => e.Trainee)
            .HasForeignKey(e => e.TraineeId)
            .IsRequired();

            modelBuilder.Entity<Mentor>()
            .HasMany(e => e.TaskAssignments)
            .WithOne(e => e.Mentor)
            .HasForeignKey(e => e.MentorId)
            .IsRequired();

            modelBuilder.Entity<LearningTask>()
            .HasMany(e => e.TaskAssignments)
            .WithOne(e => e.LearningTask)
            .HasForeignKey(e => e.LearningTaskId)
            .IsRequired();

            // modelBuilder.Entity<Submission>()
            // .HasOne(e => e.TaskAssignment)
            // .WithMany(e => e.Submissions)
            // .HasForeignKey(e => e.TaskAssignmentId)
            // .IsRequired();
            
            // modelBuilder.Entity<Review>()
            // .HasOne(e => e.Mentor)
            // .WithMany(e => e.Reviews)
            // .HasForeignKey(e => e.MentorId)
            // .IsRequired();

            // modelBuilder.Entity<TaskAssignment>(entity =>
            // {
            //     entity.HasOne<Trainee>(ta => ta.Trainee)
            //     .WithMany(t => t.TaskAssignments)
            //     .HasForeignKey(ta => ta.TraineeId)
            //     .IsRequired();

            //     entity.HasOne<Mentor>(ta => ta.Mentor)
            //     .WithMany(m => m.TaskAssignments)
            //     .HasForeignKey(ta => ta.MentorId)
            //     .IsRequired();

            //     entity.HasOne<LearningTask>(ta => ta.LearningTask)
            //     .WithMany(lt => lt.TaskAssignments)
            //     .HasForeignKey(ta => ta.LearningTaskId)
            //     .IsRequired();
            // });



        }
    }
}
