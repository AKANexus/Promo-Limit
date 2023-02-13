using Microsoft.EntityFrameworkCore;
using PromoLimit.Models.Local;

namespace PromoLimit.DbContext
{
    public class PromoLimitDbContext : Microsoft.EntityFrameworkCore.DbContext   
    {
        public PromoLimitDbContext()
        {
            
        }

        public PromoLimitDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MLInfo> MlInfos { get; set; }

        //public DbSet<ReservaBlingEntry> ReservaBlingEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
           
        }

    }
}
