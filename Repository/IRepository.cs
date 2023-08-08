namespace Repository;

using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

public interface IRepository<T> where T : PersistentObject
{
    IQueryable<T> Get();
    Task<IQueryable<T>> GetAsync();
    IQueryable<T> GetAll();
    Task<IQueryable<T>> GetAllAsync();
    void Create(T item);
    Task CreateAsync(T item);
    void Delete(T entity);
    void Update(T entity, string ip);
}
