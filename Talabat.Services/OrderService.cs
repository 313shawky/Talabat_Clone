using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository, 
            IUnitOfWork unitOfWork, 
            IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodId, Address ShippingAddress)
        {
            // 1. Get Basket From Basket Repository
            var Basket = await _basketRepository.GetBasketAsync(BasketId);

            // 2. Get Selected Items At Basket From Product Repository
            var OrderItems = new List<OrderItem>();
            if (Basket?.Items.Count > 0)
            {
                foreach (var Item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(Item.Id);
                    var ProductItemOrdered = new ProductItemOrdered(Product.Id, Product.Name, Product.PictureUrl);
                    var OrderItem = new OrderItem(ProductItemOrdered, Product.Price, Item.Quantity);
                    OrderItems.Add(OrderItem);
                }
            }

            // 3. Calculate SubTotal
            var SubTotal = OrderItems.Sum(Item => Item.Price * Item.Quantity);

            // 4. Get Delivery Method From DeliveryNethod Repository
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);

            // 5. Create Order
            var OrdersRepo = _unitOfWork.Repository<Order>();
            var OrderSpecs = new OrderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExistedOrder = await OrdersRepo.GetEntityWithSpecAsync(OrderSpecs);
            if (ExistedOrder != null)
            {
                OrdersRepo.Delete(ExistedOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(BasketId);
            }
            var Order = new Order(BuyerEmail, ShippingAddress, DeliveryMethod, OrderItems, SubTotal, Basket.PaymentIntentId);

            // 6. Add Order Locally
            await OrdersRepo.AddAsync(Order);

            // 7. Save Order To Database
            var Result = await _unitOfWork.CompleteAsync();

            if (Result <= 0) return null;
            return Order;
        }

        public async Task<Order> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int OrderId)
        {
            var Spec = new OrderSpecifications(BuyerEmail, OrderId);
            var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
            return Order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string BuyerEmail)
        {
            var Spec = new OrderSpecifications(BuyerEmail);
            var Orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Orders;
        }
    }
}
