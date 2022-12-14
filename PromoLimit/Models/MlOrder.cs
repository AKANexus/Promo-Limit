using System.Text.Json.Serialization;

namespace PromoLimit.Models
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("variation_id")]
        public long? VariationId { get; set; }
    }

    public class OrderItem
    {
        [JsonPropertyName("item")]
        public Item Item { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }


    public class MlOrder
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("order_items")]
        public List<OrderItem> OrderItems { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("seller")]
        public Seller Seller { get; set; }
    }

    public class Seller
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

    }

    public class VariationAttribute
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value_id")]
        public object ValueId { get; set; }

        [JsonPropertyName("value_name")]
        public string ValueName { get; set; }
    }


}
