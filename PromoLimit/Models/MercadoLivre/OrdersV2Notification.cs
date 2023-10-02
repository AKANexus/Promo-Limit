namespace PromoLimit.Models.MercadoLivre
{
    public class OrdersV2Notification
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("application_id")]
        public long ApplicationId { get; set; }

        [JsonPropertyName("attempts")]
        public int Attempts { get; set; }

        [JsonPropertyName("sent")]
        public DateTime Sent { get; set; }

        [JsonPropertyName("received")]
        public DateTime Received { get; set; }
    }
}
