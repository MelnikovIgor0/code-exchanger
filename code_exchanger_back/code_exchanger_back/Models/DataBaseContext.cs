using Microsoft.EntityFrameworkCore;
using code_exchanger_back.Models;

namespace code_exchanger_back.Models
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
            base.Update(this.Database);
        }

        public DbSet<Users> users { get; set; }

        public DbSet<Content> content { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}