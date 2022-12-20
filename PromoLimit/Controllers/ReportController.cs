using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using PromoLimit.Models;
using PromoLimit.Services;

namespace PromoLimit.Controllers
{
	public class ReportController : Controller
	{
		private MlInfoDataService _mlDataService;
		private MlApiService _mlApiService;
		private ProdutoDataService _produtoDataService;

		public ReportController(IServiceProvider provider)
		{
			_mlDataService = provider.GetRequiredService<MlInfoDataService>();
			_mlApiService = provider.GetRequiredService<MlApiService>();
			_produtoDataService = provider.GetRequiredService<ProdutoDataService>();
		}
		public IActionResult Index()
		{
			return View();
		}

		private CultureInfo ptBR = new CultureInfo("pt-BR");

		public async Task<IActionResult> GerarRelatorio(string? dataInicio, string? dataFim)
		{

			var listOfSellers = await _mlDataService.GetAll();
			if (listOfSellers.Count == 0)
			{
				return Ok("No sellers found");
			}

			if (!DateTime.TryParseExact(dataInicio,"yyyy-MM-dd" , ptBR, DateTimeStyles.None, out var startDate) ||
				!DateTime.TryParseExact(dataFim, "yyyy-MM-dd", ptBR, DateTimeStyles.None, out var endDate))
			{
				return BadRequest($"As datas informadas {dataInicio} e {dataFim} não foram reconhecidas");
			}
			List<(MlOrder, MLInfo)> orders = new();
			foreach (var seller in listOfSellers)
			{
				var mlOrders = await _mlApiService.GetOrdersBetweenDates(seller.UserId, startDate, endDate.AddDays(1));
				if (mlOrders.Item1 && mlOrders.Item2 is not null)
				{
					orders.AddRange(mlOrders.Item2.Select(mlOrder => (mlOrder, seller)));
				}
			}

			if (orders.Count <= 0) return Ok("No orders found");
			List<ReportClass> firstList = new();

			foreach (var tuple in orders)
			{
				foreach (var orderItem in tuple.Item1.OrderItems)
				{
					var tentativo = await _produtoDataService.GetByMlb(orderItem.Item.Id);
					if (tentativo != null)
					{
						firstList.Add(new ReportClass(tuple.Item2.Vendedor,
							tentativo.MLB,
							tentativo.Descricao,
							orderItem.Quantity));
					}
				}
			}

			List<ReportClass> reportItself = firstList.GroupBy(x => x.Mlb).Select(grouping => new ReportClass(grouping.First()
						.Vendedor,
					grouping.Key,
					grouping.First()
						.Descrição,
					grouping.Sum(x => x.Vendidos)))
				.ToList();

			return Json(reportItself.OrderByDescending(x=>x.Vendidos).ThenBy(x=>x.Descrição));
		}
	}
}
