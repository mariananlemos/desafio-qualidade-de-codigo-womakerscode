using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Services;
using Ecommerce.Application.DTOs;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Application.Services;

namespace Ecommerce.Controllers;

// Controller ainda concentrado (a divisão por recurso fica para outra etapa).
// Os ajustes abaixo removem números mágicos, aplicam enum de status,
// utilizam as novas classes de log/validação e eliminam duplicações de código (Pessoa 3).
[ApiController]
public class EcommerceController : ControllerBase
{
    // ==================== PRODUTOS (CRUD) ====================

    [HttpGet("api/produtos")]
    public IActionResult ListarProdutos()
    {
        var db = new AppDbContext();
        var lista = db.Produtos.ToList();
        return Ok(lista);
    }

    [HttpGet("api/produtos/{id}")]
    public IActionResult BuscarProduto(int id)
    {
        var db = new AppDbContext();
        var produto = db.Produtos.Find(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpPost("api/produtos")]
    public IActionResult CriarProduto([FromBody] Produto produto)
    {
        if (string.IsNullOrWhiteSpace(produto.Nome))
        {
            return BadRequest("nome invalido");
        }
        if (produto.Preco <= 0)
        {
            return BadRequest("preco invalido");
        }

        var db = new AppDbContext();
        db.Produtos.Add(produto);
        db.SaveChanges();
        ServicoLog.Log("Produto criado: " + produto.Nome);
        return Ok(produto);
    }

    [HttpPut("api/produtos/{id}")]
    public IActionResult AtualizarProduto(int id, [FromBody] Produto produto)
    {
        var db = new AppDbContext();
        var existente = db.Produtos.Find(id);
        if (existente == null) return NotFound();

        existente.Nome = produto.Nome;
        existente.Descricao = produto.Descricao;
        existente.Preco = produto.Preco;
        existente.QuantidadeEstoque = produto.QuantidadeEstoque;
        existente.Ativo = produto.Ativo;
        db.SaveChanges();
        return Ok(existente);
    }

    [HttpDelete("api/produtos/{id}")]
    public IActionResult DeletarProduto(int id)
    {
        var db = new AppDbContext();
        var existente = db.Produtos.Find(id);
        if (existente == null) return NotFound();

        db.Produtos.Remove(existente);
        db.SaveChanges();
        return Ok();
    }

    // ==================== ESTOQUE ====================

    [HttpGet("api/estoque")]
    public IActionResult ListarEstoque()
    {
        // Refatoração da duplicação com ListarProdutos: projeção direta e limpa no banco
        var db = new AppDbContext();
        var resultado = db.Produtos
            .Select(p => new { produto = p.Nome, quantidade = p.QuantidadeEstoque })
            .ToList();

        return Ok(resultado);
    }

    [HttpPost("api/estoque/reabastecer/{produtoId}/{quantidade}")]
    public IActionResult ReabastecerEstoque(int produtoId, int quantidade)
    {
        var db = new AppDbContext();
        var produto = db.Produtos.Find(produtoId);
        if (produto == null) return NotFound();

        produto.QuantidadeEstoque = produto.QuantidadeEstoque + quantidade;
        db.SaveChanges();
        ServicoLog.Log("Estoque reabastecido produto " + produtoId);
        return Ok(produto);
    }

    // ==================== PEDIDOS ====================

    [HttpPost("api/pedidos")]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClienteNome))
        {
            return BadRequest("informe o nome do cliente");
        }
        if (!ValidadorEmail.EhValido(request.ClienteEmail))
        {
            return BadRequest("email invalido");
        }
        if (request.Itens == null || request.Itens.Count == 0)
        {
            return BadRequest("pedido sem itens");
        }

        var db = new AppDbContext();

        var pedido = new Pedido
        {
            ClienteNome = request.ClienteNome,
            ClienteEmail = request.ClienteEmail,
            DataPedido = DateTime.Now,
            FormaPagamento = request.FormaPagamento,
            Status = StatusPedido.Pendente,
            Itens = new List<ItemPedido>()
        };

        decimal subtotal = 0;

        foreach (var itemReq in request.Itens)
        {
            var produto = db.Produtos.Find(itemReq.ProdutoId);
            if (produto == null)
            {
                return BadRequest("produto " + itemReq.ProdutoId + " nao encontrado");
            }
            if (!produto.Ativo)
            {
                return BadRequest("produto " + produto.Nome + " esta inativo");
            }
            if (produto.QuantidadeEstoque < itemReq.Quantidade)
            {
                return BadRequest("estoque insuficiente para " + produto.Nome);
            }

            produto.QuantidadeEstoque = produto.QuantidadeEstoque - itemReq.Quantidade;

            var item = new ItemPedido
            {
                ProdutoId = produto.Id,
                Quantidade = itemReq.Quantidade,
                PrecoUnitario = produto.Preco
            };
            pedido.Itens.Add(item);

            subtotal += produto.Preco * itemReq.Quantidade;
        }

        // Regra de negócio de desconto isolada e sem números mágicos
        pedido.ValorTotal = CalculadoraDesconto.CalcularTotalComDesconto(subtotal, request.ClienteEmail);

        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync();

        // Fluxo centralizado de pagamento (sem duplicação de lógica)
        bool pagamentoOk = await FluxoPedidoService.ProcessarPagamentoAsync(pedido, db);
        if (!pagamentoOk)
        {
            return BadRequest("pagamento recusado");
        }

        // Fluxo centralizado de logística (sem duplicação e assíncrono real)
        await FluxoPedidoService.EnviarParaLogisticaAsync(pedido, db);

        ServicoLog.Log("Pedido " + pedido.Id + " criado com sucesso");

        return Ok(pedido);
    }

    [HttpGet("api/pedidos")]
    public IActionResult ListarPedidos()
    {
        var db = new AppDbContext();
        var lista = db.Pedidos.Include(p => p.Itens).ToList();
        return Ok(lista);
    }

    [HttpGet("api/pedidos/{id}")]
    public IActionResult BuscarPedido(int id)
    {
        var db = new AppDbContext();
        var pedido = db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == id);
        if (pedido == null) return NotFound();
        return Ok(pedido);
    }

    // ==================== PAGAMENTO (reutiliza FluxoPedidoService) ====================

    [HttpPost("api/pagamentos/reprocessar/{pedidoId}")]
    public async Task<IActionResult> ReprocessarPagamento(int pedidoId)
    {
        var db = new AppDbContext();
        var pedido = db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        // Reutilização do fluxo de pagamento sem duplicar regras
        bool ok = await FluxoPedidoService.ProcessarPagamentoAsync(pedido, db);
        if (!ok)
        {
            return BadRequest("pagamento recusado novamente");
        }

        return Ok(pedido);
    }

    // ==================== LOGISTICA (reutiliza FluxoPedidoService) ====================

    [HttpPost("api/logistica/reenviar/{pedidoId}")]
    public async Task<IActionResult> ReenviarLogistica(int pedidoId)
    {
        var db = new AppDbContext();
        var pedido = db.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        // Reutilização do fluxo de despacho logístico sem duplicar regras
        await FluxoPedidoService.EnviarParaLogisticaAsync(pedido, db);
        return Ok(pedido);
    }
}
