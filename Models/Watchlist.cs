namespace ECSTASYJEWELS.Models
{
  public  class Watchlist
    {
        public int Watchlist_ID { get; set; }
        public int User_ID { get; set; }
        public int Product_ID { get; set; }
        public DateTime Date_Added { get; set; }
    }
}