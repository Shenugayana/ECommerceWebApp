using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebApp.Models.Domain
{
    public class Order
    {
        public Order()
        {
            this.Items = new HashSet<OrderItems>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int Total { get; set; }
        public int PaymentId { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }

        public ICollection<OrderItems> Items { get; set; }
    }
}