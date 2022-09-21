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
            MLInfo? mlInfo = await _mlInfoDataService.GetByUserId(46740585);
            if (mlInfo is null)
            {
                return (false, "Usuário não encontrado");
            }
            RestRequest request = new RestRequest("items")
                .AddHeader("Authorization",
                    $"Bearer {"APP_USR-5728494926210323-092111-10ccdd319c1b7071f24347142045c8ee-46740585"}")
                .AddQueryParameter("ids", produtoMlb);

            var response = await _client.ExecuteGetAsync<List<MlProduto>>(request);

            if (!response.IsSuccessful)
            {
                if (response.Data is null)
                {
                    return (false, "Data era null");
                }
                return (false, "IsSuccessful was false");
            }

            if (response.Data[0].Code != 200)
            {
                return (false, $"{response.Data[0].ProdutoBody.Error} - {response.Data[0].ProdutoBody.Message}");
            }

            return (true, response.Data[0].ProdutoBody.Title);
        }
    }
}
