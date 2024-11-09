using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepository(StoreDbContext context)
        {
            _context = context;
        }
        public void Add(T entity)
            => _context.Set<T>().Add(entity);

        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);

        public async Task<T> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);

        public async Task<T> GetEntityWithSpecifications(ISpecification<T> specification)
            => await ApplySpecification(specification).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<T>> ListAllAsync()
            => await _context.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification)
            => await ApplySpecification(specification).ToListAsync();

        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
            => SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification);

        public void Update(T entity)
            => _context.Set<T>().Update(entity);

        public async Task<int> CountAsync(ISpecification<T> specification)
            => await ApplySpecification(specification).CountAsync();
    }
}
