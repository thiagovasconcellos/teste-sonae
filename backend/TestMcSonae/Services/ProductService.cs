using TestMcSonae.Models;
using TestMcSonae.Repositories;

namespace TestMcSonae.Services
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product GetProductById(Guid id);
        void UpdateProductStock(Guid productId, float quantity);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        
        public List<Product> GetAllProducts()
        {
            return _productRepository.GetAll().Where(p => p.IsActive).ToList();
        }
        
        public Product GetProductById(Guid id)
        {
            return _productRepository.GetActiveProductById(id);
        }
        
        public void UpdateProductStock(Guid productId, float quantity)
        {
            var product = GetProductById(productId);
            if (product != null)
            {
                product.InStock = quantity;
                product.UpdatedAt = DateTime.UtcNow;
                _productRepository.Update(product);
            }
        }
    }
}