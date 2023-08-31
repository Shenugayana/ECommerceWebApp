using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Models.Domain
{
    public class Product
    {
        public Product()
        {
            this.Carts = new HashSet<Cart>();
        }
        public ICollection<Cart> Carts { get; set; }

        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public int Price { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }
        public int Rating { get; set; }
        public string? FImage { get; set; }
        public string? SImage { get; set; }
        public string? TImage { get; set; }
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
