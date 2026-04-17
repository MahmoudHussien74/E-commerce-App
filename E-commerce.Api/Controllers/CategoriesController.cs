using E_commerce.Application.Abstractions.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace E_commerce.Api.Controllers;

/// <summary>
/// Manages product categories — listing, getting, creating, updating, and deleting.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    /// <summary>
    /// Get a list of all available product categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns all categories.</response>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get a single category by its ID.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the category details.</response>
    /// <response code="404">No category found with the given ID.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Create a new product category. Requires the <c>categories:create</c> permission.
    /// </summary>
    /// <param name="request">Category data (name).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">Category created successfully. Returns the new category.</response>
    /// <response code="400">Validation failed or category name already exists.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks the required permission.</response>
    [HttpPost("")]
    [Authorize(Policy = PermissionPolicyNames.CategoriesCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Add([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.AddAsync(request, cancellationToken);

        return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
    }

    /// <summary>
    /// Update an existing category by its ID. Requires the <c>categories:update</c> permission.
    /// </summary>
    /// <param name="id">The category ID to update.</param>
    /// <param name="request">Updated category data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Category updated successfully.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks the required permission.</response>
    /// <response code="404">No category found with the given ID.</response>
    [HttpPut("{id}")]
    [Authorize(Policy = PermissionPolicyNames.CategoriesUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Delete a category by its ID. Requires the <c>categories:delete</c> permission.
    /// </summary>
    /// <param name="id">The category ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Category deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks the required permission.</response>
    /// <response code="404">No category found with the given ID.</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = PermissionPolicyNames.CategoriesDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default!)
    {
        var result = await _categoryService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
