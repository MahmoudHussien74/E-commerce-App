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

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]int id,CancellationToken cancellationToken)
    {
       var result = await _categoryService.GetByIdAsync(id,cancellationToken);

        return result is not null
            ?Ok(result)
            :NotFound();
    }
    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody]CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.AddAsync(request, cancellationToken);


        return result is null
                ? BadRequest()
                : CreatedAtAction(nameof(Get),new {id =result.Id},result);
    }

    [HttpPut("")]
    public async Task<IActionResult> Update([FromBody]UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(request, cancellationToken);

        return result ? NoContent() : BadRequest();
    }
    [HttpDelete("{id}")]

    public async Task<IActionResult>Delete([FromRoute]int id,CancellationToken cancellationToken =default!)
    {
       var result = await _categoryService.DeleteAsync(id,cancellationToken);

        return result ? NoContent()
            : BadRequest();
    }
}
