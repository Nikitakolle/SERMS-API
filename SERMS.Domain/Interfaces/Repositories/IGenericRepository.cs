using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SERMS.Domain.Interfaces.Repositories
{

    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> GetAllAsync();                    
        Task<T> GetByIdAsync(int id);                            
        Task AddAsync(T entity);                                 
        void Update(T entity);                                 
        void Delete(T entity);                                  
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); 
        Task SaveChangesAsync();
    }
}