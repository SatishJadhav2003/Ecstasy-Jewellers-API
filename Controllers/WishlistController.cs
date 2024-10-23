
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {

        private readonly WishlistRepository _repository;

        public WishlistController(WishlistRepository Repository)
        {
            _repository = Repository;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Category>>> AddtoWishlist(Wishlist data)
        {
            try
            {
                var response = await _repository.AddtoWishlist(data);
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

        [HttpGet("{User_ID}")]
        public async Task<ActionResult<IEnumerable<WishlistOutput>>> GetUserWishlistItems(decimal User_ID)
        {
            try
            {
                var response = await _repository.GetUserWishlistItems(User_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpDelete("{Wishlist_ID}")]
        public async Task<ActionResult<IEnumerable<WishlistOutput>>> RemoveFromWishlist(int Wishlist_ID)
        {
            try
            {
                var response = await _repository.RemoveFromWishlist(Wishlist_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

    }


}
