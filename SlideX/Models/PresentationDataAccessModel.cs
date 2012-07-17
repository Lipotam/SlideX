using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace SlideX.Models
{
    public class PresentationDataAccessModel
    {
        private readonly SlideXDatabaseContext DB = new SlideXDatabaseContext();

        public IEnumerable<Presentation> GetPresentationsByNameTemplate(string name)
        {
            return DB.Presentations.Where(p => p.Title.Contains(name));
        }

        public IEnumerable<Presentation> GetPresentationsByUserNameTemplate(string name)
        {
            var foundUser = DB.Users.Where(u => u.Name.Contains(name));
            if (foundUser.Count() == 0)
            {
                return null;
            }
            List<Presentation> foundPresentations = new List<Presentation>();
            foreach (var user in foundUser)
            {
                foreach (var presentation in DB.Presentations.Where(p => p.UserId == user.Id))
                {
                    foundPresentations.Add(presentation);
                }
            }
            return foundPresentations;
        }

        public IEnumerable<Presentation> GetPresentationsByUserId(Guid userId)
        {
            return DB.Presentations.Where(p => p.UserId == userId);
        }

        public IEnumerable<Presentation> GetPresentationsByTagTemplate(string name)
        {
            var foundTags = DB.Tags.Where(t => t.Name.Contains(name));
            if (foundTags.Count() == 0)
            {
                return null;
            }

            List<Presentation> foundPresentations = new List<Presentation>();
            foreach (var tag in foundTags)
            {
                foreach (var presentation in tag.Presentations)
                {
                    foundPresentations.Add(presentation);
                }
            }
            return foundPresentations;
        }

        public bool IsTagExist(string tagName)
        {
            if (DB.Tags.SingleOrDefault(p => p.Name == tagName) == null)
            {
                return false;
            }
            return true;
        }

        public void AddTag(string tagName)
        {
            DB.Tags.AddObject(new Tag { Name = tagName });
            DB.SaveChanges();
        }

        public void AddPresentation(Presentation newPresentation)
        {
            DB.Presentations.AddObject(newPresentation);
            DB.SaveChanges();
        }

        public void DeletePresentation(Presentation deletePresentation)
        {
            deletePresentation.Tags.Clear();
            DB.Presentations.DeleteObject(deletePresentation);
            DB.SaveChanges();
        }

        public bool DeletePresentationById(Guid presentationId)
        {
            Presentation foundPresentation = GetPresentationByPresentationId(presentationId);
            if (foundPresentation != null)
            {
                foundPresentation.Tags.Clear();
                DB.Presentations.DeleteObject(foundPresentation);
                DB.SaveChanges();
                return false;
            }
            return true;
        }



        public Presentation GetPresentationByPresentationId(Guid id)
        {
            return DB.Presentations.SingleOrDefault(p => p.Id == id);
        }

        public Presentation GetPresentationByCurrentUserIdAndByPreasentationId(Guid presentationId)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                var currentUserId = (Guid)currentUser.ProviderUserKey;
                return DB.Presentations.SingleOrDefault(p => p.UserId == currentUserId && p.Id == presentationId);
            }
            return null;
        }

        public IEnumerable<Presentation> GetPresentationsByCurrentUserId()
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                return DB.Presentations.Where(p => p.UserId == (Guid)currentUser.ProviderUserKey).AsEnumerable();
            }
            return null;
        }

        public Array GetTagNames()
        {
            return DB.Tags.Select(t => t.Name).ToArray();
        }

        public Tag GetTagByName(string tagName)
        {
            return DB.Tags.SingleOrDefault(p => p.Name == tagName);
        }

        public void ApplyPresentation(Presentation newPresentation)
        {
            DB.Presentations.ApplyCurrentValues(newPresentation);
            DB.SaveChanges();
        }

        public IEnumerable<User> GetUsersByTemplate(string userNameTemplate)
        {
            return DB.Users.Where(u => u.Name.Contains(userNameTemplate));
        }

        public IEnumerable<User> GetAllUsers()
        {
            return DB.Users.ToArray();
        }
        public User GetUserById(Guid id)
        {
            return DB.Users.SingleOrDefault(u => u.Id == id);
        }

        public Role GetRoleByName(string roleName)
        {
            return DB.Roles.SingleOrDefault(r => r.Name == roleName);
        }

        public Guid GetUserIdByPresentationId(Guid PresentationId)
        {
            Presentation foundPresentation = DB.Presentations.SingleOrDefault(p => p.Id == PresentationId);
            if (foundPresentation != null)
            {
                return foundPresentation.UserId;
            }
            return PresentationId;
        }

        public bool IsCurrentUserIsAdmin()
        {

            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null && GetUserById((Guid)currentUser.ProviderUserKey).Roles.Contains(GetRoleByName("AdminUser")))
            {
                return true;
            }
            return false;
        }

        public void SetAdminToUser(Guid userId, bool adminSet)
        {
            User adminCandidate = GetUserById(userId);
            Role adminRole = GetRoleByName("AdminUser");
            if (adminSet && (!adminCandidate.Roles.Contains(adminRole)))
            {
                adminCandidate.Roles.Add(adminRole);
                DB.SaveChanges();
                return;
            }
            if ((!adminSet) && adminCandidate.Roles.Contains(adminRole))
            {
                adminCandidate.Roles.Remove(adminRole);
                DB.SaveChanges();
            }
        }

        public Guid GetUserIdByUserName(string userName)
        {
            return DB.Users.SingleOrDefault(u => u.Name == userName).Id;
        }

        public bool IsUserPassEmailConfirm(string userName)
        {
            MembershipUser currentUser = Membership.GetUser(userName);
            if (currentUser != null && GetUserById((Guid)currentUser.ProviderUserKey).Roles.Contains(GetRoleByName("User")))
            {
                return true;
            }
            return false;
        }

        public void SetUserEmailConfirmed(Guid userId)
        {
            User user = GetUserById(userId);
            Role role = GetRoleByName("User");
            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
                DB.SaveChanges();
            }
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return DB.Tags.AsEnumerable();
        }
    }
}
