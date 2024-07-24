namespace Motax.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string OrderCode { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Username { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string HowToPay { get; set; } = null!;
        public string HowToTransport { get; set; } = null!;
        public double TransportFee { get; set; }
        public int Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreateDay { get; set; }
        public DateTime UpdateDay { get; set; }
        public double TotalAmount { get; set; }

        public int CarId { get; set; } // New
        public int DealerId { get; set; } // New

        public int OrderStatusId { get; set; }
        public virtual OrderStatus? OrderStatus { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Car Car { get; set; } = null!;
        public virtual Dealer Dealer { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
