
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {

        private readonly AddressRepository _repository;

        public AddressController(AddressRepository Repository)
        {
            _repository = Repository;
        }

    
        [HttpGet("{User_ID}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetUserAddresses(decimal User_ID)
        {
            try
            {
                var response = await _repository.GetUserAddresses(User_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

    }


}
