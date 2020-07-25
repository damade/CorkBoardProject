using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorkBoardProject.ViewModels
{
    public class PopularSiteViewModel
    {
        public IOrderedEnumerable<KeyValuePair<string, int>> websiteCount { get; set; }
    }
}