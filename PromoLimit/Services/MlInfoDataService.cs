using Microsoft.EntityFrameworkCore;
using PromoLimit.Contexts;
using PromoLimit.Models.Local;

namespace PromoLimit.Services
{
    public class MlInfoDataService
    {
	    private readonly IServiceProvider _provider;


	    public MlInfoDataService(IServiceProvider provider)
	    {
		    _provider = provider;
	    }

        public async Task<List<MLInfo>> GetAll()
        {
	        using var scope = _provider.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().MlInfos.AsNoTracking().ToListAsync();
        }
        public async Task<MLInfo?> GetByUserIdAsNoTracking(ulong userId)
        {
	        using var scope = _provider.CreateScope();
            return /*true switch
            {
                true => */
                await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().MlInfos.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);//,
                //false => await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().MlInfos.FirstOrDefaultAsync(x => x.UserId == userId)
            //};
        }

        public async Task DeleteInfo(ulong userId)
        {
	        using var scope = _provider.CreateScope();
            var tentative = await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().MlInfos.FirstOrDefaultAsync(x => x.UserId == userId);
            if (tentative != null)
            {
                scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Remove(tentative);
                await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
            }
        }

        public async Task AddOrUpdateMlInfo(MLInfo info)
        {
	        using var scope = _provider.CreateScope();
            var tentative = await GetByUserIdAsNoTracking(info.UserId);
            if (tentative is not null)
            {
                info.Id = tentative.Id;
            }
            scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().MlInfos.Update(info);
            await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
        }
    }
}
