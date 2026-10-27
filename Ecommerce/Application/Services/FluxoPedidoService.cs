using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Integrations;

namespace Ecommerce.Application.Services;

/// <summary>
/// Centraliza os fluxos de processamento de pedidos (pagamento e logística).
/// Elimina as duplicações de código entre CriarPedido, ReprocessarPagamento e ReenviarLogistica.
/// </summary>
public static class FluxoPedidoService
{
    /// <summary>
    /// Executa o fluxo de pagamento para um pedido e atualiza seu status de forma consistente.
    /// </summary>
    public static async Task<bool> ProcessarPagamentoAsync(Pedido pedido, AppDbContext db)
    {
        if (pedido == null)
            throw new ArgumentNullException(nameof(pedido));

        var aprovado = await PagamentoMock.ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);

        if (!aprovado)
        {
            pedido.Status = StatusPedido.Falhou;
            await db.SaveChangesAsync();
            ServicoLog.Log($"Pagamento falhou para o pedido {pedido.Id}");
            return false;
        }

        pedido.Status = StatusPedido.Pago;
        await db.SaveChangesAsync();
        ServicoLog.Log($"Pagamento aprovado para o pedido {pedido.Id}");
        return true;
    }

    /// <summary>
    /// Executa o envio para logística e atualiza o código de rastreio e status do pedido.
    /// </summary>
    public static async Task<string> EnviarParaLogisticaAsync(Pedido pedido, AppDbContext db)
    {
        if (pedido == null)
            throw new ArgumentNullException(nameof(pedido));

        var codigoRastreio = await LogisticaMock.EnviarAsync(pedido);
        pedido.CodigoRastreio = codigoRastreio;
        pedido.Status = StatusPedido.Enviado;
        await db.SaveChangesAsync();

        ServicoLog.Log($"Pedido {pedido.Id} despachado com código de rastreio {codigoRastreio}");
        return codigoRastreio;
    }
}
