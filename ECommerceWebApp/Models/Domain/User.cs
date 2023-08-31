using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Models.Domain
{
    public class User
    {
        public User()
        {
            this.Carts = new HashSet<Cart>();
            this.Orders = new HashSet<Order>();
        }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }

        public int Id { get; set; }

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
        public DateTime Date_of_Join { get; set; }
    }
}
