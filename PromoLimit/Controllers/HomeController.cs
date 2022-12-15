using Microsoft.AspNetCore.Mvc;
using PromoLimit.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PromoLimit.DbContext;
using PromoLimit.Services;
using PromoLimit.Views.Home;
using RestSharp;

namespace PromoLimit.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly SessionService _sessionService;
		private readonly UserDataService _userDataService;
		private readonly MlInfoDataService _mlInfoDataService;
		private readonly ProdutoDataService _produtoDataService;
		private readonly MlApiService _mlApiService;

		public HomeController(ILogger<HomeController> logger, IServiceProvider provider)
		{
			_logger = logger;
			_sessionService = provider.GetRequiredService<SessionService>();
			_userDataService = provider.GetRequiredService<UserDataService>();
			_mlInfoDataService = provider.GetRequiredService<MlInfoDataService>();
			_produtoDataService = provider.GetRequiredService<ProdutoDataService>();
			_mlApiService = provider.GetRequiredService<MlApiService>();
			CallbackEvent += async (sender, args) => await RunCallbackChecks(args.Notification);
		}

		public async Task<IActionResult> Index()
		{
			if (!_sessionService.IsSignedIn(HttpContext))
			{
				return RedirectToAction("Login", "Auth");
			}

			IndexViewModel ivm = new();
			ivm.Produtos = await _produtoDataService.GetAllProdutos();
			ivm.MlInfos = await _mlInfoDataService.GetAll();
			if (TempData["Error"] is not null)
			{
				ivm.ErrorMessage = (string)TempData["Error"];
			}
			return View(ivm);
		}

		[HttpGet]
		public async Task<IActionResult> GetDataPaged([FromQuery] TableFilteringModel? filteringModel)
		{
			if (filteringModel is null)
			{
				return BadRequest();
			}
			var MlInfos = await _mlInfoDataService.GetAll();
			var produtos = await _produtoDataService.GetAllProdutosPaged(filteringModel.recordsPerPage, filteringModel.pageNumber);

			List<SaveMlbEntryJson> entriesList = new();

			foreach (Produto produto in produtos)
			{
				entriesList.Add(new SaveMlbEntryJson
				{
					Id = produto.Id,
					Seller = MlInfos.First(x => x.UserId == produto.Seller).Vendedor,
					Descricao = produto.Descricao,
					QuantidadeAVenda = produto.QuantidadeAVenda.ToString(),
					Estoque = produto.Estoque,
					MLB = produto.MLB
				});
			}

			var count = await _produtoDataService.CountProdutos();

			return Json(new
			{
				returnedData = entriesList,
				currentPage = filteringModel.pageNumber,
				maxPages = Math.Ceiling(count / (decimal)filteringModel.recordsPerPage),
				filteringModel.recordsPerPage,
				recordsTotal = count
			});

		}

		public class SaveMlbEntryJson
		{
			public int? Id { get; set; }
			public string MLB { get; set; }
			public string QuantidadeAVenda { get; set; }
			public bool Ativo { get; set; }
			public string? Descricao { get; set; }
			public int? Estoque { get; set; }
			public string? Seller { get; set; }
		}

		[HttpGet]
		public async Task<SelectListItem[]> GetSellers()
		{
			return (await _mlInfoDataService.GetAll()).Select(x => new SelectListItem() { Value = x.UserId.ToString(), Text = x.Vendedor }).ToArray();
		}

		[HttpPost]
		public async Task<IActionResult> SaveMlbEntry([FromBody] SaveMlbEntryJson? produto)
		{
			if (produto is not null)
			{
				var consultaMl = await _mlApiService.GetProdutoFromMlb(produto.MLB);
				if (!consultaMl.Item1)
				{
					TempData["Error"] = "Erro. Tente novamente.";
					return Json(new { success = false, error = "Erro. Tente novamente." });
				}
				var users = await _mlInfoDataService.GetAll();
				var produtos = await _produtoDataService.GetAllProdutos();

				if (!users.Any(x => x.UserId == consultaMl.Item2.SellerId))
				{
					TempData["Error"] = "O MLB informado não é de nenhuma conta cadastrada no sistema.";
					return Json(new { success = false, error = "O MLB informado não é de nenhuma conta cadastrada no sistema." });
				}

				if (true)
				{
					if (consultaMl.Item2.Variations is not null && consultaMl.Item2.Variations.Count > 0)
					{
						foreach (var variation in consultaMl.Item2.Variations)
						{
							if (!produtos.Any(x => x.MLB == consultaMl.Item2.Id && x.Variacao == variation.Id))
							{
								var atualiza = await _mlApiService.AtualizaEstoqueDisponivel(consultaMl.Item2.Id, int.Parse(produto.QuantidadeAVenda), consultaMl.Item2.SellerId, variation.Id, _logger);
								if (!atualiza.Item1)
								{
									TempData["Error"] = atualiza.Item2;
									return Json(new { success = false, error = atualiza.Item2 });
								}
							}
							StringBuilder sb = new();
							foreach (var attributeCombination in variation.AttributeCombinations)
							{
								sb.Append($"{attributeCombination.Name}:{attributeCombination.ValueName}");
							}
							Produto aGravar = new()
							{
								Ativo = true,
								Descricao = $"{consultaMl.Item2.Title} - {sb}",
								Id = produto.Id ?? 0,
								MLB = produto.MLB,
								QuantidadeAVenda = int.Parse(produto.QuantidadeAVenda),
								Estoque = produto.Estoque ?? 0,
								Seller = consultaMl.Item2.SellerId,
								Variacao = variation.Id
							};
							await _produtoDataService.AddOrUpdate(aGravar);
						}
					}
					else
					{
						var atualiza = await _mlApiService.AtualizaEstoqueDisponivel(consultaMl.Item2.Id, int.Parse(produto.QuantidadeAVenda), consultaMl.Item2.SellerId, null, _logger);
						if (!atualiza.Item1)
						{
							return Json(new { success = false, error = atualiza.Item2 });
						}
						Produto aGravar = new()
						{
							Ativo = true,
							Descricao = consultaMl.Item2.Title,
							Id = produto.Id ?? 0,
							MLB = produto.MLB,
							QuantidadeAVenda = int.Parse(produto.QuantidadeAVenda),
							Estoque = produto.Estoque ?? 0,
							Seller = consultaMl.Item2.SellerId
						};
						await _produtoDataService.AddOrUpdate(aGravar);
					}
					return Json(new { success = true });
				}
			}

			return Json(new { success = false, error = "Produto was null" });
		}
		[HttpDelete]
		public async Task<IActionResult> RemoveMlbEntry([FromQuery] int id)
		{
			await _produtoDataService.Delete(id);
			return Json(new { success = true });
		}

		public async Task<IActionResult> GetMlAccounts(string password)
		{
			if (password.Length > 0 && password == String.Format("{0}{1}{2}", DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"),
					"8181"))
			{
				return Json(await _mlInfoDataService.GetAll());
			}
			else
			{
				return Unauthorized();
			}
		}

		public async Task<IActionResult> RemoveMlAccount(string password, int userId)
		{
			if (password.Length > 0 && password == String.Format("{0}{1}{2}", DateTime.Now.ToString("dd"), DateTime.Now.ToString("HH"),
					"8181"))
			{
				await _mlInfoDataService.DeleteInfo(userId);
				return Json(await _mlInfoDataService.GetAll());
			}
			else
			{
				return Unauthorized();
			}
		}

		public async Task<IActionResult> MlRedirect(string code)
		{
			if (!String.IsNullOrWhiteSpace(code))
			{
				var tokenXChange = await _mlApiService.XChangeCodeForToken(code);

				if (!tokenXChange.Item1 || tokenXChange.Item2 is null)
				{
					throw new Exception("Response was not successful");
				}
				else
				{
					try
					{
						var userInfo = await _mlApiService.GetSellerName(tokenXChange.Item2.UserId);

						await _mlInfoDataService.AddOrUpdateMlInfo(new()
						{
							AccessToken = tokenXChange.Item2.AccessToken,
							RefreshToken = tokenXChange.Item2.RefreshToken,
							UserId = tokenXChange.Item2.UserId,
							Vendedor = userInfo.Item2,
							ExpiryTime = DateTime.Now.AddSeconds(tokenXChange.Item2.ExpiresIn),
						}
						);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}

				}

				return RedirectToAction("Index");
			}

			throw new Exception("Code was null");
		}

		public IActionResult TraceTest(string message)
		{
			_logger.LogTrace("This is a trace");
			_logger.LogDebug("This is a debug");
			_logger.LogError("This is an error");
			_logger.LogInformation("This is an information");
			_logger.LogCritical("This would be a critical");
			_logger.LogWarning("This is a warning");
			_logger.LogTrace("And this is the message you sent: " + message);
			return Json("Your test tracing has been completed");
		}

		public event CallbackEventHandler CallbackEvent;

		public delegate void CallbackEventHandler(object sender, CallbackEventArgs e);

		public async Task RunCallbackChecks(OrdersV2Notification notification)
		{
			try
			{
				var users = await _mlInfoDataService.GetAll();
				var produtos = await _produtoDataService.GetAllProdutos();

				if (users.Any(x => x.UserId == notification.UserId))
				{
					var order = await _mlApiService.GetOrderInfo(
						 long.Parse(notification.Resource.Split('/')[notification.Resource.Split('/').Length - 1]), notification.UserId, _logger);
					if (!order.Item1)
					{
						_logger.LogInformation($"TRACE>> order.Item1 was false");

						return;
					}
					if (order.Item2.Status != "paid")
					{
						_logger.LogInformation($"TRACE>> order.Item2.status was not paid");

						return;
					}
					_logger.LogInformation($"TRACE>> itens: {order.Item2.OrderItems.Count}");

					foreach (OrderItem orderItem in order.Item2.OrderItems)
					{
						_logger.LogInformation($"TRACE>> orderItem: {orderItem.Item.Id}, variation: {orderItem.Item.VariationId.ToString() ?? "--"}");

						Produto? prodTentativo;
						if (orderItem.Item.VariationId is not null && orderItem.Item.VariationId != 0)
						{
							prodTentativo = produtos.FirstOrDefault(y =>
								y.MLB == orderItem.Item.Id && y.Variacao == orderItem.Item.VariationId);
						}
						else
						{
							prodTentativo = produtos.FirstOrDefault(y => y.MLB == orderItem.Item.Id);
						}

						if (prodTentativo is not null)
						{
							_logger.LogInformation($"TRACE>> tentativo was not null");

							var mlItem = await _mlApiService.GetProdutoFromMlb(prodTentativo.MLB);
							_logger.LogInformation($"TRACE>> mlItem encontrado: {mlItem.Item2.Title}");
							if (mlItem.Item1 && mlItem.Item2.AvailableQuantity == prodTentativo.QuantidadeAVenda)
							{
								_logger.LogInformation($"TRACE>> mlItem.Item2.AvailableQuantity == prodTentativo.QuantidadeAVenda");

								return;
							}

							if (prodTentativo.Estoque < prodTentativo.QuantidadeAVenda)
							{
								_logger.LogInformation($"TRACE>> Não há estoque o bastante para repor na venda");
								return;
							}
							prodTentativo.Estoque -= orderItem.Quantity;
							_logger.LogInformation($"TRACE>> Estoque:{prodTentativo.Estoque}, OrderQuant: {orderItem.Quantity}");
							await _produtoDataService.AddOrUpdate(prodTentativo);

							if (prodTentativo.Estoque >= prodTentativo.QuantidadeAVenda)
							{
								_logger.LogInformation("Atualizando estoque no ML");
								_logger.LogInformation($"prodTentativo.MLB: {prodTentativo.MLB}");
								_logger.LogInformation($"prodTentativo.QuantidadeAVenda: {prodTentativo.QuantidadeAVenda}");
								_logger.LogInformation($"notification.UserId: {notification.UserId}");
								_logger.LogInformation($"prodTentativo.Variacao: {prodTentativo.Variacao}");

								await _mlApiService.AtualizaEstoqueDisponivel(prodTentativo.MLB,
									prodTentativo.QuantidadeAVenda,
									notification.UserId,
									prodTentativo.Variacao, _logger);
							}
						}

					}
				}

				return;
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				return;
			}
		}

		public IActionResult MlCallback([FromBody] OrdersV2Notification notification)
		{
			_logger.LogInformation($"TRACE>> Id: {notification.Id}");
			_logger.LogInformation($"TRACE>> Resource{notification.Resource}");
			_logger.LogInformation($"TRACE>> UserId: {notification.UserId}");
			_logger.LogInformation($"TRACE>> Topic: {notification.Topic}");
			_logger.LogInformation($"TRACE>> Attempts: {notification.Attempts}");
			_logger.LogInformation($"TRACE>> Sent: {notification.Sent}");
			_logger.LogInformation($"TRACE>> Received: {notification.Received}");

			CallbackEvent?.Invoke(this, new() { Notification = notification });
			return Ok();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

	}

	public class CallbackEventArgs
	{
		public OrdersV2Notification Notification { get; set; }
	}


	public class TokenAuthResponse
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("token_type")]
		public string TokenType { get; set; }

		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonPropertyName("scope")]
		public string Scope { get; set; }

		[JsonPropertyName("user_id")]
		public int UserId { get; set; }

		[JsonPropertyName("refresh_token")]
		public string RefreshToken { get; set; }
	}
}