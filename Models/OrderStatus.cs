namespace ECSTASYJEWELS.Models
{
    public class Order_Status
    {
        public int Order_Status_ID { get; set; }
        public int Order_ID { get; set; }
        public string Status { get; set; }=string.Empty; // Example: Placed, Shipped, Out for Delivery, Delivered
        public DateTime Status_Timestamp { get; set; } // When the status was updated
        public string Location { get; set; } = string.Empty; // If needed, to track the location of the status change
    }

}