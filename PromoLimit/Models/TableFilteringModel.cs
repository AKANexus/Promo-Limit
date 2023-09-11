namespace PromoLimit.Models
{
	public class TableFilteringModel
	{
		//public List<TableFilteringColumn>? columns { get; set; }
		//public List<TableFilteringOrder>? ordering { get; set; }
		public int recordsPerPage { get; set; }
		public int pageNumber { get; set; }
		public string? query { get; set; }
	}
}
