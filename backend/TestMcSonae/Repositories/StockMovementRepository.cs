using TestMcSonae.Models;

namespace TestMcSonae.Repositories
{
    public interface IStockMovementRepository : IRepository<StockMovement>
    {
        IEnumerable<StockMovement> GetExpiredReservations(DateTime currentTime);
    }

    public class StockMovementRepository : InMemoryRepository<StockMovement>, IStockMovementRepository
    {
        public StockMovementRepository() : base(movement => movement.Id)
        {
        }

        public IEnumerable<StockMovement> GetExpiredReservations(DateTime currentTime)
        {
            return _entities
                .Where(m => m.Type == MovementType.Reservation && 
                           m.IsActive && 
                           m.ExpiresAt.HasValue && 
                           m.ExpiresAt.Value < currentTime)
                .ToList();
        }
    }
}