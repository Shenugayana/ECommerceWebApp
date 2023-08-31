using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebApp.Models.Domain
{
    public class Cart
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }        

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
