using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CorkBoardProject.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [HiddenInput]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Pin should be four digits")]
        public int Pin { get; set; }
    }
}