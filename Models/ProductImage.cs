namespace ECSTASYJEWELS.Models
{
    public class Product_Images
    {
        public int Image_ID { get; set; }
        public int Product_ID { get; set; }
        public string Image_URL { get; set; } = string.Empty;
        public bool Is_Primary { get; set; }
    }
}