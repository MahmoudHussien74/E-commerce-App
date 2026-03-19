using E_commerce.Api.Abstraction;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;


    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
     => Ok((await _productService.GetAllProductsAsync(cancellationToken)).Value);

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]int id,CancellationToken cancellationToken)
    {
        var result= await _productService.GetProductByIdAsync(id,cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("")]
    public async Task<IActionResult> Add([FromForm]ProductRequest request,CancellationToken cancellationToken)
    {
       var result =await _productService.AddAsync(request,cancellationToken);

       
        return result.IsSuccess?Created():result.ToProblem();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute]int id,[FromForm] ProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateAsync(id,request,cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute]int id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteAsync(id,cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

}
