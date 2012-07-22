using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SlideX.Models
{
    public class PresentationWithTagsModel
    {
        public Presentation CurrentPresentation { get; set; }

        public string TagsJson
        {
            get
            {
                if (CurrentPresentation != null)
                {
                    var tags = CurrentPresentation.Tags.ToArray();
                    if (tags.LongCount() != 0)
                    {
                        var tagsToJson = tags.Select(temp => temp.Name).ToList();
                        return JsonConvert.SerializeObject(tagsToJson);
                    }
                }
                return "[]";
            }
            set
            {
                var receivedTags = JsonConvert.DeserializeObject<List<string>>(value);
                var presentationData = new PresentationDataAccessModel();
                foreach (var temp in receivedTags)
                {
                    if (!presentationData.IsTagExist(temp))
                    {
                        presentationData.AddTag(temp);
                    }

                    if (CurrentPresentation.Tags.SingleOrDefault(p => p.Name == temp) == null)
                    {
                        CurrentPresentation.Tags.Add(new Tag { Name = temp });
                    }
                }

                foreach (var temp in CurrentPresentation.Tags.AsEnumerable())
                {
                    if (!(receivedTags.Contains(temp.Name)))
                    {
                        CurrentPresentation.Tags.Remove(temp);
                    }
                }
            }
        }
    }
}