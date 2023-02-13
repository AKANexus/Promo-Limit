using PromoLimit.Models.Local;

namespace PromoLimit.Models.Bling
{
    public class BlingEstoqueCallback
	{
		[JsonPropertyName("retorno")]
		public RetornoBling Retorno { get; set; }
	}

	public class RetornoBling
	{
		[JsonPropertyName("estoques")]
		public List<EstoqueNode> Estoques { get; set; }
	}

	public class EstoqueNode
	{
		[JsonPropertyName("estoque")]
		public Produto Estoque { get; set; }
	}
}
