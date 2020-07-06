using CorkBoardProject.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorkBoardProject.ViewModels
{
    public class SearchPageViewModel
    {
        public List<Pushpin> allSearchResult { get; set; }

        public Dictionary<string, string> corkboardResult { get; set; }

        public Dictionary<string, string> pushpinOwners { get; set; }
    }
}