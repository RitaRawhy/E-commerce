using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public class Order : BaseEntity
    {
        public Order() { }
        public Order(string buyerEmail, 
            ShippingAddress shippedToAddress,
            DeliveryMethod deliveryMethod, 
            IReadOnlyList<OrderItem> orderItems, 
            decimal subTotal,
            string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippedToAddress = shippedToAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OederDate { get; set; } = DateTimeOffset.Now;
        public ShippingAddress ShippedToAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItem> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string? PaymentIntentId { get; set; }
        public decimal GetTotal()
            => SubTotal + DeliveryMethod.Price;
    }
}
