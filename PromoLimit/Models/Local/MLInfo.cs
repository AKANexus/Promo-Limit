namespace PromoLimit.Models.Local
{
    public class MLInfo : EntityBase
    {
        public ulong UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Vendedor { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
