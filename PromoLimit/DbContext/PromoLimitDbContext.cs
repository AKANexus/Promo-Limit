using Microsoft.EntityFrameworkCore;
using PromoLimit.Models;

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
        //public DbSet<ParidadeBlingMLB> Paridades { get; set; }
        public DbSet<MLInfo> MlInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            //List<User> userSeed = new()
            //{
            //    new() {Ativo = true, Nome = "Admin", Login = "admin", Uuid = new Guid("f13b8527-a805-4393-9e62-199f5b775b62"), Auth = ""}
            //};
            //mb.Entity<User>().HasData(userSeed);
            //mb.Entity<Produto>()
            //    .HasMany(p => p.MLBs)
            //    .WithOne(p => p.Produto);
        }

    }
}
