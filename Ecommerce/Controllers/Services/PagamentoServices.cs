using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Integrations;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly AppDbContext _context;

    public PagamentoService(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento)
    {
        return Task.FromResult(PagamentoMock.ProcessarPagamento(valor, formaPagamento));
    }

    public async Task<bool?> ReprocessarPagamentoAsync(int pedidoId)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido is null) return null;

        if (pedido.Status == "Enviado")
            throw new InvalidOperationException("O pedido já foi enviado.");

        var aprovado = await ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);

        pedido.Status = aprovado ? "Pago" : "Falhou";

        await _context.SaveChangesAsync();

        return aprovado;
    }
}