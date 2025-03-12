using System.Linq;
using TestMcSonae.DTOs;
using TestMcSonae.Models;
using TestMcSonae.Services;

namespace TestMcSonae.Mapping
{
    public class OrderMapper
    {
        private readonly IProductService _productService;

        public OrderMapper(IProductService productService)
        {
            _productService = productService;
        }

        public OrderDTO MapToDTO(Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                Status = order.Status,
                StatusName = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ExpiresAt = order.ExpiresAt,
                Items = order.Items.Select(item => 
                {
                    var product = _productService.GetProductById(item.ProductId);
                    return new OrderItemDTO
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductDescription = product?.ProductDescription ?? "Unknown Product",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                }).ToList()
            };
        }
    }
}