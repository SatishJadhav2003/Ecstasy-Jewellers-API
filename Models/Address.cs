namespace ECSTASYJEWELS.Models
{
    public class Address
    {
        public int Address_ID { get; set; }
        public int User_ID { get; set; }
        public decimal Mobile { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address_Type { get; set; } = string.Empty;
        public string Address_Line1 { get; set; } = string.Empty;
        public string Address_Line2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int Postal_Code { get; set; }
        public bool Is_Default { get; set; }
        public DateTime Date_Added { get; set; }
    }
}