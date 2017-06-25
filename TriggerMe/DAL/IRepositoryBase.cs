using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TriggerMe.DAL
{
    public interface IRepositoryBase<TEntity> where TEntity:class
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity,bool>> filter=null);
        IEnumerable<TEntity> Get<TOrder>(Expression<Func<TEntity, TOrder>> orderby, int size, int skip = 0, Expression<Func<TEntity, bool>> filter = null);
        TEntity GetById(int id);
        TEntity Delete(int id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        int Count(Expression<Func<TEntity, bool>> filter = null);
        TEntity Single(Expression<Func<TEntity, bool>> filter);
        TEntity GetFirst(Expression<Func<TEntity, bool>> filter = null);
        //async
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter=null);
        Task<IEnumerable<TEntity>> GetAsync<TOrder>(Expression<Func<TEntity, TOrder>> orderby, int size, int skip = 0, Expression<Func<TEntity, bool>> filter = null);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> DeleteAsync(int id);
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter = null);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> filter);


    }
}
