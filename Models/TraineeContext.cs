using Microsoft.EntityFrameworkCore;
using Models;

namespace TraineeManagementApi.Models
{
class TraineeContext : DbContext
{
    public TraineeContext(DbContextOptions<TraineeContext> options) : base(options) { }

    public DbSet<Trainee> Trainees { get; set; }
}    
}
