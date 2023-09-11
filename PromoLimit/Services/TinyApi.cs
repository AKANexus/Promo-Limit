using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using RestSharp;

namespace PromoLimit.Services
{
    public class TinyApi
    {
        private RestClient _restClient = new("https://api.tiny.com.br/api2/");

        public TinyApi()
        {
            _restClient
                .AddDefaultQueryParameter("token", "0ea97cafcd06b189770d21537f31a80fbd659d95")
                .AddDefaultQueryParameter("formato", "json");
        }

        public async Task<JsonObject?> ProcuraEstoquePorCodigo(string codigo)
        {
            RestRequest request = new RestRequest("produtos.pesquisa.php")
                .AddQueryParameter("pesquisa", codigo);
            //var aa = await _restClient.PostAsync(request);
            var response = await _restClient.ExecutePostAsync(request);

            while (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(3000);
                Console.WriteLine("Lixo do tiny não aguentou. Tentando de novo.");
                response = await _restClient.ExecutePostAsync(request);
            }

            var objeto = JsonNode.Parse(response.Content).AsObject();

            if (objeto["retorno"]?["produtos"] is null || (objeto["retorno"]["produtos"]).AsArray().Count == 0)
            {
                Console.WriteLine("Código não encontrado no Tiny");
                return null;
            }

            return await PegaEstoquePorId((string)objeto["retorno"]["produtos"][0]["produto"]["id"]);
        }

        public async Task<JsonObject?> PegaEstoquePorId(string id)
        {
            RestRequest request = new RestRequest("produto.obter.estoque.php")
                .AddQueryParameter("id", id);
            //var aa = await _restClient.PostAsync(request);
            var response = await _restClient.ExecutePostAsync(request);

            while (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(3000);
                Console.WriteLine("Lixo do tiny não aguentou. Tentando de novo.");
                response = await _restClient.ExecutePostAsync(request);
            }

            var objeto = JsonNode.Parse(response.Content).AsObject();

            if (objeto["retorno"]["produto"] is null)
            {
                return null;
            }
            Console.WriteLine($"Produto encontrado no Tiny: {objeto["retorno"]?["produto"]?["nome"]}");

            return objeto["retorno"]?["produto"]?.AsObject();
        }
    }
}
