namespace ECommerceWebApp.Models.Domain
{
    public class Payment
    {
        public Payment()
        {
            this.Orders = new HashSet<Order>();
        }
        public ICollection<Order> Orders { get; set; }

        public int Id { get; set; }
        public string? Provider { get; set; }
        public DateTime Date { get; set; }
    }
}
