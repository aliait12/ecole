using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Repositories
{
    // Generic repository for CRUD operations on entities of type "T"
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly SchoolDbContext _context;
        private SchoolDbContext context;

        // Constructor to initialize the database context
        public GenericRepository(SchoolDbContext context)
        {
            _context = context;
        }

        

        // Returns all entities of type "T" without tracking
        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        // Asynchronously retrieves an entity by its ID
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        // Asynchronously creates a new entity
        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity); // Add entity
            await SaveAllAsync(); // Save changes
        }

        // Asynchronously updates an existing entity
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity); // Update entity
            await SaveAllAsync(); // Save changes
        }

        // Asynchronously deletes an entity
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity); // Remove entity
            await SaveAllAsync(); // Save changes
        }

        // Checks if an entity exists by its ID
        public async Task<bool> ExistAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id); // Check existence
        }

        // Saves all changes to the database
        private async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; // Return success status
        }
    }
}
