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

        public class SaveMlbEntryJson
        {
            public int? Id { get; set; }
            public string MLB { get; set; }
            public string QuantidadeAVenda { get; set; }
            public bool Ativo { get; set; }
            public string? Descricao { get; set; }
            public int? Estoque { get; set; }
            public int? Seller { get; set; }
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
                var consultaMl = await _mlApiService.GetDescricaoFromMLB(produto.MLB);
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
                            if (!produtos.Any(x => x.MLB == consultaMl.Item2.Id && x.Variacao == int.Parse(variation.Id)))
                            {
                                await _mlApiService.AtualizaEstoqueDisponivel(consultaMl.Item2.Id,
                                    int.Parse(produto.QuantidadeAVenda), consultaMl.Item2.SellerId, int.Parse(variation.Id));
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
                                Seller = consultaMl.Item2.SellerId
                            };
                            await _produtoDataService.AddOrUpdate(aGravar);
                        }
                    }
                    else
                    {
                        await _mlApiService.AtualizaEstoqueDisponivel(consultaMl.Item2.Id,
                            int.Parse(produto.QuantidadeAVenda), consultaMl.Item2.SellerId, null);

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

        public async Task<IActionResult> MlCallback([FromBody] OrdersV2Notification notification)
        {
            return Ok();
            var users = await _mlInfoDataService.GetAll();
            var produtos = await _produtoDataService.GetAllProdutos();

            if (users.Any(x => x.UserId == notification.UserId))
            {
                var order = await _mlApiService.GetOrderInfo(
                     int.Parse(notification.Resource.Split('/')[notification.Resource.Split('/').Length - 1]), notification.UserId);
                foreach (OrderItem orderItem in order.Item2.OrderItems)
                {
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
                        prodTentativo.Estoque -= orderItem.Quantity;
                        await _produtoDataService.AddOrUpdate(prodTentativo);

                        if (prodTentativo.Estoque >= prodTentativo.QuantidadeAVenda)
                        {
                            await _mlApiService.AtualizaEstoqueDisponivel(prodTentativo.MLB,
                                prodTentativo.QuantidadeAVenda,
                                notification.UserId,
                                prodTentativo.Variacao);
                        }
                    }

                }
            }
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
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