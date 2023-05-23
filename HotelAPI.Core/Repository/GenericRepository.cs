using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelAPI.Core.Contracts;
using HotelAPI.Core.Exceptions;
using HotelAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        readonly HotelDbContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(HotelDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException();
            this._mapper = mapper;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource source)
        {
            var entity = _mapper.Map<T>(source);

            await AddAsync(entity);
            return _mapper.Map<TResult>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if(entity == null) throw new NotFoundException(typeof(T).Name, id);

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

        public async Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
        {
            var totalSize = await _context.Set<T>().CountAsync();
            var items = await _context.Set<T>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<TResult> {
                Items = items, 
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
        }

        public async Task<List<TResult>> GetAllAsync<TResult>()
        {
            return await _context.Set<T>()
                .ProjectTo<TResult>( _mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id == null) return null;

            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<TResult?> GetAsync<TResult>(int? id)
        {
            if (id is null)
                return default;

            var result = await _context.Set<T>().FindAsync(id);
            if(result is null)
                throw new NotFoundException(typeof(T).Name, id);

            return _mapper.Map<TResult>(result);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<TSource>(int id, TSource source)
        {
            var entity = await GetAsync(id);
            if (entity == null) throw new NotFoundException(typeof(T).Name, id);

            _mapper.Map(entity, source);
            await UpdateAsync(entity);
        }
    }
}
