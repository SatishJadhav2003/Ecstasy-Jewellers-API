
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly CartRepository _repository;

        public CartController(CartRepository Repository)
        {
            _repository = Repository;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Category>>> AddtoCart(Cart data)
        {
            try
            {
                var response = await _repository.AddtoCart(data);
                if (response == 0)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

    }


}
