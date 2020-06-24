using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorkBoardProject.ViewModels
{
    public class ViewPushpinViewModel
    {
        public string userName { get; set; }

        public int userId { get; set; }

        public int corkboardId { get; set; }

        public int pushpinId { get; set; }

        public string corkboardTitle { get; set; }

        public string pushpinDescription { get; set; }

        public string pushpinUrl { get; set; }

        public string pushpinTags { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? lastUpdateTime { get; set; }
    }
}