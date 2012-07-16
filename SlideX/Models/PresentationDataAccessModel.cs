using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace SlideX.Models
{
    public class PresentationDataAccessModel
    {
        private readonly SlideXDatabaseContext DB = new SlideXDatabaseContext();

        public IEnumerable<Presentation> GetPresentationsByName(string name)
        {
            return DB.Presentations.Where(p => p.Title == name);
        }

        public IEnumerable<Presentation> GetPresentationsByUserName(string name)
        {
            var user = DB.Users.SingleOrDefault(u => u.Name == name);
            if (user == null)
            {
                return null;
            }
            return DB.Presentations.Where(p => p.UserId == user.Id);
        }

        public IEnumerable<Presentation> GetPresentationsByUserId(Guid  userId)
        {
            return DB.Presentations.Where(p => p.UserId == userId);
        }

        public IEnumerable<Presentation> GetPresentationsByTag(string name)
        {
            Tag currentTag = DB.Tags.SingleOrDefault(t => t.Name == name);
            if (currentTag == null)
            {
                return null;
            }
            return DB.Presentations.Where(p => p.Tags.Contains(currentTag));
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
            DB.Presentations.DeleteObject(deletePresentation);
            DB.SaveChanges();
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
    }
}