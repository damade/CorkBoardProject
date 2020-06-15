using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CorkBoardProject.ViewModels
{
    public class ConfirmPrivateCorkboardViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Pin should be four digits")]
        public int Pin { get; set; }

        public int CId { get; set; }
    }
}