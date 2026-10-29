namespace Ecommerce.Application.DTOs;

public class PedidoResponse
{
    public int Id { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? CodigoRastreio { get; set; }
    public List<ItemPedidoResponse> Itens { get; set; } = new();
}

public class ItemPedidoResponse
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}