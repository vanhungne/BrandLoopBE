namespace BrandLoop.Infratructure.Configurations
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        Task Add(TEntity entity);
        Task Add(IEnumerable<TEntity> entities);

        TEntity GetById(object id);

        IQueryable<TEntity> GetAll();

        Task Update(TEntity entity);

        Task Update(IEnumerable<TEntity> entities);

        void Remove(int id);

        void Remove(TEntity entity);

        void Remove(params TEntity[] entities);

        void Remove(IEnumerable<TEntity> entities);
    }
}
