using CorkBoardProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorkBoardProject.ViewModels
{
    public class ViewCorkboardViewModel
    {
        public string userName { get; set; }

        public int loggedInUserId { get; set; }

        public int corkBoardUserId { get; set; }

        public int watch { get; set; }

        public int corkboardId{ get; set; }


        public string corkboardTitle { get; set; }
        public string corkboardCategory { get; set; }

        public IEnumerable<Pushpin> pushpins { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? lastUpdateTime { get; set; }
    }
}