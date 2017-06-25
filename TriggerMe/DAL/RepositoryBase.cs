using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TriggerMe.Model;
using TriggerMe.Models;
using Z.EntityFramework.Plus;

namespace TriggerMe.DAL
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity:ModelBase
    {
        private readonly DbContext _context;
       
        private DbSet<TEntity> _entities;
        private AppTenant tenant;
        protected Expression<Func<TEntity, bool>> TenantFilter { get; set; }
 
        public RepositoryBase(DbContext context, AppTenant tenant,bool tenancyActive=true)
        { 
            _context = context;
            _entities = context.Set<TEntity>();
            this.tenant = tenant;
            TenantFilter = z => (z.TenantId ?? "") == tenant.Id;
            if (!tenancyActive) TenantFilter = z=>true ;


        }
        public void Add(TEntity entity)
        {
            entity.TenantId = tenant.Id;
            _entities.Add(entity);

        }

        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null ? _entities.Count(TenantFilter) : _entities.Where(TenantFilter).Count(filter);
        }

        public TEntity Delete(int id)
        {
            var entity = _entities.SingleOrDefault(z => z.Id == id && z.TenantId == tenant.Id);
            if (entity == null) return null;
            _entities.Remove(entity);
            return entity;
        }

        public async Task<TEntity> DeleteAsync(int id)
        {
            var entity = await _entities.SingleOrDefaultAsync(z=>z.Id==id && z.TenantId==tenant.Id);
            if (entity == null) return null;
            _entities.Remove(entity);
            return entity;
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            if (filter != null) result = _entities.Where(filter);
            return result.ToList();

        }
        public IEnumerable<TEntity> Get<TOrder>(Expression<Func<TEntity, TOrder>> orderby, int size, int skip = 0,Expression<Func<TEntity, bool>> filter=null   )
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            result = result.OrderBy(orderby);
            if (filter != null) result = result.Where(filter);
            return result.Skip(skip).Take(size).ToList();

        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter=null)
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            if (filter != null) result = _entities.Where(filter);
            return  await result.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAsync<TOrder>(Expression<Func<TEntity, TOrder>> orderby, int size, int skip = 0, Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            result = result.OrderBy(orderby);
            if (filter != null) result = result.Where(filter);
            return await result.Skip(skip).Take(size).ToListAsync();
        }

        public TEntity GetById(int id)
        {
            return _entities.AsNoTracking().SingleOrDefault(z => z.Id == id && z.TenantId == tenant.Id);
        }
        public TEntity GetFirst(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            if (filter != null) result = result.Where(filter);
            return result.FirstOrDefault();
  
        }
        public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            if (filter != null) result = result.Where(filter);
            return await result.FirstOrDefaultAsync();

        }
        public TEntity Single(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            if (filter != null) result = result.Where(filter);
            return result.SingleOrDefault();

        }
        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> result = _entities.Where(TenantFilter);
            if (filter != null) result = result.Where(filter);
            return await result.SingleOrDefaultAsync();

        }
        public async Task<TEntity> GetByIdAsync(int id)
        {
             return await _entities.AsNoTracking().SingleOrDefaultAsync(z => z.Id == id && z.TenantId == tenant.Id);
        }

        public void Update(TEntity entity)
        {
            entity.TenantId = tenant.Id;
            _entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
