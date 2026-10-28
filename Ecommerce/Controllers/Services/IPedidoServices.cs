using Ecommerce.Application.DTOs;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IPedidoService
{
    Task<List<Pedido>> ListarPedidoAsync();
    Task<Pedido?> BuscarPedidoAsync(int id);
    Task<PedidoResultado> CriarPedidoAsync(PedidoRequest request);
}