using Microsoft.EntityFrameworkCore;
using Models;

namespace TraineeManagementApi.Models
{
class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Trainee> Trainees { get; set; }
    public DbSet<User> Users { get; set; }

}    
}
