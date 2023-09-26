using Microsoft.EntityFrameworkCore;
using PromoLimit.Contexts;
using PromoLimit.Models.Local;

namespace PromoLimit.Services
{
    public class ProdutoDataService
    {
	    private readonly IServiceProvider _provider;


	    public ProdutoDataService(IServiceProvider provider)
	    {
		    _provider = provider;
	    }
        
        public async Task<List<Produto>> GetAllProdutos(bool asNoTracking = true)
        {
	        using var scope = _provider.CreateScope();
            return asNoTracking switch
            {
                true => await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.AsNoTracking().OrderBy(x=>x.MLB).ToListAsync(),
                false => await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.OrderBy(x=>x.MLB).ToListAsync()
            };
        }

        public async Task<int> CountProdutos()
        {
	        using var scope = _provider.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.CountAsync();
        }

        public async Task<List<Produto>> GetAllProdutosPaged(int recordsPerPage = 15, int pageNumber = 1, string? query = null, bool asNoTracking = true)
        {
	        using var scope = _provider.CreateScope();
	        var request = scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.AsNoTracking();
	        if (query is not null)
	        {
		        request = request.Where(x => x.MLB.Contains(query));
	        }
	        return await request
		        .OrderBy(x => x.MLB)
		        .Skip((pageNumber - 1) * recordsPerPage)
		        .Take(recordsPerPage)
		        .ToListAsync();
        }

        public async Task<Produto> AddOrUpdate(Produto produto)
        {
	        using var scope = _provider.CreateScope();
            var tentativo = await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.FirstOrDefaultAsync(x => x.Id == produto.Id);
            if (tentativo == null)
            {
                scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.Update(produto);
            }
            else
            {
                tentativo.QuantidadeAVenda = produto.QuantidadeAVenda;
                tentativo.Estoque = produto.Estoque;
                scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.Update(tentativo);
            }
            await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
            return produto;
        }

        public async Task Delete(int id)
        {
	        using var scope = _provider.CreateScope();
            var tentativo = await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.FirstOrDefaultAsync(x => x.Id == id);
            if (tentativo is not null)
            {
                scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos.Remove(tentativo);
                await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
            }
        }

        public async Task<Produto?> GetByMlb(string mlb)
        {
	        using var scope = _provider.CreateScope();
	        return await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().Produtos!.AsNoTracking().FirstOrDefaultAsync(x => x.MLB == mlb);
        }
    }
}
