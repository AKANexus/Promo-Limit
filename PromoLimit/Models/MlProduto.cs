using System.Text.Json.Serialization;

namespace PromoLimit.Models
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class AttributeCombination
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value_id")]
        public string ValueId { get; set; }

        [JsonPropertyName("value_name")]
        public string ValueName { get; set; }
        }

    public class ProdutoBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subtitle")]
        public object Subtitle { get; set; }

        [JsonPropertyName("seller_id")]
        public int SellerId { get; set; }

        [JsonPropertyName("available_quantity")]
        public int AvailableQuantity { get; set; }

        [JsonPropertyName("variations")]
        public List<Variation>? Variations { get; set; }
    }
    
    public class Variation
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("attribute_combinations")]
        public List<AttributeCombination> AttributeCombinations { get; set; }

        [JsonPropertyName("available_quantity")]
        public int AvailableQuantity { get; set; }
        
    }
    
}
