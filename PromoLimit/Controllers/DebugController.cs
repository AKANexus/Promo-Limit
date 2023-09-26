using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;
using PromoLimit.Contexts;
using PromoLimit.Models.Local;
using PromoLimit.Services;

namespace PromoLimit.Controllers
{
	[Route("debug")]
	public class DebugController : Controller
	{
		private PromoLimitDbContext _context;
        private readonly TinyApi _tinyApiService;

        public DebugController(PromoLimitDbContext context, IServiceProvider provider)
		{
			_context = context;
            _tinyApiService = provider.GetRequiredService<TinyApi>();
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

        [HttpGet("getTinyStock")]
        public async Task<IActionResult> GetTinyStock()
        {
            var tinyTentative = await _tinyApiService.ProcuraEstoquePorCodigo("7898181810593-300");
            if (tinyTentative is not null)
            {
                //await _logger.LogInformation($"Existe um produto no Tiny com esse SKU ({sku.ValueName}). Vamos verificar o estoque", $"RunCallbackChecks ({operation})");

                var saldo = (int?)tinyTentative["saldo"];
                //int.TryParse(saldo, out int saldoInt);
                //prodTentativo.Estoque = saldoInt;
            }

            return Ok();
        }
	}
}
