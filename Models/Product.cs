namespace ECSTASYJEWELS.Models
{
     public class Product
    {
        public int Product_ID { get; set; }
        public string Product_Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Category_ID { get; set; }
        public decimal Price { get; set; }
        public int Stock_Quantity { get; set; }
        public decimal Weight { get; set; }
        public string Product_Image { get; set; } = string.Empty;
        
        public bool Is_Active { get; set; }
        public DateTime Date_Added { get; set; }
        public DateTime? Updated_Date { get; set; }
    }
}
