namespace ECSTASYJEWELS.Models
{
    public class EJ_Product
    {
       public decimal Product_ID {get;set;} 
       public string Product_Name {get;set;} ="";
       public string Product_Image {get;set;} ="";
       public string Product_Desc {get;set;} ="";
       public string Caret {get;set;} ="";
       public string Weight {get;set;} ="";
       public decimal Rating {get;set;} 
       public decimal Price {get;set;} 
       public decimal Making_Charges {get;set;} 
       public decimal Other_Charges {get;set;} 
       public decimal Category_ID {get;set;} 

       public bool Is_Active {get;set;} 
       public bool Is_Edited {get;set;} 
       public bool Is_Deleted {get;set;} 
       public DateTime Inserted_Date {get;set;} 
       public DateTime Updated_Date {get;set;} 
    }
}