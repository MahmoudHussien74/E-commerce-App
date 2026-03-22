namespace E_commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController(IBasketService basketService) : ControllerBase
    {
        private readonly IBasketService _basketService = basketService;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _basketService.GetBasketAsync(id);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CustomerBasketResponse basket)
        {
            var result = await _basketService.UpdateBasketAsync(basket);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _basketService.DeleteBasketAsync(id);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

    }
}
