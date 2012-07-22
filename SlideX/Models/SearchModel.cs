namespace SlideX.Models
{
    public enum SearchType
    {
        Users,
        Presentations,
        Tags
    }

    /// <summary>
    /// Provides search presentation model
    /// </summary>
    public class SearchModel
    {
        /// <summary>
        /// Gets or sets the search string.
        /// </summary>
        /// <value>
        /// The search string.
        /// </value>
        public string SearchString {get; set;}
        /// <summary>
        /// Gets or sets the search type.
        /// </summary>
        /// <value>
        /// The search type.
        /// </value>
        public SearchType Type { get; set; }
    }
}