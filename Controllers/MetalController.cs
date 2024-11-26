using ECSTASYJEWELS.Data;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetalController : ControllerBase
    {

        private readonly MetalRepository _repository;

        public MetalController(MetalRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Metal>>> GetAllMetal()
        {
            try
            {
                var metal = await _repository.GetAllMetal();
                return Ok(metal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }



    }
}
