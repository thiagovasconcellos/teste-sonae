using TestMcSonae.Models;

namespace TestMcSonae.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetExpiredOrders(DateTime currentTime);
    }

    public class OrderRepository : InMemoryRepository<Order>, IOrderRepository
    {
        public OrderRepository() : base(order => order.Id)
        {
        }

        public IEnumerable<Order> GetExpiredOrders(DateTime currentTime)
        {
            return _entities
                .Where(o => o.Status == OrderStatus.Created && o.ExpiresAt < currentTime)
                .ToList();
        }
    }
}