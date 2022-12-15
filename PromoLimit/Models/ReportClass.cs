namespace PromoLimit.Models
{
	public class ReportClass
	{
		public ReportClass(string vendedor, string mlb, string descrição, string vendidos)
		{
			Vendedor = vendedor;
			Mlb = mlb;
			Descrição = descrição;
			Vendidos = vendidos;
		}

		public string Vendedor { get; set; }
		public string Mlb { get; set; }
		public string Descrição { get; set; }
		public string Vendidos { get; set; }
	}
}
