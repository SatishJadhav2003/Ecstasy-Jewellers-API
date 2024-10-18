namespace ECSTASYJEWELS.Models
{
    public class Orders
    {
        public int Order_ID { get; set; }
        public int Address_ID { get; set; }
        public int User_ID { get; set; }
        public DateTime Order_Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total_Amount { get; set; }
        public string Shipping_Address { get; set; } = string.Empty;
        public string Billing_Address { get; set; } = string.Empty;
        public string Payment_Status { get; set; } = string.Empty;
    }
}