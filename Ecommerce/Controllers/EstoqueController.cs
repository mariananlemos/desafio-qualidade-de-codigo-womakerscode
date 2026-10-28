using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/estoque")]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueService _service;

    public EstoqueController(IEstoqueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ListarEstoque()
        => Ok(await _service.ListarEstoqueAsync());

    [HttpPost("reabastecer/{produtoId:int}/{quantidade:int}")]
    public async Task<IActionResult> ReabastecerEstoque(int produtoId, int quantidade)
    {
        try
        {
            var atualizado = await _service.ReabastecerEstoqueAsync(produtoId, quantidade);

            return atualizado ? Ok() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}