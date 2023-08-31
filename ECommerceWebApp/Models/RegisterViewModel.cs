using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Models
{
    public class RegisterViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Emailaddress { get; set; }
        public string? Image { get; set; }

        [Required]
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }

        [Required]
        public string? Address { get; set; }
        public int Contact_Number { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public DateTime Date_of_Join { get; set; }
    }
}
