namespace ECSTASYJEWELS.Models
{
    public class Payments
    {
        public int Payment_ID { get; set; }
        public int Order_ID { get; set; }
        public string Payment_Method { get; set; } = string.Empty;
        public string Transaction_ID { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Payment_Date { get; set; }
        public string Payment_Status { get; set; } = string.Empty;
    }
}