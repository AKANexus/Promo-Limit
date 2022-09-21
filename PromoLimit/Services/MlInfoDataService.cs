using Microsoft.EntityFrameworkCore;
using PromoLimit.DbContext;
using PromoLimit.Models;

namespace PromoLimit.Services
{
	public class MlInfoDataService
	{
        private readonly PromoLimitDbContext _context;

        public MlInfoDataService(PromoLimitDbContext context)
        {
            _context = context;
        }

        public async Task<MLInfo?> GetByUserId(int userId)
        {
            return await _context.MlInfos.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task AddOrUpdateMlInfo(MLInfo info)
        {
            var tentative = await GetByUserId(info.UserId);
            if (tentative is not null)
            {
                info.Id = tentative.Id;
            }
            _context.MlInfos.Update(info);
            await _context.SaveChangesAsync();
        }
	}
}
