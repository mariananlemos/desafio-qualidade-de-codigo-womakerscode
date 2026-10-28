using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly IPagamentoService _service;
    public PagamentosController(IPagamentoService service)
    {
        _service = service;
    }

    [HttpPost("reprocessar/{pedidoId:int}")]
    public async Task<IActionResult> ReprocessarPagamento(int pedidoId)
    {
        try
        {
            var resultado = await _service.ReprocessarPagamentoAsync(pedidoId);

            if (resultado is null)
                return NotFound();

            if (!resultado.Value)
                return BadRequest(new {mensagem = "Pagamento recusado."});

            return Ok(new {mensagem = "Pagamento aprovado."});
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new {mensagem = ex.Message});
        }
    }
}