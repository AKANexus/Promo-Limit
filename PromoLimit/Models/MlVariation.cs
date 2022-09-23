using System.Text.Json.Serialization;

namespace PromoLimit.Models
{
// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class AttributeCombination1
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("value_id")]
        public string ValueId { get; set; }

        [JsonPropertyName("value_name")]
        public string ValueName { get; set; }

        [JsonPropertyName("value_struct")]
        public object ValueStruct { get; set; }

        [JsonPropertyName("values")]
        public List<Value1> Values { get; set; }
    }

    public class MlVariation
    {
        [JsonPropertyName("variations")]
        public List<Variation1> Variations { get; set; }
    }

    public class Value1
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("struct")]
        public object Struct { get; set; }
    }

    public class Variation1
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        [JsonPropertyName("attribute_combinations")]
        public List<AttributeCombination1> AttributeCombinations { get; set; }

        [JsonPropertyName("available_quantity")]
        public int AvailableQuantity { get; set; }
    }


}
