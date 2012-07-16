using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SlideX.Models
{
    public class UserEditModel
    {
        private readonly PresentationDataAccessModel presentationData = new PresentationDataAccessModel();
        public Guid UserId { get; set; }
        public bool AdminStatus
        { 
            get 
            {  
                return presentationData.GetUserById(UserId).Roles.Contains(presentationData.GetRoleByName("AdminUser"));
            }
            set { }
        }
        public bool BannStatus 
        {
            get
            {
                return Membership.GetUser(UserId).IsApproved;
            }
            set { }
        }
        public   IEnumerable<Presentation> FoundPresentations 
        {
            get
            {
                return presentationData.GetPresentationsByUserId(UserId);
            }
            set { }
        }

    }
}