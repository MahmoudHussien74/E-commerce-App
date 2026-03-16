using E_commerce.Api.Abstraction;
using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.AddAsync(request, cancellationToken);

        return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value}, result.Value)
                : result.ToProblem();
    }

    [HttpPut("")]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default!)
    {
        var result = await _categoryService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
