
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly OrderRepository _repository;

        public OrderController(OrderRepository Repository)
        {
            _repository = Repository;
        }
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDto orderDto)
        {
            try
            {
                // Convert DTO to Model if necessary
                var order = new Order
                {
                    Address_ID = orderDto.Address_ID,
                    User_ID = orderDto.User_ID,
                    Order_Date = DateTime.Now,  // or use orderDto.Order_Date if needed
                    Status = orderDto.Status,
                    Total_Amount = orderDto.Total_Amount,
                    Shipping_Address = orderDto.Shipping_Address,
                    Billing_Address = orderDto.Billing_Address,
                    Payment_Status = orderDto.Payment_Status
                };

                var orderItems = orderDto.OrderItems.Select(item => new Order_Items
                {
                    Product_ID = item.Product_ID,
                    Quantity = item.Quantity,
                    Unit_Price = item.Unit_Price
                }).ToList();

                // Call the AddOrderAsync method
                var newOrderId = await _repository.AddOrderAsync(order, orderItems);

                return Ok(new { Order_ID = newOrderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class OrderDto
    {
        public int Address_ID { get; set; }
        public int User_ID { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal Total_Amount { get; set; }
        public string Shipping_Address { get; set; } = "";
        public string Billing_Address { get; set; } = "";
        public string Payment_Status { get; set; } = "";
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public decimal Unit_Price { get; set; }
    }

}
