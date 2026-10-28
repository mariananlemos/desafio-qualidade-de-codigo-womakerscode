using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _context;
    private readonly IPagamentoService _pagamentoService;
    private readonly ILogisticaService _logisticaService;

    public PedidoService(
        AppDbContext context,
        IPagamentoService pagamentoService,
        ILogisticaService logisticaService)
    {
        _context = context;
        _pagamentoService = pagamentoService;
        _logisticaService = logisticaService;
    }

    public async Task<List<Pedido>> ListarPedidoAsync()
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .ToListAsync();
    }

    public async Task<Pedido?> BuscarPedidoAsync(int id)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PedidoResultado> CriarPedidoAsync(
        PedidoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClienteNome))
            return Falha("Informe o nome do cliente");

        if (!Utils.ValidarEmail(request.ClienteEmail))
            return Falha("Email inválido");

        if (request.Itens is null || request.Itens.Count == 0)
            return Falha("Pedido sem itens");

        if (request.Itens.Any(i => i.Quantidade <= 0))
            return Falha("Quantidade inválida");

        var quantidades = request.Itens
            .GroupBy(i => i.ProdutoId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantidade));

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var ids = quantidades.Keys.ToList();

        var produtos = await _context.Produtos
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var (produtoId, quantidade) in quantidades)
        {
            if (!produtos.TryGetValue(produtoId, out var produto))
                return Falha($"Produto {produtoId} não encontrado");

            if (!produto.Ativo)
                return Falha($"Produto {produto.Nome} está inativo");

            if (produto.QuantidadeEstoque < quantidade)
                return Falha($"Quantidade indisponível no estoque para {produto.Nome}");
        }

        var pedido = new Pedido
        {
            ClienteNome = request.ClienteNome,
            ClienteEmail = request.ClienteEmail,
            DataPedido = DateTime.Now,
            FormaPagamento = request.FormaPagamento,
            Status = "Pendente",
            Itens = new List<ItemPedido>()
        };

        decimal total = 0;

        foreach (var itemRequest in request.Itens)
        {
            var produto = produtos[itemRequest.ProdutoId];

            pedido.Itens.Add(new ItemPedido
            {
                ProdutoId = produto.Id,
                Quantidade = itemRequest.Quantidade,
                PrecoUnitario = produto.Preco
            });

            total += produto.Preco * itemRequest.Quantidade;
        }

        var valorCompraParaDesconto = 500;
        var descontoParaCompraMinima = 0.10m;
        var descontoClienteVip = 0.20m;

        if (total > valorCompraParaDesconto)
            total -= total * descontoParaCompraMinima;
        else if (request.ClienteEmail.Contains(
            "vip", StringComparison.OrdinalIgnoreCase))
            total -= total * descontoClienteVip;

        pedido.ValorTotal = total;

        foreach (var (produtoId, quantidade) in quantidades)
            produtos[produtoId].QuantidadeEstoque -= quantidade;

        _context.Pedidos.Add(pedido);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var pagamentoAprovado = await _pagamentoService.ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);

        if (!pagamentoAprovado)
        {
            await using var compensacao =
                await _context.Database.BeginTransactionAsync();

            foreach (var (produtoId, quantidade) in quantidades)
                produtos[produtoId].QuantidadeEstoque += quantidade;

            pedido.Status = "Falhou";

            await _context.SaveChangesAsync();
            await compensacao.CommitAsync();

            Utils.Log("Pagamento falhou, pedido " + pedido.Id);

            return Falha("Pagamento recusado");
        }

        pedido.Status = "Pago";
        await _context.SaveChangesAsync();

        try
        {
            var codigo = await _logisticaService.EnviarLogisticaAsync(pedido);

            pedido.CodigoRastreio = codigo;
            pedido.Status = "Enviado";

            await _context.SaveChangesAsync();
        }
        catch
        {
            throw;
        }

        Utils.Log("Pedido " + pedido.Id + " criado com sucesso");

        return new PedidoResultado(true, "Pedido criado com sucesso", pedido);
    }

    private static PedidoResultado Falha(string mensagem)
    {
        return new PedidoResultado(false, mensagem);
    }
}