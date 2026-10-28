using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutosController(IProdutoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ListarProduto()
        => Ok(await _service.ListarProdutoAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarProduto(int id)
    {
        var produto = await _service.BuscarProdutoPorIdAsync(id);
        return produto is null ? NotFound() : Ok(produto);
    }

    [HttpPost]
    public async Task<IActionResult> CriarProduto([FromBody] Produto produto)
    {
        try
        {
            var criado = await _service.CriarProdutoAsync(produto);

            return CreatedAtAction(nameof(BuscarProduto), new {id = criado.Id}, criado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new {mensagem = ex.Message});
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
    {
        try
        {
            var atualizado = await _service.AtualizarProdutoAsync(id, produto);
            return atualizado is null ? NotFound() : Ok(atualizado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarProduto(int id)
    {
        var removido = await _service.DeletarProdutoAsync(id);
        return removido ? NoContent() : NotFound();
    }
}