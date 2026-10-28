using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface ILogisticaService
{
    Task<string> EnviarLogisticaAsync(Pedido pedido);
    Task<Pedido?> ReenviarLogisticaAsync(int pedidoId);
}