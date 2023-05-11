using HotelAPI.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Models.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        readonly HotelDbContext _context;
        public GenericRepository(HotelDbContext context)
        {
            _context = context ?? throw new ArgumentNullException();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();  
        }

        public async Task<bool> ExistAsync(int id)
        {
            var exist = await GetAsync(id);
            return exist != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id == null) return null;

            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
