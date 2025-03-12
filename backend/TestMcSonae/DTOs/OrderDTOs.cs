using System;
using System.Collections.Generic;
using TestMcSonae.Models;

namespace TestMcSonae.DTOs
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public string StatusName { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }

    public class OrderItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductDescription { get; set; }
        public float Quantity { get; set; }
        public float UnitPrice { get; set; }
    }

    public class CreateOrderDTO
    {
        public List<CreateOrderItemDTO> Items { get; set; } = new List<CreateOrderItemDTO>();
    }

    public class CreateOrderItemDTO
    {
        public Guid ProductId { get; set; }
        public float Quantity { get; set; }
    }
}