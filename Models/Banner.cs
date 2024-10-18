namespace ECSTASYJEWELS.Models
{
   public class Banner
    {
        public decimal Banner_ID { get; set; }
        public string Banner_Name { get; set; } = string.Empty;
        public string Banner_Image { get; set; } = string.Empty;
        public decimal Category_ID { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Featured { get; set; }
        public DateTime Date_Added { get; set; }
    }
}