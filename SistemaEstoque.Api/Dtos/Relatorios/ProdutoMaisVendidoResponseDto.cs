namespace SistemaEstoque.Api.Dtos.Relatorios
{
    public class ProdutoMaisVendidoResponseDto
    {
        public int ProdutoId { get; set; }

        public string Produto { get; set; } = string.Empty;

        public int QuantidadeVendida { get; set; }

        public decimal ValorTotalVendido { get; set; }
    }
}