
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

        [HttpPost("increment")]
        public async Task<ActionResult<IEnumerable<Category>>> IncrementQty([FromBody] int Cart_ID)
        {
            try
            {
                var response = await _repository.IncrementQty(Cart_ID);
                if (response)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "Unable to Increment Quantity");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpPost("decrement")]
        public async Task<ActionResult<IEnumerable<Category>>> DecrementQty([FromBody] int Cart_ID)
        {
            try
            {
                var response = await _repository.DecrementQty(Cart_ID);
                if (response)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "Unable to Decrement Quantity");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpGet("{User_ID}")]
        public async Task<ActionResult<IEnumerable<CartOutput>>> GetUserCartItems(decimal User_ID)
        {
            try
            {
                var response = await _repository.GetUserCartItems(User_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpDelete("{Cart_ID}")]
        public async Task<ActionResult<IEnumerable<CartOutput>>> RemoveFromCart(int Cart_ID)
        {
            try
            {
                var response = await _repository.RemoveFromCart(Cart_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

    }


}
