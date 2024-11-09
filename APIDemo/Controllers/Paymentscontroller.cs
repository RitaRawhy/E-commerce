using APIDemo.ResponseModule;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace APIDemo.Controllers
{
    public class Paymentscontroller : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<Paymentscontroller> _logger;
        private const string WHSecret = "whsec_7a623f7ce35ee51bc8e97030ec9370c98bc1ecb9cde17b07080e9c4ea392a18e";

        public Paymentscontroller(
            IPaymentService paymentService ,
            ILogger<Paymentscontroller> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentTntent(basketId);

            if (basket is null)
                return BadRequest(new ApiResponse(400, "Problem with your basket"));

            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WHSecret);

            PaymentIntent intent;
            Order order;

            switch(stripeEvent.Type)
            {
                case Events.PaymentIntentPaymentFailed:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed : ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFaild(intent.Id);
                    _logger.LogInformation("Payment Failed : ", order.Id);
                    break;

                case Events.PaymentIntentSucceeded:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeded : ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceded(intent.Id);
                    _logger.LogInformation("Order Updated to Payment Received : ", order.Id);
                    break;
            }

            return new EmptyResult();
        }
    }
}
