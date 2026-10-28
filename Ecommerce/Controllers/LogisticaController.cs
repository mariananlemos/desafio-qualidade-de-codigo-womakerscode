using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/logistica")]
public class LogisticaController : ControllerBase
{
    private readonly ILogisticaService _service;

    public LogisticaController(ILogisticaService service)
    {
        _service = service;
    }

    [HttpPost("reenviar/{pedidoId:int}")]
    public async Task<IActionResult> ReenviarLogistica(int pedidoId)
    {
        try
        {
            var pedido = await _service.ReenviarLogisticaAsync(pedidoId);
            return pedido is null ? NotFound() : Ok(pedido);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new {mensagem = ex.Message});
        }
    }
}