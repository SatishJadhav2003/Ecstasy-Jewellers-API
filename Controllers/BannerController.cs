using ECSTASYJEWELS.Data;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannerController : ControllerBase
    {

        private readonly BannerRepository _repository;

        public BannerController(BannerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> GetAllActiveBanner()
        {
            try
            {
                var banners = await _repository.GetAllActiveBanners();
                return Ok(banners);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }



    }
}
