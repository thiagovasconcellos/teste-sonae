using Microsoft.AspNetCore.Mvc;
using TestMcSonae.DTOs;
using TestMcSonae.Models;
using TestMcSonae.Services;
using TestMcSonae.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestMcSonae.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ProductDTO>>> GetAll()
        {
            var products = _productService.GetAllProducts();
            var productDTOs = products.Select(p => new ProductDTO
            {
                Id = p.Id,
                ProductDescription = p.ProductDescription,
                Value = p.Value,
                InStock = p.InStock,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsActive = p.IsActive
            }).ToList();
            
            return this.ApiResponse(ApiResponse<IEnumerable<ProductDTO>>.Create(true, productDTOs));
        }
        
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<ProductDTO>> GetById(Guid id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return this.ApiResponse(ApiResponse<ProductDTO>.Create(false, default, $"Product with ID {id} not found", 404));
            }
            
            return this.ApiResponse(ApiResponse<ProductDTO>.Create(true, MapProductToDTO(product)));
        }
        
        private ProductDTO MapProductToDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                ProductDescription = product.ProductDescription,
                Value = product.Value,
                InStock = product.InStock,
                IsActive = product.IsActive
            };
        }
    }
}