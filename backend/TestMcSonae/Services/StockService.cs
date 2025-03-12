using Microsoft.Extensions.Options;
using TestMcSonae.Configurations;
using TestMcSonae.Events;
using TestMcSonae.Models;
using TestMcSonae.Repositories;

namespace TestMcSonae.Services
{
    public interface IStockService
    {
        bool ReserveStock(Guid productId, float quantity);
        void ReleaseReservation(Guid productId, float quantity);
        void InitializeExpirationTimer();
    }

    public class StockService : IStockService
    {
        private readonly IProductService _productService;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ReservationSettings _reservationSettings;
        private Timer _reservationTimer;
        
        public StockService(
            IProductService productService, 
            IStockMovementRepository stockMovementRepository,
            IOrderRepository orderRepository,
            IOptions<ReservationSettings> reservationSettings)
        {
            _productService = productService;
            _stockMovementRepository = stockMovementRepository;
            _orderRepository = orderRepository;
            _reservationSettings = reservationSettings.Value;
        }
        
        public void InitializeExpirationTimer()
        {
            _reservationTimer = new Timer(CheckExpiredItems, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            
            OrderEvents.OrderExpired += (sender, args) => {
                var order = args.Order;
                foreach (var item in order.Items)
                {
                    ReleaseReservation(item.ProductId, item.Quantity);
                }
            };
        }
        
        public bool ReserveStock(Guid productId, float quantity)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.InStock < quantity)
            {
                return false;
            }
            
            var movement = new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = quantity,
                Type = MovementType.Reservation,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(_reservationSettings.ReservationTimeoutSeconds),
                IsActive = true
            };
            
            _stockMovementRepository.Add(movement);
            
            _productService.UpdateProductStock(productId, product.InStock - quantity);
            
            return true;
        }
        
        public void ReleaseReservation(Guid productId, float quantity)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                return;
            }
            
            var movement = new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = quantity,
                Type = MovementType.ReservationRelease,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            _stockMovementRepository.Add(movement);
            
            _productService.UpdateProductStock(productId, product.InStock + quantity);
        }
        
        private void CheckExpiredItems(object? state)
        {
            var now = DateTime.UtcNow;
            var expiredReservations = _stockMovementRepository.GetExpiredReservations(now);
            
            foreach (var reservation in expiredReservations)
            {
                reservation.IsActive = false;
                _stockMovementRepository.Update(reservation);
            }
            
            var expiredOrders = _orderRepository.GetExpiredOrders(now);
            foreach (var order in expiredOrders)
            {
                order.Status = OrderStatus.Expired;
                order.UpdatedAt = DateTime.UtcNow;
                _orderRepository.Update(order);
                
                OrderEvents.RaiseOrderExpired(order);
            }
        }
    }
}