namespace ECSTASYJEWELS.Models
{
    public class Shipping
    {
        public int Shipping_ID { get; set; }
        public int Order_ID { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string Tracking_Number { get; set; } = string.Empty;
        public DateTime Shipping_Date { get; set; }
        public DateTime? Delivery_Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}