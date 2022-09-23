using Microsoft.EntityFrameworkCore;
using PromoLimit.DbContext;
using PromoLimit.Models;

namespace PromoLimit.Services
{
    public class ProdutoDataService
    {
        private readonly PromoLimitDbContext _context;

        public ProdutoDataService(IServiceProvider provider)
        {
            _context = provider.GetRequiredService<PromoLimitDbContext>();
        }


        public async Task<List<Produto>> GetAllProdutos()
        {
            return await _context.Produtos.OrderBy(x=>x.MLB).ToListAsync();
        }

        public async Task<Produto> AddOrUpdate(Produto produto)
        {
            var tentativo = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == produto.Id);
            if (tentativo == null)
            {
                _context.Produtos.Update(produto);
            }
            else
            {
                tentativo.QuantidadeAVenda = produto.QuantidadeAVenda;
                tentativo.Estoque = produto.Estoque;
                _context.Produtos.Update(tentativo);
            }
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task Delete(int id)
        {
            var tentativo = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
            if (tentativo is not null)
            {
                _context.Produtos.Remove(tentativo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
