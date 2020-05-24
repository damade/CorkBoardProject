using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CorkBoardProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //Required attribute implements validation on Model item that this fields is mandatory for user
        [Required]
        //We are also implementing Regular expression to check if email is valid like a1@test.com
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Pin should be four digits")]
        public int Pin { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Pin")]
        [Compare("Pin", ErrorMessage = "The pin and confirmation pin do not match.")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Pin should be four digits")]
        public int ConfirmPin { get; set; }
    }
}
