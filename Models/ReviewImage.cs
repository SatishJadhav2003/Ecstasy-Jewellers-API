namespace ECSTASYJEWELS.Models
{
  public  class ReviewImage
    {
        public int RR_Image_ID {get;set;}
        public int Review_ID {get;set;}
        public int Product_ID {get;set;}
        public string Image {get;set;} = string.Empty;
    }
}