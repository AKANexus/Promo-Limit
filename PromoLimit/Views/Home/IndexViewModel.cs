using PromoLimit.Models;

namespace PromoLimit.Views.Home
{
	public class IndexViewModel
	{
        public List<Produto> Produtos { get; set; }
        public string? ErrorMessage { get; set; }
	}
}
