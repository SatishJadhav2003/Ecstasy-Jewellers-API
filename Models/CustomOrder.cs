namespace ECSTASYJEWELS.Models
{
    public class Custom_Order
    {
         public int Custom_ID {get;set;}
        public int User_ID {get;set;}
        public int Category_ID {get;set;}
        public string Purity { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string File_Path { get; set; } = string.Empty;
        
        public string Mobile_Number {get;set;}  = string.Empty;
        public int Price {get;set;}
        public int Weight {get;set;}
        public DateTime Date_Added {get;set;}

    }
}