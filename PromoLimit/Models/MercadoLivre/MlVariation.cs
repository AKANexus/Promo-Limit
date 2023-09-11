namespace PromoLimit.Models.MercadoLivre
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class MlVariation
    {
        [JsonPropertyName("variations")]
        public List<Variation1> Variations { get; set; }
    }

    public class Variation1
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("available_quantity")]
        public int AvailableQuantity { get; set; }
    }


}
