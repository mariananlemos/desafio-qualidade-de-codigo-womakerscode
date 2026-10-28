using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.DTOs;

public record PedidoResultado(bool Sucesso, string Mensagem, Pedido? Pedido = null);