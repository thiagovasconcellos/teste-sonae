using System;
using System.Collections.Generic;

namespace TestMcSonae.Models
{
    public enum OrderStatus
    {
        Created,
        Confirmed,
        Cancelled,
        Expired
    }

    public class Order
    {
        public Guid Id { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public float Quantity { get; set; }
        public float UnitPrice { get; set; }
    }
}