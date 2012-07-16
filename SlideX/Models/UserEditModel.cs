using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace SlideX.Models
{
    [Authorize(Roles = "AdminUser")]
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
            set
            {
                if (presentationData.IsCurrentUserIsAdmin())
                {
                    presentationData.SetAdminToUser(UserId, value);
                }
            }
        }
        public bool BannStatus 
        {
            get
            {
                return ! Membership.GetUser(UserId).IsApproved;
            }
            set
            {
                if (presentationData.IsCurrentUserIsAdmin())
                {
                    Membership.GetUser(UserId).IsApproved = !value;
                }
            }
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