namespace ECSTASYJEWELS.Models
{
    public class User
    {
        public int? User_ID { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public bool? Email_Verified { get; set; }
        public string? Phone_Number { get; set; }
        public bool? Phone_Verified { get; set; }
        public string? Password { get; set; }
        public string? Password_Hash { get; set; }
        public string? Password_Salt { get; set; }
        public DateTime? Date_Created { get; set; }
        public DateTime? Last_Login { get; set; }
        public bool? Is_Active { get; set; }
    }

}