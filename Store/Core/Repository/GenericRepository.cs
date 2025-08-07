using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.DTO.Pagination;
using Store.Extensions;
using Store.Middlewares;
using Store.Models;
using Store.Specification;
using System.Linq.Expressions;

namespace Store.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly StoreDbContext context;
        protected readonly DbSet<T> dbSet;
        protected readonly IMapper _map;

        public GenericRepository(StoreDbContext context, IMapper map)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
            _map = map;
        }

        public Func<IQueryable<T>, IQueryable<T>> InjectSpecification<TFilter>(
            IGenericFiltersBuilder<T, TFilter> filterBuilder, TFilter filter, ISpecification<T> specification = null)
            where TFilter : class
        {
            return query =>
            {
                return specification == null
                    ? filterBuilder.Apply(filter, query)
                    : specification.Apply(filterBuilder.Apply(filter, query));
            };
        }

        public Func<IQueryable<T>, IQueryable<T>> InjectSpecification(
            ISpecification<T> specification)
        {
            return specification.Apply;
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", int take = 0,
            Func<IQueryable<T>, IQueryable<T>> extendQuery = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (extendQuery != null)
            {
                query = extendQuery(query);
            }

            if (orderBy != null)
            {
                if (take != 0) return await query.Take(take).ToListAsync();
                return await orderBy(query).ToListAsync();
            }
            else
            {
                if (take != 0) return await query.Take(take).ToListAsync();
                return await query.ToListAsync();
            }
        }

        public async Task<int> GetCount(Expression<Func<T, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.CountAsync();
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, string includeProperties = "",
            Func<IQueryable<T>, IQueryable<T>> extendQuery = null, bool ValidiateNotFound = false)
        {
            IQueryable<T> query = dbSet;

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (extendQuery != null)
            {
                query = extendQuery(query);
            }
            var entity = await query.AsNoTracking().FirstOrDefaultAsync(expression);
            if (ValidiateNotFound && entity == null)
                throw new HusseinErrorResponseException("NOT FOUND", StatusCodes.Status404NotFound);
            return entity;
        }

        public async Task Insert(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task Delete(object id)
        {
            T entityToDelete = await dbSet.FindAsync(id);
            if (entityToDelete == null)
                throw new Exception("");
            if (entityToDelete is ISoftDelete)
            {
                ISoftDelete entity = (ISoftDelete)entityToDelete;
                entity.ArchiveDate = DateTime.UtcNow.ToLocalTime();
                entity.IsArchived = 1;
                dbSet.Update((T)entity);
            }
            else
            {
                dbSet.Remove(entityToDelete);
            }
        }

        public async Task DeleteRange(List<T> entities)
        {
            foreach (T entity in entities)
            {
                await this.Delete(entity);
            }
            //dbSet.RemoveRange(entities);
        }

        public async Task Delete(T entityToDelete)
        {
            if (entityToDelete is ISoftDelete)
            {
                ISoftDelete entity = (ISoftDelete)entityToDelete;
                entity.ArchiveDate = DateTime.UtcNow.ToLocalTime();
                entity.IsArchived = 1;
                dbSet.Update((T)entity);
            }
            else
            {
                dbSet.Remove(entityToDelete);
            }
            //dbSet.Remove(entityToDelete);
            // return null;
        }
        public async Task DeleteRange(Expression<Func<T, bool>> filter)
        {
            var entities = dbSet.Where(filter).ToList();
            var tt = typeof(T);
            var t2 = tt.GetInterfaces();
            var IsSoftDelete = t2.Contains(typeof(ISoftDelete));
            //bool IsSoftDelete = typeof(T).GetInterfaces().Any(x =>
            // // x.IsGenericType &&
            //  x.GetGenericTypeDefinition() == typeof(IEC_SoftDelete));
            if (IsSoftDelete)
            {
                foreach (var entityToDelete in entities)
                {
                    ISoftDelete entity = (ISoftDelete)entityToDelete;
                    entity.ArchiveDate = DateTime.UtcNow;
                    entity.IsArchived = 1;
                    dbSet.Update((T)entity);
                }
            }
            else
                dbSet.RemoveRange(entities);
        }
        public async Task Update(T entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public async Task InsertRange(List<T> entity)
        {
            await dbSet.AddRangeAsync(entity);
        }

        public Task<PagedList<T>> GetAllWithPagination(PaginationParams paginationParams,
            Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", Func<IQueryable<T>, IQueryable<T>> extendQuery = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (extendQuery != null)
            {
                query = extendQuery(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.AsNoTracking().CreateAsync(paginationParams.PageNumber, paginationParams.PageSize);
        }


    }
}
