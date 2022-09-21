namespace PromoLimit.Models
{
    public class User : EntityBase
    {
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Auth { get; set; }
        public bool Ativo { get; set; }
        public Guid Uuid { get; set; }
    }
}
