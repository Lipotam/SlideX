using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlideX.Models
{
    public class UserSearchModel
    {
        public  string SearchTemlate { get; set;}
        public IEnumerable<User> FoundUsers { 
            
            get
            {
                PresentationDataAccessModel presentationData = new PresentationDataAccessModel();
                if (SearchTemlate == "")
                {
                    return presentationData.GetAllUsers();
                }
                return  presentationData.GetUsersByTemplate(SearchTemlate);
            }
            set { }
        }
      
    }
}