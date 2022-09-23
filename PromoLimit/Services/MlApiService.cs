using PromoLimit.Controllers;
using PromoLimit.Models;
using RestSharp;
using RestSharp.Serializers;

namespace PromoLimit.Services
{
	public class MlApiService
    {
        private readonly RestClient _client;
        private readonly MlInfoDataService _mlInfoDataService;

        public MlApiService(IServiceProvider provider)
        {
            _client = new RestClient("https://api.mercadolibre.com");
            _mlInfoDataService = provider.GetRequiredService<MlInfoDataService>();
        }
        public async Task<(bool, ProdutoBody?)> GetDescricaoFromMLB(string produtoMlb)
        {
            RestRequest request = new RestRequest($"items/{produtoMlb}");
            var response = await _client.ExecuteGetAsync<ProdutoBody>(request);

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

        public async Task RefreshToken(int userId)
        {
            var code = await _mlInfoDataService.GetByUserId(userId, true);
            var tokenXChange = await XChangeCodeForToken(code.RefreshToken);

            if (!tokenXChange.Item1 || tokenXChange.Item2 is null)
            {
                throw new Exception("Response was not successful");
            }
            else
            {
                try
                {
                    var userInfo = await GetSellerName(tokenXChange.Item2.UserId);

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
        }

        public async Task<(bool, TokenAuthResponse?)> XChangeCodeForToken(string code)
        {
            RestRequest request = new RestRequest("oauth/token").AddJsonBody(
                new
                {
                    grant_type = "authorization_code",
                    client_id = "5728494926210323",
                    client_secret = "8FipsohX5nBunPKnymueFoxjYOCaCXmx",
                    code,
                    redirect_uri = "https://promolimit.azurewebsites.net/Home/MlRedirect",
                }
            );

            var response = await _client.ExecutePostAsync<TokenAuthResponse>(request);

            if (!response.IsSuccessful)
            {
                return (false, null);
            }

            return (true, response.Data);
        }

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

        public async Task<(bool, MlOrder?)> GetOrderInfo(int orderId, int sellerId)
        {
            var apiKeyInfo = await _mlInfoDataService.GetByUserId(sellerId, true);
            if (apiKeyInfo.ExpiryTime >= DateTime.Now)
            {
                await RefreshToken(sellerId);
                apiKeyInfo = await _mlInfoDataService.GetByUserId(sellerId, true);

            }
            string apiKey = apiKeyInfo.AccessToken;

            RestRequest request = new RestRequest($"orders/{orderId}");
            var response = await _client.ExecuteGetAsync<MlOrder>(request);
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("content-type", "application/json");

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

        public async Task<bool> AtualizaEstoqueDisponivel(string mlb, int estoqueDisponivel, int sellerId, int? variacao)
        {
            var apiKeyInfo = await _mlInfoDataService.GetByUserId(sellerId, true);
            if (apiKeyInfo.ExpiryTime >= DateTime.Now)
            {
                await RefreshToken(sellerId);
                apiKeyInfo = await _mlInfoDataService.GetByUserId(sellerId, true);

            }
            string apiKey = apiKeyInfo.AccessToken;
            
            RestRequest request = new RestRequest($"items/{mlb}");
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("content-type", "application/json");
            if (variacao is not null)
            {
                request.AddJsonBody(new {available_quantity = estoqueDisponivel});
            }
            else
            {
                MlVariation mlvar = new();
                mlvar.Variations = new();
                mlvar.Variations.Add(new() {Id = variacao, AvailableQuantity = estoqueDisponivel});
                request.AddJsonBody(mlvar);
            }

            var response = await _client.ExecutePutAsync(request);

            if (!response.IsSuccessful)
            {
                return false;
            }

          return true;
        }
    }
}
