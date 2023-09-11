using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PromoLimit.Models.Bling;

namespace PromoLimit.Controllers
{
	public class ReservaController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public event CallbackEventHandler CallbackEvent;

		public delegate void CallbackEventHandler(object sender, CallbackEventArgs args);

		public async Task<IActionResult> BlingCallBack(BlingEstoqueCallback notification)
		{
			return Ok();
		}
	}
}
