
using ECSTASYJEWELS.Data;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly ProductRepository _repository;

        public ProductController(ProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{Product_ID}")]
        public async Task<ActionResult<IEnumerable<ProductData[]>>> GetProductByID(int Product_ID)
        {
            try
            {
                var products = await _repository.GetProductById(Product_ID);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpGet("ByCatgory/{Category_ID}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(decimal Category_ID)
        {
            try
            {
                var products = await _repository.GetAllProductsByCategory(Category_ID);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpGet("suggestions/{q}")]
        public async Task<IActionResult> GetSuggestions(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Ok(new List<string>());

            var suggestions = await _repository.GeatSearchProducts(q);
            return Ok(suggestions);
        }



    }


}
