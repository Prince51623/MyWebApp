using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Guest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\+?[1-9][0-9]{7,14}$", ErrorMessage = "Please enter a valid phone number")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters")]
        public required string Address { get; set; }
    }
}