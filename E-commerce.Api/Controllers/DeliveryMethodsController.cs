using E_commerce.Application.Abstractions.Authorization;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Contracts.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers;

/// <summary>
/// Manages delivery methods including creation, retrieval, updates, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class DeliveryMethodsController(IDeliveryMethodService deliveryMethodService) : ControllerBase
{
    private readonly IDeliveryMethodService _deliveryMethodService = deliveryMethodService;

    /// <summary>
    /// Get all available delivery methods.
    /// </summary>
    /// <response code="200">Returns the list of delivery methods.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _deliveryMethodService.GetAllDeliveryMethodsAsync(HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get a specific delivery method by its ID.
    /// </summary>
    /// <param name="id">The delivery method ID.</param>
    /// <response code="200">Returns the delivery method details.</response>
    /// <response code="404">Delivery method not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await _deliveryMethodService.GetDeliveryMethodByIdAsync(id, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Create a new delivery method. Requires admin privileges.
    /// </summary>
    /// <param name="request">Delivery method details.</param>
    /// <response code="201">Delivery method created successfully.</response>
    /// <response code="400">Validation failed.</response>
    [Authorize(Policy = PermissionPolicyNames.DeliveryMethodsCreate)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] DeliveryMethodRequest request)
    {
        var result = await _deliveryMethodService.CreateDeliveryMethodAsync(request, HttpContext.RequestAborted);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Update an existing delivery method. Requires admin privileges.
    /// </summary>
    /// <param name="id">The delivery method ID.</param>
    /// <param name="request">Updated delivery method details.</param>
    /// <response code="200">Delivery method updated successfully.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="404">Delivery method not found.</response>
    [Authorize(Policy = PermissionPolicyNames.DeliveryMethodsUpdate)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DeliveryMethodRequest request)
    {
        var result = await _deliveryMethodService.UpdateDeliveryMethodAsync(id, request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Delete a delivery method. Requires admin privileges.
    /// </summary>
    /// <param name="id">The delivery method ID.</param>
    /// <response code="204">Delivery method deleted successfully.</response>
    /// <response code="404">Delivery method not found.</response>
    [Authorize(Policy = PermissionPolicyNames.DeliveryMethodsDelete)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var result = await _deliveryMethodService.DeleteDeliveryMethodAsync(id, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
