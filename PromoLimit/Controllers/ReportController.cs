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


		public async Task<IActionResult> GerarRelatorio(string? dataInicio, string? dataFim)
		{

			var listOfSellers = await _mlDataService.GetAll();
			if (listOfSellers.Count == 0)
			{
				return NoContent();
			}

			if (!DateTime.TryParse(dataInicio, out var startDate) ||
				!DateTime.TryParse(dataFim, out var endDate))
			{
				return BadRequest();
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

			if (orders.Count <= 0) return NoContent();
			List<ReportClass> reportItself = new();

			foreach (var tuple in orders)
			{
				foreach (var orderItem in tuple.Item1.OrderItems)
				{
					var tentativo = await _produtoDataService.GetByMlb(orderItem.Item.Id);
					if (tentativo != null)
					{
						reportItself.Add(new ReportClass(tuple.Item2.Vendedor,
							tentativo.MLB,
							tentativo.Descricao,
							orderItem.Quantity.ToString()));
					}
				}
			}


			return Json(reportItself);
		}
	}
}
