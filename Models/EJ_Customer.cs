namespace ECSTASYJEWELS.Models
{
    public class EJ_Customer
    {
       public decimal Cust_ID {get;set;} 
       public string Cust_Name {get;set;} ="";
       public decimal Cust_Mobile {get;set;} 
       public string Cust_Email {get;set;} ="";
       public string Cust_Password {get;set;} ="";
       public bool Is_Active {get;set;} 
       public bool Is_Edited {get;set;} 
       public bool Is_Deleted {get;set;} 
       public DateTime Inserted_Date {get;set;} 
       public DateTime Updated_Date {get;set;} 
    }
}