using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Emailaddress { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
