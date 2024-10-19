namespace ECSTASYJEWELS.Models
{
    public class Dimension
    {
        public int Dimension_ID {get;set;}
        public int Product_ID {get;set;}
        public int Category_ID {get;set;}
        public string Title { get; set; } = string.Empty;
        public string Dim_Desc { get; set; } = string.Empty;

    }
}