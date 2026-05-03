using CollegeWebApplication.Data;
using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebApplication.Repository
{
    public class StateMasterEFRepository : IStateMasterEFRepository
    {
        private readonly CollegeWebDbContext _context;
        private readonly ILogger<StateMasterEFRepository> _logger;
        public StateMasterEFRepository(CollegeWebDbContext context, ILogger<StateMasterEFRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<StateMaster>> GetAllStateMastersAsync()
        {
            try
            {
                // Include navigation properties if they exist on the model
                return await _context.Set<StateMaster>()
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving all StudentMasters");
                // Fallback: try returning without includes if navigation properties aren't registered
                try
                {
                    return await _context.Set<StateMaster>()
                        .AsNoTracking()
                        .ToListAsync();
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<StateMaster> GetStateMasterByIdAsync(int id)
        {
            try
            {
                // Use FindAsync then explicit load if needed to avoid multiple joins when unnecessary
                var state = await _context.Set<StateMaster>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.StateId == id);

                return state!;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving StateMaster id {Id}", id);
                throw;
            }
        }

        public async Task AddStateMasterAsync(StateMaster stateMaster)
        {
            if (stateMaster == null) throw new ArgumentNullException(nameof(stateMaster));

            try
            {
                await _context.Set<StateMaster>().AddAsync(stateMaster);
                await _context.SaveChangesAsync();
                _logger?.LogInformation("Added StateMaster (Id: {Id})", stateMaster.StateId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding StateMaster {@StateMaster}", stateMaster);
                throw;
            }
        }

        public async Task UpdateStateMasterAsync(StateMaster stateMaster)
        {
            if (stateMaster == null) throw new ArgumentNullException(nameof(stateMaster));

            try
            {
                // Attach then mark modified to ensure proper update even if not tracked
                var entry = _context.Set<StateMaster>().Local.FirstOrDefault(e => e.StateId == stateMaster.StateId);
                if (entry == null)
                {
                    _context.Attach(stateMaster);
                    _context.Entry(stateMaster).State = EntityState.Modified;
                }
                else
                {
                    _context.Entry(entry).CurrentValues.SetValues(stateMaster);
                }

                await _context.SaveChangesAsync();
                _logger?.LogInformation("Updated StateMaster (Id: {Id})", stateMaster.StateId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger?.LogError(ex, "Concurrency error updating StateMaster {@StateMaster}", stateMaster);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating StateMaster {@StateMaster}", stateMaster);
                throw;
            }
        }

        public async Task DeleteStateMasterAsync(int id)
        {
            try
            {
                var entity = await _context.Set<StateMaster>().FindAsync(id);
                if (entity == null)
                {
                    _logger?.LogWarning("Delete attempted but StateMaster not found for id {Id}", id);
                    return;
                }

                _context.Set<StateMaster>().Remove(entity);
                await _context.SaveChangesAsync();
                _logger?.LogInformation("Deleted StateMaster (Id: {Id})", id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting StateMaster id {Id}", id);
                throw;
            }
        }
    }
}
