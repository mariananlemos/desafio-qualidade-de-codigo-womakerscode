using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Integrations;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class LogisticaService : ILogisticaService
{
    private readonly AppDbContext _context;

    public LogisticaService(AppDbContext context)
    {
        _context = context;
    }

    public Task<string> EnviarLogisticaAsync(Pedido pedido)
    {
        return LogisticaMock.EnviarAsync(pedido);
    }

    public async Task<Pedido?> ReenviarLogisticaAsync(int pedidoId)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido is null) return null;

        if (pedido.Status != "Pago" && pedido.Status != "Enviado")
        {
            throw new InvalidOperationException("Pagamento do pedido está pendente.");
        }

        var codigo = await EnviarLogisticaAsync(pedido);

        pedido.CodigoRastreio = codigo;
        pedido.Status = "Enviado";

        await _context.SaveChangesAsync();

        return pedido;
    }
}