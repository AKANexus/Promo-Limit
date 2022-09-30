using System.Runtime.CompilerServices;
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

        public async Task<List<MLInfo>> GetAll()
        {
            return await _context.MlInfos.AsNoTracking().ToListAsync();
        }
        public async Task<MLInfo?> GetByUserIdAsNoTracking(int userId)
        {
            return true switch
            {
                true => await _context.MlInfos.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId),
                false => await _context.MlInfos.FirstOrDefaultAsync(x => x.UserId == userId)
            };
        }

        public async Task DeleteInfo(int userId)
        {
            var tentative = await _context.MlInfos.FirstOrDefaultAsync(x => x.UserId == userId);
            if (tentative != null)
            {
                _context.Remove(tentative);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddOrUpdateMlInfo(MLInfo info)
        {
            var tentative = await GetByUserIdAsNoTracking(info.UserId);
            if (tentative is not null)
            {
                info.Id = tentative.Id;
            }
            _context.MlInfos.Update(info);
            await _context.SaveChangesAsync();
        }
    }
}
