using ECSTASYJEWELS.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {

        private readonly CommonRepository _repository;

        public CommonController(CommonRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet("images/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                // Set the path to where the images are stored
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/category_images/", fileName);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound("Image not found.");
                }

                // Return the image as a file
                var fileBytes = System.IO.File.ReadAllBytes(imagePath);
                return File(fileBytes, "image/jpeg"); // Adjust MIME type if needed
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}