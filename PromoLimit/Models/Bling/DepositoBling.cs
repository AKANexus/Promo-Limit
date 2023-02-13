namespace PromoLimit.Models.Bling
{
	public class DepositoBling
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("nome")]
		public string Nome { get; set; }

		[JsonPropertyName("saldo")]
		public object Saldo { get; set; }

		[JsonPropertyName("desconsiderar")]
		public string Desconsiderar { get; set; }

		[JsonPropertyName("saldoVirtual")]
		public object SaldoVirtual { get; set; }
	}

	public class DepositoNode
	{
		[JsonPropertyName("deposito")]
		public DepositoBling Deposito { get; set; }
	}
}
