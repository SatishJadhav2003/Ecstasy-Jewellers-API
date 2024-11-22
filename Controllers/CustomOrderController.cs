
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomOrderController : ControllerBase
    {

        private readonly CustomOrderRepository _repository;

        public CustomOrderController(CustomOrderRepository Repository)
        {
            _repository = Repository;
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddToCustomOrder([FromForm] Custom_Order data, IFormFile file)
        {
            try
            {

                // Check if a file was provided
                if (file != null && file.Length > 0)
                {
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string filePath = Path.Combine(uploadPath, file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }


                    // Set the file path in the data model
                    data.File_Path = filePath;
                }

                // Save the data to the database
                var response = await _repository.AddtoCustomOrder(data);

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
