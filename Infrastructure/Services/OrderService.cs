using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, ShippingAddress address)
        {
            //Get Basket
            var basket = await _basketRepository.GetBasketAsync(basketId);

            //Get BasketItems from product repository
            var items = new List<OrderItem>();

            foreach(var item in basket.basketItems)
            {
                var producrItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                var itemOrdered = new ProductItemOrdered(producrItem.Id, producrItem.Name, producrItem.PictureUrl);

                var orderItem = new OrderItem(itemOrdered, producrItem.Price, item.Quantity);

                items.Add(orderItem);
            }

            //Get DeliveryMethod
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            //Calculate subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            //Payment Stuff
            //Check to see if order exists
            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);

            if(existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentTntent(basketId);
            }

            //create order
            var order = new Order(buyerEmail, address, deliveryMethod, items, subtotal , basket.PaymentIntentId);

            _unitOfWork.Repository<Order>().Add(order);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return null;

            //Delete Basket

            await _basketRepository.DeleteasketAsync(basketId);

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var orderSpec = new OrderWithItemSpecification(id, buyerEmail);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpecifications(orderSpec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var orderSpec = new OrderWithItemSpecification(buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsync(orderSpec);
        }
    }
}
