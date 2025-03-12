using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using Moq;
using TestMcSonae.Configurations;
using TestMcSonae.Models;
using TestMcSonae.Repositories;
using TestMcSonae.Services;
using Xunit;
using FluentAssertions;

namespace TestMcSonae.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IStockService> _stockServiceMock;
        private readonly ReservationSettings _reservationSettings;
        private readonly OrderService _orderService;
        
        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productServiceMock = new Mock<IProductService>();
            _stockServiceMock = new Mock<IStockService>();
            
            _reservationSettings = new ReservationSettings
            {
                ReservationTimeoutSeconds = 5
            };
            
            var optionsMock = new Mock<IOptions<ReservationSettings>>();
            optionsMock.Setup(o => o.Value).Returns(_reservationSettings);
            
            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _productServiceMock.Object,
                _stockServiceMock.Object,
                optionsMock.Object
            );
        }
        
        [Fact]
        public void CreateOrder_WithValidItems_ShouldCreateOrderSuccessfully()
        {
            
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                ProductDescription = "Test Product",
                Value = 10.0f,
                InStock = 5
            };
            
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = 2
                }
            };
            
            _productServiceMock.Setup(p => p.GetProductById(productId)).Returns(product);
            _stockServiceMock.Setup(s => s.ReserveStock(productId, 2)).Returns(true);
            
            
            string errorMessage;
            var result = _orderService.CreateOrder(orderItems, out errorMessage);
            
            
            result.Should().NotBeNull();
            errorMessage.Should().BeNull();
            result.Status.Should().Be(OrderStatus.Created);
            result.Items.Should().HaveCount(1);
            result.Items.First().UnitPrice.Should().Be(10.0f);
            
            _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
        }
        
        [Fact]
        public void CreateOrder_WithInvalidProductId_ShouldReturnError()
        {
            
            var productId = Guid.NewGuid();
            
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = 2
                }
            };
            
            _productServiceMock.Setup(p => p.GetProductById(productId)).Returns((Product)null);
            
            
            string errorMessage;
            var result = _orderService.CreateOrder(orderItems, out errorMessage);
            
            
            result.Should().BeNull();
            errorMessage.Should().Contain("not found");
            
            _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
        }
        
        [Fact]
        public void CreateOrder_WithInsufficientStock_ShouldReturnError()
        {
            
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                ProductDescription = "Test Product",
                Value = 10.0f,
                InStock = 5
            };
            
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = 10
                }
            };
            
            _productServiceMock.Setup(p => p.GetProductById(productId)).Returns(product);
            _stockServiceMock.Setup(s => s.ReserveStock(productId, 10)).Returns(false);
            
            
            string errorMessage;
            var result = _orderService.CreateOrder(orderItems, out errorMessage);
            
            
            result.Should().BeNull();
            errorMessage.Should().Contain("Insufficient stock");
            
            _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
        }
        
        [Fact]
        public void Order_ShouldExpireAfterTimeout()
        {
            
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(_reservationSettings.ReservationTimeoutSeconds),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Quantity = 2
                    }
                }
            };
            
            _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);
            
            order.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            
            Thread.Sleep((_reservationSettings.ReservationTimeoutSeconds + 1) * 1000);
            
            order.ExpiresAt.Should().BeBefore(DateTime.UtcNow);
        }
        
        [Fact]
        public void CancelOrder_ShouldReleaseStock()
        {
            
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Created,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Quantity = 2
                    }
                }
            };
            
            _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);
            
            
            string errorMessage;
            var result = _orderService.CancelOrder(orderId, out errorMessage);
            
            
            result.Should().NotBeNull();
            errorMessage.Should().BeNull();
            result.Status.Should().Be(OrderStatus.Cancelled);
            
            _stockServiceMock.Verify(s => s.ReleaseReservation(productId, 2), Times.Once);
            _orderRepositoryMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Once);
        }
        
        [Fact]
        public void StockShouldBeReturnedAfterOrderExpiration()
        {            
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow.AddSeconds(-10),
                ExpiresAt = DateTime.UtcNow.AddSeconds(-5),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Quantity = 3
                    }
                }
            };
            
            _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);
            
            
            string errorMessage;
            var result = _orderService.CancelOrder(orderId, out errorMessage);
            
            
            result.Should().NotBeNull();
            errorMessage.Should().BeNull();
            
            _stockServiceMock.Verify(s => s.ReleaseReservation(productId, 3), Times.Once);
        }
    }
}