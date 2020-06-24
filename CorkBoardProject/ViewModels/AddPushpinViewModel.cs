using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorkBoardProject.ViewModels
{
    public class AddPushpinViewModel
    {
        [Required]
        public int CorkboardId { get; set; }

        public string CorkboardTitle { get; set; }

        [Required(ErrorMessage ="Fill in an appropriate URL")]
        [Display(Name ="URL",Prompt ="http://anything.anything.jpeg")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Fill in a description that best decribes your Pushpin")]
        public string Description { get; set; }

        public string Tags { get; set; }
    }
}