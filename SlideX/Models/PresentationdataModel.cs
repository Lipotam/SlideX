using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SlideX.Models
{
    public class PresentationDataModel
    {
        private readonly SlideXDatabaseContext DB = new SlideXDatabaseContext();
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


                foreach (var temp in receivedTags)
                {
                    if (DB.Tags.SingleOrDefault(p => p.Name == temp) == null)
                    {
                        DB.Tags.AddObject(new Tag { Name = temp });
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

                DB.SaveChanges();
            }

        }


    }

    
}