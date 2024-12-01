
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

        [HttpGet("GetCategoryByID/{Category_ID}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoryByID(int Category_ID)
        {
            try
            {
                var category = await _repository.GetCategoryByID(Category_ID);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }



    }


}
