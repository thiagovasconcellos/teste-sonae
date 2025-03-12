namespace TestMcSonae.Repositories
{
    public abstract class InMemoryRepository<T> : IRepository<T> where T : class
    {
        protected readonly List<T> _entities = new List<T>();
        protected readonly Func<T, Guid> _idSelector;

        protected InMemoryRepository(Func<T, Guid> idSelector)
        {
            _idSelector = idSelector;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _entities.ToList();
        }

        public virtual T GetById(Guid id)
        {
            return _entities.FirstOrDefault(e => _idSelector(e) == id);
        }

        public virtual void Add(T entity)
        {
            _entities.Add(entity);
        }

        public virtual void Update(T entity)
        {
            var index = _entities.FindIndex(e => _idSelector(e) == _idSelector(entity));
            if (index >= 0)
            {
                _entities[index] = entity;
            }
        }

        public virtual void Delete(Guid id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _entities.Remove(entity);
            }
        }
    }
}