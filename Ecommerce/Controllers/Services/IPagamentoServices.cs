using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IPagamentoService
{
    Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento);
    Task<bool?> ReprocessarPagamentoAsync(int pedidoId);
}