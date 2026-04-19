using Microsoft.AspNetCore.Authorization;
using E_commerce.Infrastructure.Authentication.Permissions;

namespace E_commerce.Api.Controllers;

/// <summary>
/// Manages the product catalog — listing, searching, adding, updating, and removing products.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    /// <summary>
    /// Get a paginated and optionally filtered list of all products.
    /// </summary>
    /// <param name="filter">Optional query filters: search term, category, page size, page index, sort.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the filtered list of products.</response>
    [HttpGet("")]
    [HasPermission(PermissionPolicyNames.ProductsRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilter? filter, CancellationToken cancellationToken)
    {
        var result = await _productService.GetAllProductsAsync(filter, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get a single product by its ID.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns the product details.</response>
    /// <response code="404">No product found with the given ID.</response>
    [HttpGet("{id}")]
    [HasPermission(PermissionPolicyNames.ProductsRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Add a new product. Requires the <c>products:create</c> permission.
    /// </summary>
    /// <param name="request">Product data sent as multipart/form-data (supports image upload).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">Product created. Returns the new product with its generated ID.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks the required permission.</response>
    [HttpPost("")]
    [HasPermission(PermissionPolicyNames.ProductsCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Add([FromForm] ProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.AddAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Update an existing product by its ID. Requires the <c>products:update</c> permission.
    /// </summary>
    /// <param name="id">The product ID to update.</param>
    /// <param name="request">Updated product data sent as multipart/form-data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Product updated successfully. Returns the updated product.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks the required permission.</response>
    /// <response code="404">No product found with the given ID.</response>
    [HttpPut("{id}")]
    [HasPermission(PermissionPolicyNames.ProductsUpdate)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] ProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Delete a product by its ID. Requires the <c>products:delete</c> permission.
    /// </summary>
    /// <param name="id">The product ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Product deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks the required permission.</response>
    /// <response code="404">No product found with the given ID.</response>
    [HttpDelete("{id}")]
    [HasPermission(PermissionPolicyNames.ProductsDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

}
