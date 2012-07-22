using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace SlideX.Models
{
    /// <summary>
    /// Provides getting data from DB using entity framework
    /// </summary>
    public class PresentationDataAccessModel
    {
        private readonly SlideXDatabaseContext dbEntity = new SlideXDatabaseContext();

        /// <summary>
        /// Gets the presentations in searching by presentation name. 
        /// </summary>
        /// <param name="name">Presentation name template.</param>
        /// <returns></returns>
        public IEnumerable<Presentation> GetPresentationsByNameTemplate(string name)
        {
            return dbEntity.Presentations.Where(p => p.Title.Contains(name));
        }

        /// <summary>
        /// Gets the presentations  in searching by user name.
        /// </summary>
        /// <param name="name">User name template.</param>
        /// <returns></returns>
        public IEnumerable<Presentation> GetPresentationsByUserNameTemplate(string name)
        {
            var foundUser = dbEntity.Users.Where(u => u.Name.Contains(name));
            if (foundUser.Count() == 0)
            {
                return null;
            }
            var foundPresentations = new List<Presentation>();
            foreach (var user in foundUser)
            {
                foreach (var presentation in dbEntity.Presentations.Where(p => p.UserId == user.Id))
                {
                    foundPresentations.Add(presentation);
                }
            }
            return foundPresentations;
        }

        /// <summary>
        /// Gets the presentations by user id.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <returns></returns>
        public IEnumerable<Presentation> GetPresentationsByUserId(Guid userId)
        {
            return dbEntity.Presentations.Where(p => p.UserId == userId);
        }

        /// <summary>
        /// Gets the presentations by in searching tag template.
        /// </summary>
        /// <param name="name">Tag name template</param>
        /// <returns></returns>
        public IEnumerable<Presentation> GetPresentationsByTagTemplate(string name)
        {
            var foundTags = dbEntity.Tags.Where(t => t.Name.Contains(name));
            if (foundTags.Count() == 0)
            {
                return null;
            }

            var foundPresentations = new List<Presentation>();
            foreach (var tag in foundTags)
            {
                foundPresentations.AddRange(tag.Presentations);
            }
            return foundPresentations;
        }

        /// <summary>
        /// Determines whether [is tag exist] [the specified tag name].
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>
        ///   <c>true</c> if [is tag exist] [the specified tag name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTagExist(string tagName)
        {
            if (dbEntity.Tags.SingleOrDefault(p => p.Name == tagName) == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds the tag to tag list
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        public void AddTag(string tagName)
        {
            dbEntity.Tags.AddObject(new Tag { Name = tagName });
            dbEntity.SaveChanges();
        }

        /// <summary>
        /// Adds the presentation.
        /// </summary>
        /// <param name="newPresentation">The new presentation.</param>
        public void AddPresentation(Presentation newPresentation)
        {
            dbEntity.Presentations.AddObject(newPresentation);
            dbEntity.SaveChanges();
        }

        /// <summary>
        /// Deletes the presentation.
        /// </summary>
        /// <param name="deletePresentation">The  presentation to delete.</param>
        public void DeletePresentation(Presentation deletePresentation)
        {
            deletePresentation.Tags.Clear();
            dbEntity.Presentations.DeleteObject(deletePresentation);
            dbEntity.SaveChanges();
        }

        /// <summary>
        /// Deletes the presentation by presentation id.
        /// </summary>
        /// <param name="presentationId">The presentation id.</param>
        /// <returns></returns>
        public bool DeletePresentationById(Guid presentationId)
        {
            Presentation foundPresentation = GetPresentationByPresentationId(presentationId);
            if (foundPresentation != null)
            {
                foundPresentation.Tags.Clear();
                dbEntity.Presentations.DeleteObject(foundPresentation);
                dbEntity.SaveChanges();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the presentation by presentation id.
        /// </summary>
        /// <param name="id">The presentation id.</param>
        /// <returns></returns>
        public Presentation GetPresentationByPresentationId(Guid id)
        {
            return dbEntity.Presentations.SingleOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Gets the presentation by current user id and by presentation id.
        /// </summary>
        /// <param name="presentationId">The presentation id.</param>
        /// <returns></returns>
        public Presentation GetPresentationByCurrentUserIdAndByPresentationId(Guid presentationId)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                var currentUserId = (Guid)currentUser.ProviderUserKey;
                return dbEntity.Presentations.SingleOrDefault(p => p.UserId == currentUserId && p.Id == presentationId);
            }
            return null;
        }

        /// <summary>
        /// Gets the presentations by current user.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Presentation> GetPresentationsByCurrentUser()
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                return dbEntity.Presentations.Where(p => p.UserId == (Guid)currentUser.ProviderUserKey).AsEnumerable();
            }
            return null;
        }

        /// <summary>
        /// Gets the list if  tag names.
        /// </summary>
        /// <returns>List of string</returns>
        public Array GetTagNames()
        {
            return dbEntity.Tags.Select(t => t.Name).ToArray();
        }

        /// <summary>
        /// Gets the tag  by name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        public Tag GetTagByName(string tagName)
        {
            return dbEntity.Tags.SingleOrDefault(p => p.Name == tagName);
        }

        /// <summary>
        /// Applies the presentation changes.
        /// </summary>
        /// <param name="newPresentation"> presentation.</param>
        public void ApplyPresentation(Presentation newPresentation)
        {
            dbEntity.Presentations.ApplyCurrentValues(newPresentation);
            dbEntity.SaveChanges();
        }

        /// <summary>
        /// Gets the users by template.
        /// </summary>
        /// <param name="userNameTemplate">The user name template.</param>
        /// <returns></returns>
        public IEnumerable<User> GetUsersByTemplate(string userNameTemplate)
        {
            return dbEntity.Users.Where(u => u.Name.Contains(userNameTemplate));
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetAllUsers()
        {
            return dbEntity.Users.ToArray();
        }

        /// <summary>
        /// Gets the user by user id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public User GetUserById(Guid id)
        {
            return dbEntity.Users.SingleOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Gets the  role by name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public Role GetRoleByName(string roleName)
        {
            return dbEntity.Roles.SingleOrDefault(r => r.Name == roleName);
        }

        /// <summary>
        /// Gets the user id by presentation id.
        /// </summary>
        /// <param name="presentationId">The presentation id.</param>
        /// <returns></returns>
        public Guid GetUserIdByPresentationId(Guid presentationId)
        {
            Presentation foundPresentation = dbEntity.Presentations.SingleOrDefault(p => p.Id == presentationId);
            if (foundPresentation != null)
            {
                return foundPresentation.UserId;
            }
            return presentationId;
        }

        /// <summary>
        /// Determines whether [is current user is admin].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is current user is admin]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrentUserIsAdmin()
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null && GetUserById((Guid)currentUser.ProviderUserKey).Roles.Contains(GetRoleByName("AdminUser")))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the admin to user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="adminSet">if set to <c>true</c> [admin set],if set to <c>false</c> [admin unset].</param>
        public void SetAdminToUser(Guid userId, bool adminSet)
        {
            User adminCandidate = GetUserById(userId);
            Role adminRole = GetRoleByName("AdminUser");
            if (adminSet && (!adminCandidate.Roles.Contains(adminRole)))
            {
                adminCandidate.Roles.Add(adminRole);
                dbEntity.SaveChanges();
                return;
            }
            if ((!adminSet) && adminCandidate.Roles.Contains(adminRole))
            {
                adminCandidate.Roles.Remove(adminRole);
                dbEntity.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the user id by user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public Guid GetUserIdByUserName(string userName)
        {
            return dbEntity.Users.SingleOrDefault(u => u.Name == userName).Id;
        }

        /// <summary>
        /// Determines whether [is user pass email confirm] [the specified user name].
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>
        ///   <c>true</c> if [is user pass email confirm] [the specified user name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUserPassEmailConfirm(string userName)
        {
            MembershipUser currentUser = Membership.GetUser(userName);
            if (currentUser != null && GetUserById((Guid)currentUser.ProviderUserKey).Roles.Contains(GetRoleByName("User")))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the user email confirmed.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void SetUserEmailConfirmed(Guid userId)
        {
            User user = GetUserById(userId);
            Role role = GetRoleByName("User");
            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
                dbEntity.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all tags.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tag> GetAllTags()
        {
            return dbEntity.Tags.AsEnumerable();
        }
    }
}
