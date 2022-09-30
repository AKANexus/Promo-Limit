using PromoLimit.Models;

namespace PromoLimit.Views.Home
{
	public class IndexViewModel
	{
        public List<MLInfo> MlInfos { get; set; }
        public List<Produto> Produtos { get; set; }
        public string? ErrorMessage { get; set; }
        public string LastUpdate => DateTime.Now.ToShortTimeString();
    }
}
