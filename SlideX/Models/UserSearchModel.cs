using System.Collections.Generic;

namespace SlideX.Models
{
    public class UserSearchModel
    {
        public  string SearchTemlate { get; set;}
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
            set { }
        }
    }
}