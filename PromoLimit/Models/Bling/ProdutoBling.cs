﻿namespace PromoLimit.Models.Bling
{
	    public class ProdutoBling : IEqualityComparer<ProdutoBling>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("situacao")]
        public string Situacao { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }

        [JsonPropertyName("preco")]
        public string Preco { get; set; }

        [JsonPropertyName("precoCusto")]
        public string PrecoCusto { get; set; }

        [JsonPropertyName("descricaoCurta")]
        public string DescricaoCurta { get; set; }

        [JsonPropertyName("descricaoComplementar")]
        public string DescricaoComplementar { get; set; }

        [JsonPropertyName("dataInclusao")]
        public string DataInclusao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public string DataAlteracao { get; set; }

        [JsonPropertyName("imageThumbnail")]
        public string ImageThumbnail { get; set; }

        [JsonPropertyName("urlVideo")]
        public string UrlVideo { get; set; }

        [JsonPropertyName("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        [JsonPropertyName("codigoFabricante")]
        public string CodigoFabricante { get; set; }

        [JsonPropertyName("marca")]
        public string Marca { get; set; }

        [JsonPropertyName("class_fiscal")]
        public string ClassFiscal { get; set; }

        [JsonPropertyName("cest")]
        public string Cest { get; set; }

        [JsonPropertyName("origem")]
        public string Origem { get; set; }

        [JsonPropertyName("idGrupoProduto")]
        public string IdGrupoProduto { get; set; }

        [JsonPropertyName("linkExterno")]
        public string LinkExterno { get; set; }

        [JsonPropertyName("observacoes")]
        public string Observacoes { get; set; }

        [JsonPropertyName("grupoProduto")]
        public object GrupoProduto { get; set; }

        [JsonPropertyName("garantia")]
        public string Garantia { get; set; }

        [JsonPropertyName("descricaoFornecedor")]
        public string DescricaoFornecedor { get; set; }

        [JsonPropertyName("idFabricante")]
        public string IdFabricante { get; set; }

        //[JsonPropertyName("categoria")]
        //public CategoriaBling Categoria { get; set; }

        [JsonPropertyName("pesoLiq")]
        public string PesoLiq { get; set; }

        [JsonPropertyName("pesoBruto")]
        public string PesoBruto { get; set; }

        [JsonPropertyName("estoqueMinimo")]
        public string EstoqueMinimo { get; set; }

        [JsonPropertyName("estoqueMaximo")]
        public string EstoqueMaximo { get; set; }

        [JsonPropertyName("gtin")]
        public string Gtin { get; set; }

        [JsonPropertyName("gtinEmbalagem")]
        public string GtinEmbalagem { get; set; }

        [JsonPropertyName("larguraProduto")]
        public string LarguraProduto { get; set; }

        [JsonPropertyName("alturaProduto")]
        public string AlturaProduto { get; set; }

        [JsonPropertyName("profundidadeProduto")]
        public string ProfundidadeProduto { get; set; }

        [JsonPropertyName("unidadeMedida")]
        public string UnidadeMedida { get; set; }

        [JsonPropertyName("itensPorCaixa")]
        public int ItensPorCaixa { get; set; }

        [JsonPropertyName("volumes")]
        public int Volumes { get; set; }

        [JsonPropertyName("localizacao")]
        public string Localizacao { get; set; }

        [JsonPropertyName("crossdocking")]
        public string Crossdocking { get; set; }

        [JsonPropertyName("condicao")]
        public string Condicao { get; set; }

        [JsonPropertyName("freteGratis")]
        public string FreteGratis { get; set; }

        [JsonPropertyName("producao")]
        public string Producao { get; set; }

        [JsonPropertyName("dataValidade")]
        public string DataValidade { get; set; }

        [JsonPropertyName("spedTipoItem")]
        public string SpedTipoItem { get; set; }

        [JsonPropertyName("estoqueAtual")]
        public string EstoqueAtual { get; set; }

        [JsonPropertyName("depositos")]
        public List<DepositoNode> Depositos { get; set; }

        [JsonPropertyName("clonarDadosPai")]
        public string ClonarDadosPai { get; set; }

        //[JsonPropertyName("estrutura")]
        //public List<Estrutura> Estrutura { get; set; }

        [JsonPropertyName("codigoPai")]
        public string CodigoPai { get; set; }

        //[JsonPropertyName("produtoLoja")]
        //public ProdutoLoja ProdutoLoja { get; set; }

        [JsonPropertyName("variacoes")]
        public List<VariaçãoNode> Variacoes { get; set; }

        public bool Equals(ProdutoBling? x, ProdutoBling? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Codigo == y.Codigo;
        }

        public int GetHashCode(ProdutoBling obj)
        {
            return obj.Codigo.GetHashCode();
        }
    }

}
