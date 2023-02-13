namespace PromoLimit.Models.Bling
{
	public class VariaçãoBling
	{
		[JsonPropertyName("nome")]
		public string Nome { get; set; }

		[JsonPropertyName("codigo")]
		public string Codigo { get; set; }
	}

	public class VariaçãoNode
	{
		[JsonPropertyName("variacao")]
		public VariaçãoBling VariaçãoBling { get; set; }
	}
}
