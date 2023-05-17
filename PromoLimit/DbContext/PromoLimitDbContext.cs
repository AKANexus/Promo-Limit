using Microsoft.EntityFrameworkCore;
using PromoLimit.Models;
using PromoLimit.Models.Local;

namespace PromoLimit.DbContext
{
	public class PromoLimitDbContext : Microsoft.EntityFrameworkCore.DbContext
	{
		public PromoLimitDbContext()
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}

		public PromoLimitDbContext(DbContextOptions<PromoLimitDbContext> options) : base(options)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Produto> Produtos { get; set; }
		public DbSet<MLInfo> MlInfos { get; set; }
		public DbSet<PromoLog> PromoLogs { get; set; }

	}
}
