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
        public async Task<(bool, string?)> GetDescricaoFromMLB(string produtoMlb)
        {
            RestRequest request = new RestRequest($"items/{produtoMlb}");
            var response = await _client.ExecuteGetAsync<ProdutoBody>(request);

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

            return (true, response.Data.Title);
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
                    redirect_uri = "https://promolimit.azurewebsites.net/Home/MlRedirect"
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
    }
}
