using System.Text.Json.Serialization;

namespace PromoLimit.Models
{
	public class ReportClass
	{
		public ReportClass(string vendedor, string mlb, string descrição, int vendidos)
		{
			Vendedor = vendedor;
			Mlb = mlb;
			Descrição = descrição;
			Vendidos = vendidos;
		}

		public string Vendedor { get; set; }
		public string Mlb { get; set; }
		[JsonPropertyName("descricao")]
		public string Descrição { get; set; }
		public int Vendidos { get; set; }
	}
}
