using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SlideX.Models
{
    public enum SearchType
    {
        [Display(Name = "Users", Order = 0)]
        Users,
        [Display(Name = "Presentations", Order = 1)]
        Presentations,
        [Display(Name = "tags", Order = 2)]
        Tags
    }

    public class SearchModel
    {
       
        public string SearchString {get; set;}
        public SearchType Type { get; set; }


    }
}