using TestMcSonae.Models;

namespace TestMcSonae.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetActiveProductById(Guid id);
    }

    public class ProductRepository : InMemoryRepository<Product>, IProductRepository
    {
        public ProductRepository() : base(product => product.Id)
        {
            var macbook = new Product
            {
                Id = Guid.NewGuid(),
                ProductDescription = "MacBook Pro M3",
                Value = 3445.32f,
                InStock = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            _entities.Add(macbook);
        }

        public Product GetActiveProductById(Guid id)
        {
            return _entities.FirstOrDefault(p => p.Id == id && p.IsActive);
        }
    }
}