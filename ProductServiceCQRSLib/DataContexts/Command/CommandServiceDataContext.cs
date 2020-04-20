using Microsoft.EntityFrameworkCore;
using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.DataContexts.Command
{
    public class CommandServiceDataContext : DbContext
    {
        public CommandServiceDataContext(DbContextOptions<CommandServiceDataContext> options)
            : base(options)
        {  }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EventEntityTypeConfiguration());
        }
    }
}