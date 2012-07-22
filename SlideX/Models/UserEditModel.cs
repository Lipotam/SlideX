using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace SlideX.Models
{
    /// <summary>
    /// Provides model for admin to edit user activity
    /// </summary>
    public class UserEditModel
    {
        private readonly PresentationDataAccessModel presentationData = new PresentationDataAccessModel();
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public Guid UserId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating admin status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [admin status]; otherwise, <c>false</c>.
        /// </value>
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
        /// <summary>
        /// Gets or sets a value indicating bann status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bann status]; otherwise, <c>false</c>.
        /// </value>
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