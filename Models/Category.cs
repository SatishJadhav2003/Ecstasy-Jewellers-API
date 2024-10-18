namespace ECSTASYJEWELS.Models
{
   public class Category
    {
        public decimal Category_ID { get; set; }
        public string Category_Name { get; set; } = string.Empty;
        public string Category_Image { get; set; } = string.Empty;
        public bool Is_Active { get; set; }
        public DateTime Date_Added { get; set; }
    }
}