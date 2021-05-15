using Microsoft.EntityFrameworkCore;

namespace AgentApi.Models
{
    public class AgentContext : DbContext
    {
        public AgentContext(DbContextOptions<AgentContext> options)
            : base(options)
        {
        }

        public DbSet<AgentItem> AgentItems { get; set; }
    }

}