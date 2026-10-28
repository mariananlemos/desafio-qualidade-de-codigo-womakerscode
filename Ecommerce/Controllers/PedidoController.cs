using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;
    public PedidosController(IPedidoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ListarPedido()
        => Ok(await _service.ListarPedidoAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPedido(int id)
    {
        var pedido = await _service.BuscarPedidoAsync(id);
        return pedido is null ? NotFound() : Ok(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequest request)
    {
        PedidoResultado resultado = await _service.CriarPedidoAsync(request);

        if (!resultado.Sucesso)
            return BadRequest(new {mensagem = resultado.Mensagem});

        return CreatedAtAction(
            nameof(BuscarPedido),
            new {id = resultado.Pedido!.Id},
            resultado.Pedido);
    }
}