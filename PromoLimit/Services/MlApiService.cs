using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MlSuite.EntityFramework.EntityFramework;
using PromoLimit.Controllers;
using PromoLimit.Models.Local;
using PromoLimit.Models.MercadoLivre;
using RestSharp;

namespace PromoLimit.Services
{
	public class MlApiService
	{
        private readonly IServiceProvider _provider;

        private readonly RestClient _client;
		//private readonly MlInfoDataService _mlInfoDataService;

		public MlApiService(IServiceProvider provider)
        {
            _provider = provider;
            _client = new RestClient("https://api.mercadolibre.com");
			//_mlInfoDataService = provider.GetRequiredService<MlInfoDataService>();
        }
		public async Task<(bool, ProdutoBody?, string?)> GetProdutoFromMlb(string produtoMlb)
		{
			RestRequest request = new RestRequest($"items/{produtoMlb}");
			var response = await _client.ExecuteGetAsync<ProdutoBody>(request);

			if (!response.IsSuccessful)
			{
				if (response.Data is null)
				{
					return (false, null, response.ErrorMessage ?? "Erro");
				}
				return (false, null, response.ErrorMessage ?? "Erro");
			}

			if (response.Data.Error is not null)
			{
				return (false, null, null);
			}

			return (true, response.Data, null);
		}

		//public async Task<string?> RefreshToken(int userId)
		//{
		//	MLInfo? mlInfo = await _mlInfoDataService.GetByUserIdAsNoTracking(userId);
		//	(bool success, TokenAuthResponse response, string error) tokenXChange = await RefreshApiKey(mlInfo.RefreshToken);

		//	if (!tokenXChange.success || tokenXChange.response is null)
		//	{
		//		return tokenXChange.error;
		//	}
		//	else
		//	{
		//		try
		//		{
		//			(bool, string) userInfo = await GetSellerName(tokenXChange.response.UserId);

		//			await _mlInfoDataService.AddOrUpdateMlInfo(new()
		//			{
		//				AccessToken = tokenXChange.response.AccessToken,
		//				RefreshToken = tokenXChange.response.RefreshToken,
		//				UserId = tokenXChange.response.UserId,
		//				Vendedor = userInfo.Item2,
		//				ExpiryTime = DateTime.Now.AddSeconds(tokenXChange.response.ExpiresIn),
		//			}
		//			);
		//		}
		//		catch (Exception e)
		//		{
		//			return e.Message;
		//		}
		//	}

		//	return null;
		//}

		//public async Task<(bool success, TokenAuthResponse? response, string? error)> XChangeCodeForToken(string code)
		//{
		//	RestRequest request = new RestRequest("oauth/token").AddJsonBody(
		//		new
		//		{
		//			grant_type = "authorization_code",
		//			client_id = "5728494926210323",
		//			client_secret = "8FipsohX5nBunPKnymueFoxjYOCaCXmx",
		//			code,
		//			redirect_uri = "https://promolimit.azurewebsites.net/Home/MlRedirect",
		//		}
		//	);

		//	var response = await _client.ExecutePostAsync<TokenAuthResponse>(request);

		//	if (!response.IsSuccessful)
		//	{
		//		var erro = JsonSerializer.Deserialize<ErrorBody>(response.Content);
		//		return (false, null, $"{erro.Error} - {erro.Message}");
		//	}

		//	return (true, response.Data, null);
		//}

		//public async Task<(bool success, TokenAuthResponse? response, string? error)> RefreshApiKey(string refreshKey)
		//{
		//	RestRequest request = new RestRequest("oauth/token").AddJsonBody(
		//		new
		//		{
		//			grant_type = "refresh_token",
		//			client_id = "5728494926210323",
		//			client_secret = "8FipsohX5nBunPKnymueFoxjYOCaCXmx",
		//			refresh_token = refreshKey,
		//			redirect_uri = "https://promolimit.azurewebsites.net/Home/MlRedirect",
		//		}
		//	);

		//	var response = await _client.ExecutePostAsync<TokenAuthResponse>(request);

		//	if (!response.IsSuccessful)
		//	{
		//		var erro = JsonSerializer.Deserialize<ErrorBody>(response.Content);
		//		return (false, null, $"{erro.Error} - {erro.Message}");
		//	}

		//	return (true, response.Data, null);
		//}

		public async Task<(bool, string)> GetSellerName(int sellerId)
		{
			RestRequest request = new RestRequest($"users/{sellerId}");
			var response = await _client.ExecuteGetAsync<MlUser>(request);

			if (!response.IsSuccessful)
			{
				if (response.Data is null)
				{
					return (false, "Data era null");
				}
				return (false, "IsSuccessful was false");
			}

			if (response.Data.Error is not null)
			{
				return (false, $"{response.Data.Error} - {response.Data.Message}");
			}

			return (true, response.Data.Nickname);

		}

