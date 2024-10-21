using ECSTASYJEWELS.Data;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {

        private readonly ProductImagesRepository _repository;

        public ProductImagesController(ProductImagesRepository repository)
        {
            _repository = repository;
        }
        [HttpGet("{Product_ID}")]
        public async Task<ActionResult<IEnumerable<ProductData[]>>> GetProductByID(decimal Product_ID)
        {
            try
            {
                var products = await _repository.GetAllProductImages(Product_ID);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }
    }
}
