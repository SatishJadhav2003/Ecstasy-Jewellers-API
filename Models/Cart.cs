namespace ECSTASYJEWELS.Models
{
    public class Cart
    {
        public int Cart_ID { get; set; }
        public int User_ID { get; set; }
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public DateTime Date_Added { get; set; }
    }
}