namespace ECSTASYJEWELS.Models
{
    public class User
    {
        public int User_ID { get; set; }
        public string First_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Email_Verified { get; set; }
        public string Phone_Number { get; set; } = string.Empty;
        public bool Phone_Verified { get; set; }
        public string Password_Hash { get; set; } = string.Empty;
        public string Password_Salt { get; set; } = string.Empty;
        public DateTime Date_Created { get; set; }
        public DateTime? Last_Login { get; set; }
        public bool Is_Active { get; set; }
    }
}