namespace PromoLimit.Models
{
    public class Produto : EntityBase
    {
        public string MLB { get; set; }
        public int QuantidadeAVenda { get; set; } = 1;
        public bool Ativo { get; set; }
        public string Descricao { get; set; }
        public int Estoque { get; set; }

        public int Seller { get; set; }
        //public List<ParidadeBlingMLB>? MLBs { get; set; }
    }
}
