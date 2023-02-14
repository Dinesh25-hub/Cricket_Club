using Cricket_Club.Models;
using Microsoft.EntityFrameworkCore;

public class DataContext:DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Cricketer> cricketer { get; set; }
   
    public DbSet<Credential> credential{ get; set; }
}