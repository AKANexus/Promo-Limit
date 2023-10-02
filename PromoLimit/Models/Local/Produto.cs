namespace PromoLimit.Models.Local
{
    public class Produto : EntityBase
    {
        public string MLB { get; set; }
        public int QuantidadeAVenda { get; set; } = 1;
        public bool Ativo { get; set; }
        public string Descricao { get; set; }
        public int Estoque { get; set; }
        public long? Variacao { get; set; }
        public ulong Seller { get; set; }
    }
}
