using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;
using Banner.DataAccess;

namespace ECSTASYJEWELS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannerController : ControllerBase
    {
        private readonly EJ_BannerDataAccess _BannerDataAccess;

        public BannerController(EJ_BannerDataAccess bannerDA)
        {
            _BannerDataAccess = bannerDA;
        }

        // Create Banner
        [HttpPost]
        public IActionResult AddBanner([FromBody] EJ_Banner Banner)
        {
            _BannerDataAccess.AddBanner(Banner);
            return Ok("Banner created successfully");
        }

        // Get All Banners
        [HttpGet]
        public ActionResult<List<EJ_Banner>> GetAllBanners()
        {
            var Banners = _BannerDataAccess.GetAllBanners();
            return Ok(Banners);
        }

        
    }
}
