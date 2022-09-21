
using System.Text.Json.Serialization;

namespace PromoLimit.Models
{
    public class ErrorBody
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("cause")]
        public List<object> Cause { get; set; }
    }

    public class MlErro
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("body")]
        public ErrorBody Body { get; set; }
    }
}
