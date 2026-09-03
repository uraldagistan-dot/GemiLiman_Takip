using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions; 
using System;
namespace PortTrackingSystem.Core.Repositories
{
    // <T> ifadesi, bu yapının Ship, Port, Cargo gibi tüm sınıflarla çalışabileceğini gösterir.
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);

        // Ic ice iliskiler icin (orn: "CrewAssignments.CrewMember").
        // Lambda tabanli Include tek seviye gidebildigi icin string tabanli surum gerekli.
        Task<IEnumerable<T>> GetAllWithNestedIncludeAsync(params string[] includes);
    }
}
  
