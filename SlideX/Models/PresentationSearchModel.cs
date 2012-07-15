using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlideX.Models
{
    public class PresentationSearchModel
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

        public IEnumerable<Presentation> GetPresentationsByTag(string name)
        {
            Tag currentTag = DB.Tags.SingleOrDefault(t => t.Name == name);
            if (currentTag == null)
            {
                return null;
            }
            return DB.Presentations.Where(p => p.Tags.Contains(currentTag));
        }
    }
}