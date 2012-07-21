namespace SlideX.Models
{
    public enum SearchType
    {
        Users,
        Presentations,
        Tags
    }

    public class SearchModel
    {
        public string SearchString {get; set;}
        public SearchType Type { get; set; }
    }
}