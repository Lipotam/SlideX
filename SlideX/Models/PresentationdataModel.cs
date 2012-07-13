using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace SlideX.Models
{
    public class PresentationDataModel
    {
        SlideXDatabaseContext db = new SlideXDatabaseContext();
        public Presentation CurrentPresentation { get; set; }

        public string TagsJson
        {
            get
            {
                if (CurrentPresentation != null)
                {

                    var tags = CurrentPresentation.Tags.ToArray();
                    if (tags != null)
                    {
                        List<String> tagsToJson = new List<String>();
                        foreach (var temp in tags)
                        {
                            tagsToJson.Add(temp.Name);
                        }


                        return JsonConvert.SerializeObject(tagsToJson); 
                        


                    }

                }
                return "[]";
            }
            set
            {
                List<string> receivedTags = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(value);


                foreach (var temp in receivedTags)
                {
                    if (db.Tags.SingleOrDefault(p => p.Name == temp) == null)
                    {
                        db.Tags.AddObject(new Tag { Name = temp });
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

                db.SaveChanges();
            }

        }


    }
}