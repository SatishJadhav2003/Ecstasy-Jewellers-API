
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
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Address>>> AddNewAddress(Address data)
        {
            try
            {
                var response = await _repository.AddNewAddress(data);
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

        [HttpPut("{addressId}")]
        public async Task<ActionResult<bool>> UpdateAddress(int addressId, Address updatedAddress)
        {
            try
            {
                // Call the UpdateAddress method from your repository
                var isUpdated = await _repository.UpdateAddress(addressId, updatedAddress);

                if (isUpdated)
                {
                    return Ok(isUpdated);  // Return a success response if updated
                }
                else
                {
                    return NotFound();  // Return Not Found if no address was updated
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }



        [HttpDelete("{Address_ID}")]
        public async Task<ActionResult<IEnumerable<Boolean>>> RemoveFromAddress(int Address_ID)
        {
            try
            {
                var response = await _repository.RemoveFromAddress(Address_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }

    }


}
