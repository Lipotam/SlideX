using System.Collections.Generic;
using System.Linq;


namespace SlideX.Models
{


    public class PresentationWithTags
    {
        /// <summary>
        /// current presentation  
        /// </summary>
        public Presentation presentation { get; set; }
        /// <summary>
        /// All tags of the presentation
        /// </summary>
        //public List<string> tags { get; set; }
        public List<string> tags { get; set; }
        ASPNETDBEntities db = new ASPNETDBEntities();
        public PresentationWithTags(Presentation currentPresentation)
        {
            
            presentation = currentPresentation;
            var tagsMatch = db.TagsToPresentations.Where(pt => pt.PresentationId == currentPresentation.PresentationId).AsEnumerable();
            if (tagsMatch != null)
            {
                foreach (var tt in tagsMatch)
                {
                    tags.Add(db.PresentationTags.SingleOrDefault(pt => tt != null && pt.TagId == tt.TagId).TagTitle);
                }
            }
        }
    }
}