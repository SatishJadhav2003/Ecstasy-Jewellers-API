namespace ECSTASYJEWELS.Models
{
    public class EJ_Banner
    {
       public int Banner_ID {get;set;} 
       public string Banner_Name {get;set;} ="";
       public string Banner_Image {get;set;} ="";
       public bool Is_Active {get;set;} 
       public bool Is_Edited {get;set;} 
       public bool Is_Deleted {get;set;} 
       public DateTime Inserted_Date {get;set;} 
       public DateTime Updated_Date {get;set;} 
    }
}