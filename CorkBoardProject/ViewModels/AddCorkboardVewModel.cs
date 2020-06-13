using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CorkBoardProject.Models;
using System.Web.Mvc;

namespace CorkBoardProject.ViewModels
{
    public class AddCorkboardVewModel
    {
        [Required]
        public string Title { get; set; }

        public int UserId { get; set; }

        public int Watch { get; set; }

        public IEnumerable<CorkboardCategory> CorkboardCategories { get; set; }

        [Required(ErrorMessage = "Please Select a Category")]
        [Display(Name ="Category")]
        public byte CategoryTypeId { get; set; }

        [Required]
        [Display(Name = "Visibility")]
        public string VisibilityTypeId { get; set; }

        [Required(ErrorMessage = "Wrong Pin to Authenticate it")]
        [DataType(DataType.Password)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Pin should be four digits")]
        public int Pin { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Timestamp { get; set; }
    }
}