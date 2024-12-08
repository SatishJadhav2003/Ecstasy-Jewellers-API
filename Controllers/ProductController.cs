
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

        [HttpPost("GetFiltered")]
        public async Task<IActionResult> GetFilteredData(FilterData data)
        {
            var suggestions = await _repository.GetFilteredProducts(data);
            return Ok(suggestions);
        }

    }
    public class FilterData
    {
        public List<int> Category { get; set; } = new List<int>();
        public List<string> Gender { get; set; } = new List<string>();
        public List<int> Metal { get; set; } = new List<int>();
        public List<int> Price { get; set; } = new List<int>();

    }

}
