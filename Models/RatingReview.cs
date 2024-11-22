namespace ECSTASYJEWELS.Models
{
    public class Rating_Reviews
    {
        public int Review_ID { get; set; }
        public int Product_ID { get; set; }
        public int User_ID { get; set; }
        public int Rating { get; set; }
        public string Review_Text { get; set; } = string.Empty;
        public DateTime Review_Date { get; set; }
    }
}