using Microsoft.EntityFrameworkCore;
using PromoLimit.Models;
using PromoLimit.Models.Local;

namespace PromoLimit.Contexts
{
	public class PromoLimitDbContext : DbContext
	{
		public PromoLimitDbContext()
		{
			//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}

		public PromoLimitDbContext(DbContextOptions<PromoLimitDbContext> options) : base(options)
		{
			//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Produto> Produtos { get; set; } = null!;
        public DbSet<MLInfo> MlInfos { get; set; } = null!;
        public DbSet<PromoLog> PromoLogs { get; set; } = null!;

    }
}
