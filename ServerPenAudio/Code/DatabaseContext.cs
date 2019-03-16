using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ServerPenAudio.Code
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Password { get; set; }
    }
}