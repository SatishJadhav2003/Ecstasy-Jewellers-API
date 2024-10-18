namespace ECSTASYJEWELS.Models
{
    public class Order_Items
    {
        public int Order_Item_ID { get; set; }
        public int Order_ID { get; set; }
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public decimal Unit_Price { get; set; }
        public decimal Total_Price => Quantity * Unit_Price;
    }
}