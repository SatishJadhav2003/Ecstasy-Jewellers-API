namespace ECSTASYJEWELS.Models
{
  public  class Review_Images
    {
        public int RR_Image_ID {get;set;}
        public int Review_ID {get;set;}
        public int Product_ID {get;set;}
        public string Image {get;set;} = string.Empty;
    }
}