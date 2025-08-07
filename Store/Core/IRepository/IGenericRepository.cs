using System.Linq.Expressions;
using Store.DTO.Pagination;
using Store.Specification;

namespace Store.Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<List<T>> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>,
                IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", int take = 0,
            Func<IQueryable<T>, IQueryable<T>> extendQuery = null);

        public Func<IQueryable<T>, IQueryable<T>> InjectSpecification<TFilter>(
            IGenericFiltersBuilder<T, TFilter> filterBuilder, TFilter filter, ISpecification<T> specification = null)
            where TFilter : class;

        public Func<IQueryable<T>, IQueryable<T>> InjectSpecification(ISpecification<T> specification);

        public Task<T> Get(Expression<Func<T, bool>> expression, string includeProperties = "",
            Func<IQueryable<T>, IQueryable<T>> extendQuery = null, bool ValidiateNotFound = false);

        public Task<int> GetCount(Expression<Func<T, bool>> filter = null, string includeProperties = "");

        public Task Insert(T entity);
        public Task InsertRange(List<T> entity);

        public Task Delete(object id);
        public Task DeleteRange(Expression<Func<T, bool>> filter);


        public Task DeleteRange(List<T> entities);

        public Task Delete(T entityToDelete);

        public Task Update(T entityToUpdate);

        public Task<PagedList<T>> GetAllWithPagination(PaginationParams paginationParams,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>,
                IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", Func<IQueryable<T>, IQueryable<T>> extendQuery = null);
    }
}
