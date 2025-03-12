using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using TestMcSonae.Configurations;
using TestMcSonae.DTOs;
using TestMcSonae.Models;
using TestMcSonae.Repositories;

namespace TestMcSonae.Services
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        Order GetOrderById(Guid id);
        Order CreateOrder(List<OrderItem> items, out string errorMessage);
        Order ConfirmOrder(Guid orderId, out string errorMessage);
        Order CancelOrder(Guid orderId, out string errorMessage);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;
        private readonly ReservationSettings _reservationSettings;
        
        public OrderService(
            IOrderRepository orderRepository,
            IProductService productService, 
            IStockService stockService, 
            IOptions<ReservationSettings> reservationSettings)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _stockService = stockService;
            _reservationSettings = reservationSettings.Value;
        }
        
        public Order CreateOrder(List<OrderItem> items, out string errorMessage)
        {
            errorMessage = null;
            
            foreach (var item in items)
            {
                var product = _productService.GetProductById(item.ProductId);
                if (product == null)
                {
                    errorMessage = $"Product with ID {item.ProductId} not found";
                    return null;
                }
                
                if (!_stockService.ReserveStock(item.ProductId, item.Quantity))
                {
                    foreach (var processedItem in items.TakeWhile(i => i != item))
                    {
                        _stockService.ReleaseReservation(processedItem.ProductId, processedItem.Quantity);
                    }
                    errorMessage = $"Insufficient stock for product {product.ProductDescription}";
                    return null;
                }
                
                item.UnitPrice = product.Value;
            }
            
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Items = items,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(_reservationSettings.ReservationTimeoutSeconds)
            };
            
            _orderRepository.Add(order);
            return order;
        }
        
        public Order ConfirmOrder(Guid orderId, out string errorMessage)
        {
            errorMessage = null;
            
            var order = _orderRepository.GetById(orderId);
            if (order == null)
            {
                errorMessage = $"Order with ID {orderId} not found";
                return null;
            }
            
            if (order.Status != OrderStatus.Created)
            {
                errorMessage = $"Order with ID {orderId} cannot be confirmed because its status is {order.Status}";
                return null;
            }
            
            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepository.Update(order);
            return order;
        }
        
        public Order CancelOrder(Guid orderId, out string errorMessage)
        {
            errorMessage = null;
            
            var order = _orderRepository.GetById(orderId);
            if (order == null)
            {
                errorMessage = $"Order with ID {orderId} not found";
                return null;
            }
            
            if (order.Status != OrderStatus.Created)
            {
                errorMessage = $"Order with ID {orderId} cannot be cancelled because its status is {order.Status}";
                return null;
            }
            
            foreach (var item in order.Items)
            {
                _stockService.ReleaseReservation(item.ProductId, item.Quantity);
            }
            
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepository.Update(order);
            return order;
        }
        
        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAll().ToList();
        }
        
        public Order GetOrderById(Guid id)
        {
            return _orderRepository.GetById(id);
        }
    }
}