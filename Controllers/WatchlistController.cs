
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {

        private readonly WatchlistRepository _repository;

        public WatchlistController(WatchlistRepository Repository)
        {
            _repository = Repository;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Category>>> AddtoWatchlist(Watchlist data)
        {
            try
            {
                var response = await _repository.AddtoWatchlist(data);
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
        public async Task<ActionResult<IEnumerable<WatchlistOutput>>> GetUserWatchlistItems(decimal User_ID)
        {
            try
            {
                var response = await _repository.GetUserWatchlistItems(User_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

        [HttpDelete("{Watchlist_ID}")]
        public async Task<ActionResult<IEnumerable<WatchlistOutput>>> RemoveFromWatchlist(int Watchlist_ID)
        {
            try
            {
                var response = await _repository.RemoveFromWatchlist(Watchlist_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

    }


}
