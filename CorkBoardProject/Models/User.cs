//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CorkBoardProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int Password { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public Nullable<int> Followers { get; set; }
        public Nullable<int> Following { get; set; }
    }
}
