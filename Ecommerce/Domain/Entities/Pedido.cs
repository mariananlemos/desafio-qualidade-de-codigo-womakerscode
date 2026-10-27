
using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;

public class Pedido{
    public int Id { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public DateTime DataPedido { get; set; }

    // Status tipado com enum em vez de string solta
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; }
    public string CodigoRastreio { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
}