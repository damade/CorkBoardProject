using CorkBoardProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorkBoardProject.ViewModels
{
    public class HomePageViewModel
    {
        public string userName { get; set; }

        public User userDetails { get; set; }

        public IEnumerable<Corkboard> myCorkboards { get; set; }

        public Dictionary<string,string> recentCorkboardOwners { get; set; }

        public Dictionary<string, int> pushpinCount { get; set; }

        public IEnumerable<Corkboard> recentCorkboards { get; set; }





    }
}