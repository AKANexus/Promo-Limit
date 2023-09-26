using Microsoft.EntityFrameworkCore;
using PromoLimit.Contexts;
using PromoLimit.Models.Local;

namespace PromoLimit.Services
{
    public class UserDataService
    {
        private readonly PromoLimitDbContext _context;

        public UserDataService(PromoLimitDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUuid(Guid uuid)
        {
            return await _context.Users!.FirstOrDefaultAsync(x => x.Uuid == uuid);
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _context.Users!
                .FirstOrDefaultAsync(x => x.Login == username);
        }

        public async Task<User> AddOrUpdateUser(User funcionário)
        {
            _context.Update(funcionário);
            await _context.SaveChangesAsync();
            return funcionário;
        }
    }
}