		public async Task<(bool, MlOrder?)> GetOrderInfo(long orderId, int sellerId, LoggingDataService logger)
		{
			_ = logger.LogInformation($"TRACE>> Getting order {orderId}", "GetOrderInfo");
			//MLInfo? apiKeyInfo = await _mlInfoDataService.GetByUserIdAsNoTracking(sellerId);
			//if (apiKeyInfo.ExpiryTime <= DateTime.Now)
			//{
			//	_ = logger.LogInformation($"TRACE>> ApiToken venceu", "GetOrderInfo");

			//	await RefreshToken(sellerId);
			//	apiKeyInfo = await _mlInfoDataService.GetByUserIdAsNoTracking(sellerId);

			//}
			//string apiKey = apiKeyInfo.AccessToken;
            string apiKey = (await _provider.GetRequiredService<TrilhaDbContext>().MlUserAuthInfos
                .FirstAsync(x => x.UserId == sellerId)).AccessToken;

			RestRequest request = new RestRequest($"orders/{orderId}");
			request.AddHeader("Authorization", $"Bearer {apiKey}");
			request.AddHeader("content-type", "application/json");
			try
			{
				var response = await _client.ExecuteGetAsync<MlOrder>(request);
				_ = logger.LogInformation($"TRACE>> Response: {response.IsSuccessful}", "GetOrderInfo");

				if (!response.IsSuccessful)
				{
					if (response.Data is null)
					{
						return (false, null);
					}
					return (false, null);
				}

				if (response.Data.Error is not null)
				{
					return (false, null);
				}

				return (true, response.Data);
			}
			catch (Exception e)
			{
				await File.WriteAllTextAsync("logCallback.txt", e.Message);

				return (false, null);

			}

		}

		public async Task<(bool, List<MlOrder>?, string?)> GetOrdersBetweenDates(int sellerId, DateTime start, DateTime end)
		{
			List<MlOrder> orders = new();
            //MLInfo? apiKeyInfo = await _mlInfoDataService.GetByUserIdAsNoTracking(sellerId);
            //if (apiKeyInfo.ExpiryTime <= DateTime.Now)
            //{
            //	await RefreshToken(sellerId);
            //	apiKeyInfo = await _mlInfoDataService.GetByUserIdAsNoTracking(sellerId);
            //}

            //string apiKey = apiKeyInfo.AccessToken;
            string apiKey = (await _provider.GetRequiredService<TrilhaDbContext>().MlUserAuthInfos
                .FirstAsync(x => x.UserId == sellerId)).AccessToken;
            RestRequest request = new RestRequest($"orders/search?seller={sellerId}&order.date_created.from={start.ToString("yyyy-MM-ddTHH:mm:ss.fff-03:00")}&order.date_created.to={end.ToString("yyyy-MM-ddTHH:mm:ss.fff-03:00")}");
			request.AddHeader("Authorization", $"Bearer {apiKey}");
			request.AddHeader("content-type", "application/json");
			try
			{
				var response = await _client.ExecuteAsync<MlOrders>(request);

				if (!response.IsSuccessful)
				{
					if (response.Data is null)
					{
						return (false, null, null);
					}

					return (false, null, null);
				}

				if (response.Data.Error is not null)
				{
					return (false, null, null);
				}

				orders.AddRange(response.Data.Results);

				if (response.Data.Paging.Total <= response.Data.Paging.Limit)
				{
					return (true, orders, null);
				}

				int offset = 0;
				while (response.Data.Paging.Total > response.Data.Paging.Limit + response.Data.Paging.Offset)
				{
					request.Resource =
						$"orders/search?seller={sellerId}&order.date_created.from={start.ToString("yyyy-MM-ddTHH:mm:ss.fff-03:00")}&order.date_created.to={end.ToString("yyyy-MM-ddTHH:mm:ss.fff-03:00")}&offset={offset += response.Data.Paging.Limit}";

					response = await _client.ExecuteAsync<MlOrders>(request);

					if (!response.IsSuccessful)
					{
						if (response.Data is null)
						{
							return (false, null, null);
						}

						return (false, null, null);
					}

					if (response.Data.Error is not null)
					{
						return (false, null, null);
					}

					orders.AddRange(response.Data.Results);
				}

				return (true, orders, null);


			}
			catch (Exception e)
			{
				await File.WriteAllTextAsync("logCallback.txt", e.Message);

				return (false, null, null);
			}
		}

		public async Task<(bool, string?)> AtualizaEstoqueDisponivel(string mlb, int estoqueDisponivel, int sellerId, long? variacao, LoggingDataService logger)
		{
            //MLInfo? apiKeyInfo = await _mlInfoDataService.GetByUserIdAsNoTracking(sellerId);
            //if (apiKeyInfo.ExpiryTime <= DateTime.Now)
            //{
            //	var error = await RefreshToken(sellerId);
            //	if (error is not null)
            //	{
            //		return (false, error);
            //	}
            //	apiKeyInfo = await _mlInfoDataService.GetByUserIdAsNoTracking(sellerId);

            //}
            //string apiKey = apiKeyInfo.AccessToken;

            string? apiKey = (await _provider.GetRequiredService<TrilhaDbContext>().MlUserAuthInfos
                .FirstOrDefaultAsync(x => x.UserId == sellerId))?.AccessToken;

            if (apiKey == null)
            {
				Debugger.Break();
            }

            RestRequest request = new RestRequest();
			request.AddHeader("Authorization", $"Bearer {apiKey}");
			request.AddHeader("content-type", "application/json");

			if (variacao is null)
			{
				request.Resource = $"items/{mlb}";
				request.AddJsonBody(new { available_quantity = estoqueDisponivel });
			}
			else
			{
				request.Resource = $"items/{mlb}/variations/{variacao}";
				Variation1 mlvar = new() { Id = (long)variacao, AvailableQuantity = estoqueDisponivel };

				request.AddJsonBody(mlvar);
			}

			RestResponse response = await _client.ExecutePutAsync(request);

			if (!response.IsSuccessful)
			{
				_ = logger.LogInformation($"TRACE>> atualizaestoque not successful", "AtualizaEstoqueDisponivel");
				await File.WriteAllTextAsync("logCallback.txt", response.ErrorMessage ?? "message null");

				return (false, response.ErrorMessage);
			}

			return (true, null);
		}
	}
}
