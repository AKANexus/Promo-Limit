using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoLimit.DbContext;
using PromoLimit.Models.Local;

namespace PromoLimit.Controllers
{
	public class DebugController : Controller
	{
		private PromoLimitDbContext _context;

		public DebugController(PromoLimitDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> GetAllMlInfo()
		{
			return Json(await _context.MlInfos.ToListAsync());
		}

		public async Task<IActionResult> GetAllProdutos()
		{
			return Json(await _context.Produtos.ToListAsync());
		}

		public async Task<IActionResult> GetAllUsers()
		{
			return Json(await _context.Users.ToListAsync());
		}

		public class Pessoa
		{
			public string name { get; set; }
			public int idade { get; set; }
			public string empresa { get; set; }
		}

		[HttpPost("testePost")]
		public async Task<IActionResult> TestePost([FromBody]Pessoa body)
		{
			return Ok();
		}
	}
}
