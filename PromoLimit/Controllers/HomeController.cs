﻿using Microsoft.AspNetCore.Mvc;
using PromoLimit.Models;
using System.Diagnostics;
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
            return (await _mlInfoDataService.GetAll()).Select(x => new SelectListItem() {Value = x.UserId.ToString(), Text = x.Vendedor}).ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMlbEntry([FromBody] SaveMlbEntryJson? produto)
        {
            if (produto is not null)
            {
                //await _produtoDataService.AddOrUpdate(produto);
                var consultaMl = await _mlApiService.GetDescricaoFromMLB(produto.MLB);
                if (!consultaMl.Item1)
                {
                    TempData["Error"] = consultaMl.Item2;
                    return Json(new { success = false, error = consultaMl.Item2 });
                }
                if (true)
                {
                    Produto aGravar = new()
                    {
                        Ativo = true,
                        Descricao = consultaMl.Item2,
                        Id = produto.Id ?? 0,
                        MLB = produto.MLB,
                        QuantidadeAVenda = int.Parse(produto.QuantidadeAVenda),
                        Estoque = produto.Estoque ?? 0,
                        Seller = (int?)produto.Seller ?? 0
                    };
                    aGravar = await _produtoDataService.AddOrUpdate(aGravar);
                    return Json(new { success= true });
                }
            }

            return Json(new { success = false, error = "Produto was null" });
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveMlbEntry([FromQuery] int id)
        {
            await _produtoDataService.Delete(id);
            return Json(new { success= true });
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
                                AccessToken = tokenXChange.Item2.AccessToken, RefreshToken = tokenXChange.Item2.RefreshToken, UserId = tokenXChange.Item2.UserId, Vendedor = userInfo.Item2
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
            string[] contents = new string[8];
            contents[0] = $"Id: {notification.Id}";
            contents[1] = $"Resource: {notification.Resource}";
            contents[2] = $"UserId: {notification.UserId}";
            contents[3] = $"Topic: {notification.Topic}";
            contents[4] = $"ApplicationId: {notification.Topic}";
            contents[5] = $"Attempts: {notification.Attempts}";
            contents[6] = $"Sent: {notification.Sent}";
            contents[7] = $"Received: {notification.Received}";
            await System.IO.File.AppendAllLinesAsync("logVendas.txt", contents);
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