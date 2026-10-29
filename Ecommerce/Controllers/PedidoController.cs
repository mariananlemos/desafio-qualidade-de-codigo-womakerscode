using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(IPedidoService service, ILogger<PedidosController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ListarPedido()
        {
        try
        {
            var pedidos = await _service.ListarPedidoAsync();
            return Ok(pedidos.Select(ParaResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos");
            return StatusCode(500, new { mensagem = "Erro interno ao listar pedidos." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPedido(int id)
    {
        try
        {
            var pedido = await _service.BuscarPedidoAsync(id);
            return pedido is null ? NotFound() : Ok(ParaResponse(pedido));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido {Id}", id);
            return StatusCode(500, new { mensagem = "Erro interno ao buscar pedido." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequest request)
    {
        try
        {
            PedidoResultado resultado = await _service.CriarPedidoAsync(request);

            if (!resultado.Sucesso)
                return BadRequest(new { mensagem = resultado.Mensagem });

            var response = ParaResponse(resultado.Pedido!);
            return CreatedAtAction(
                nameof(BuscarPedido),
                new { id = response.Id },
                response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, new { mensagem = "Erro interno ao criar pedido." });
        }
    }
    private static PedidoResponse ParaResponse(Pedido pedido)
    {
        return new PedidoResponse
        {
            Id = pedido.Id,
            ClienteNome = pedido.ClienteNome,
            ClienteEmail = pedido.ClienteEmail,
            DataPedido = pedido.DataPedido,
            FormaPagamento = pedido.FormaPagamento,
            Status = pedido.Status,
            ValorTotal = pedido.ValorTotal,
            CodigoRastreio = pedido.CodigoRastreio,
            Itens = pedido.Itens.Select(i => new ItemPedidoResponse
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList()
        };
    }
}