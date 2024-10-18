// Controllers/CategoryController.cs
using System.Data.SqlClient;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly CategoryRepository _repository;

        public CategoryController(CategoryRepository Repository)
        {
            _repository = Repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllActiveCategory()
        {
            try
            {
                var category = await _repository.GetAllActiveCategory();
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }



    }


}
