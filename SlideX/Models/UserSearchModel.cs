using System.Collections.Generic;

namespace SlideX.Models
{
    /// <summary>
    /// Provides model for admin to search users by name template
    /// </summary>
    public class UserSearchModel
    {
        /// <summary>
        /// Gets or sets the search temlate.
        /// </summary>
        /// <value>
        /// The search temlate.
        /// </value>
        public  string SearchTemlate { get; set;}
        /// <summary>
        /// Gets the found users by search template.
        /// </summary>
        public IEnumerable<User> FoundUsers
        {
            get
            {
                var presentationData = new PresentationDataAccessModel();
                if (SearchTemlate == null)
                {
                    return presentationData.GetAllUsers();
                }
                return  presentationData.GetUsersByTemplate(SearchTemlate);
            }
        }
    }
}