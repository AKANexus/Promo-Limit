using Microsoft.AspNetCore.Mvc;
using PromoLimit.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using PromoLimit.Services;
using PromoLimit.Views.Home;
using PromoLimit.Models.MercadoLivre;
using PromoLimit.Models.Local;

namespace PromoLimit.Controllers
{
    public class HomeController : Controller
	{
		private readonly IServiceProvider _provider;
		private readonly LoggingDataService _logger;
		private readonly SessionService _sessionService;
		private readonly UserDataService _userDataService;
		private readonly MlInfoDataService _mlInfoDataService;
		private readonly ProdutoDataService _produtoDataService;
		private readonly MlApiService _mlApiService;
        private readonly TinyApi _tinyApiService;

        public HomeController(IServiceProvider provider)
		{
			_provider = provider;

			_logger = provider.GetRequiredService<LoggingDataService>();
			_sessionService = provider.GetRequiredService<SessionService>();
			_userDataService = provider.GetRequiredService<UserDataService>();
			_mlInfoDataService = provider.GetRequiredService<MlInfoDataService>();
			_produtoDataService = provider.GetRequiredService<ProdutoDataService>();
			_mlApiService = provider.GetRequiredService<MlApiService>();
			_tinyApiService = provider.GetRequiredService<TinyApi>();
            CallbackEvent += async (sender, args) => await RunCallbackChecks(args.Notification, provider);
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

			if (string.IsNullOrEmpty(filteringModel.query) || filteringModel.query == "null") filteringModel.query = null;
			var MlInfos = await _mlInfoDataService.GetAll();
			var produtos = await _produtoDataService.GetAllProdutosPaged(filteringModel.recordsPerPage, filteringModel.pageNumber, filteringModel.query);

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
			public string MLB { get; set; }
			public int? Id { get; set; }
			public string QuantidadeAVenda { get; set; }
			public int? Estoque { get; set; }
			public bool Ativo { get; set; }
			public string? Descricao { get; set; }
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
			if (produto is null) return BadRequest(new {success = false, error = "Produto was null"});
			if (produto.MLB[..3] != "MLB")
				produto.MLB = "MLB" + produto.MLB;
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
				if (consultaMl.Item2?.Variations is not null && consultaMl.Item2.Variations.Count > 0)
				{
					foreach (var variation in consultaMl.Item2.Variations)
					{
						//if (produtos.Any(x => x.MLB == consultaMl.Item2.Id && x.Variacao == variation.Id))
						//{
							var atualiza = await _mlApiService.AtualizaEstoqueDisponivel(consultaMl.Item2.Id, int.Parse(produto.QuantidadeAVenda), consultaMl.Item2.SellerId, variation.Id, _logger);
							if (!atualiza.Item1)
							{
								TempData["Error"] = atualiza.Item2;
								return Json(new { success = false, error = atualiza.Item2 });
							}
						//}
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

				return RedirectToAction("Index");
			}

			throw new Exception("Code was null");
		}

		public IActionResult TraceTest(string message)
		{
			_ = _logger.LogTrace("This is a trace", "TraceTest");
			_ = _logger.LogDebug("This is a debug", "TraceTest");
			_ = _logger.LogError("This is an error", "TraceTest");
			_ = _logger.LogInformation("This is an information", "TraceTest");
			_ = _logger.LogCritical("This would be a critical", "TraceTest");
			_ = _logger.LogWarning("This is a warning", "TraceTest");
			_ = _logger.LogTrace("And this is the message you sent: " + message, "TraceTest");
			return Json("Your test tracing has been completed", "TraceTest");
		}

		public event CallbackEventHandler CallbackEvent;

		public delegate void CallbackEventHandler(object sender, CallbackEventArgs e);

		public async Task RunCallbackChecks(OrdersV2Notification notification, IServiceProvider provider)
        {
            Guid operation = Guid.NewGuid();
			try
			{
				await _logger.LogTrace("Running Callback Checks", $"RunCallbackChecks ({operation})");
				await _logger.LogTrace(JsonSerializer.Serialize(notification), $"RunCallbackChecks ({operation})");
			}
			catch (Exception e)
			{
				await _logger.LogError(e.Message, $"RunCallbackChecks ({operation})");
				throw;
			}
			
			try
			{
				//using IServiceScope scope = provider.CreateScope();
				//MlInfoDataService scopedMlInfoDataService =
				//	provider.GetRequiredService<MlInfoDataService>();
				//ProdutoDataService scopedProdutoDataService =
				//	provider.GetRequiredService<ProdutoDataService>();

				var users = await _mlInfoDataService.GetAll();
				var produtos = await _produtoDataService.GetAllProdutos();

				if (users.Any(x => x.UserId == notification.UserId))
				{
					var order = await _mlApiService.GetOrderInfo(
						 long.Parse(notification.Resource.Split('/')[notification.Resource.Split('/').Length - 1]), notification.UserId, _logger);
					if (!order.Item1)
					{
						await _logger.LogInformation($"order.Item1 was false", $"RunCallbackChecks ({operation})");

						return;
					}
					if (order.Item2.Status != "paid")
					{
						await _logger.LogInformation($"order.Item2.status was not paid", $"RunCallbackChecks ({operation})");

						return;
					}
					await _logger.LogInformation($"TRACE>> itens: {order.Item2.OrderItems.Count}", $"RunCallbackChecks ({operation})");

					foreach (OrderItem orderItem in order.Item2.OrderItems)
					{
						await _logger.LogInformation($"TRACE>> orderItem: {orderItem.Item.Id}, variation: {orderItem.Item.VariationId.ToString() ?? "--"}", $"RunCallbackChecks ({operation})");

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

						if (prodTentativo is null) continue;

						await _logger.LogInformation($"TRACE>> tentativo was not null", $"RunCallbackChecks ({operation})");

						var mlItem = await _mlApiService.GetProdutoFromMlb(prodTentativo.MLB);
						await _logger.LogInformation($"TRACE>> mlItem encontrado: {mlItem.Item2.Title}", $"RunCallbackChecks ({operation})");
						if (mlItem.Item1 && mlItem.Item2.AvailableQuantity == prodTentativo.QuantidadeAVenda)
						{
							await _logger.LogInformation($"TRACE>> mlItem.Item2.AvailableQuantity == prodTentativo.QuantidadeAVenda", $"RunCallbackChecks ({operation})");

							return;
						}

                        try
                        {
							var sku = mlItem.Item2.Attributes?.FirstOrDefault(x => x.Id == "SELLER_SKU");
							if (sku != null)
							{
								await _logger.LogTrace($"SKU {sku.ValueName}", $"RunCallbackChecks ({operation})");
								var tinyTentative = await _tinyApiService.ProcuraEstoquePorCodigo(sku.ValueName);
								if (tinyTentative is not null)
								{
									await _logger.LogTrace($"Existe um produto no Tiny com esse SKU ({sku.ValueName}). Vamos verificar o estoque", $"RunCallbackChecks ({operation})");

                                    var saldo = (int?)tinyTentative["saldo"];
                                    if (saldo is null)
                                    {
                                        await _logger.LogTrace(
                                            $"Houve um erro ao puxar estoque do Tiny.\n[\"saldo\"] era nulo.\nIgnorando...",
                                            $"RunCallbackChecks ({operation})");
                                    }
									else
                                    {
                                        await _logger.LogTrace($"Saldo no Tiny: {saldo}",
                                            $"RunCallbackChecks ({operation})");
                                        prodTentativo.Estoque = (int) saldo;
                                    }
                                }
                                else
                                {
                                    await _logger.LogTrace(
                                        $"Não existe um produto no Tiny com esse SKU ({sku.ValueName}).\nIgnorando...",
                                        $"RunCallbackChecks ({operation})");
                                }

							}
                            else
                            {
                                await _logger.LogTrace($"SKU era nulo",
                                    $"RunCallbackChecks ({operation})");
                            }
						}
                        catch (Exception e)
                        {
                            await _logger.LogError($"Houve um erro ao puxar estoque do Tiny.\n{e.Message}\nIgnorando...",
                                $"RunCallbackChecks ({operation})");
                        }
                       

						if (prodTentativo.Estoque < prodTentativo.QuantidadeAVenda)
						{
							await _logger.LogInformation($"TRACE>> Não há estoque {prodTentativo.Estoque} o bastante para repor na venda", $"RunCallbackChecks ({operation})");
							return;
						}

						if (prodTentativo.Estoque >= prodTentativo.QuantidadeAVenda)
						{
							await _logger.LogInformation("Atualizando estoque no ML", $"RunCallbackChecks ({operation})");
							await _logger.LogInformation($"prodTentativo.MLB: {prodTentativo.MLB}", $"RunCallbackChecks ({operation})");
							await _logger.LogInformation($"prodTentativo.QuantidadeAVenda: {prodTentativo.QuantidadeAVenda}", $"RunCallbackChecks ({operation})");
							await _logger.LogInformation($"notification.UserId: {notification.UserId}", $"RunCallbackChecks ({operation})");
							await _logger.LogInformation($"prodTentativo.Variacao: {prodTentativo.Variacao}", $"RunCallbackChecks ({operation})");

							await _mlApiService.AtualizaEstoqueDisponivel(prodTentativo.MLB,
								prodTentativo.QuantidadeAVenda,
								notification.UserId,
								prodTentativo.Variacao, _logger);
						}

						prodTentativo.Estoque -= orderItem.Quantity;


						await _logger.LogInformation($"TRACE>> Estoque:{prodTentativo.Estoque}, OrderQuant: {orderItem.Quantity}", $"RunCallbackChecks ({operation})");
						await _produtoDataService.AddOrUpdate(prodTentativo);
					}
				}
				else
				{
					await _logger.LogWarning("Usuário do ML não cadastrado, pulando notificação", $"RunCallbackChecks ({operation})");
				}
			}
			catch (Exception e)
			{
				await _logger.LogError(e.Message, $"RunCallbackChecks ({operation})");
			}
		}

		public IActionResult MlCallback([FromBody] OrdersV2Notification notification)
		{
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