using CollegeWebApplication.Data;
using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Repository
{
    public class StudentMasterEFRepository : IStudentMasterEFRepository
    {
        private readonly CollegeWebDbContext _context;
        private readonly ILogger<StudentMasterEFRepository> _logger;

        public StudentMasterEFRepository(CollegeWebDbContext context, ILogger<StudentMasterEFRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        public async Task<List<StudentMaster>> GetAllStudentMastersAsync()
        {
            try
            {
                // Include navigation properties if they exist on the model
                return await _context.Set<StudentMaster>()
                    .AsNoTracking()
                    .Include(sm => sm.StateMaster!)
                    .Include(sm => sm.CityMaster!)
                    .Include(sm => sm.CourseMaster!)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving all StudentMasters");
                // Fallback: try returning without includes if navigation properties aren't registered
                try
                {
                    return await _context.Set<StudentMaster>()
                        .AsNoTracking()
                        .ToListAsync();
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<StudentMaster> GetStudentMasterByIdAsync(int id)
        {
            try
            {
                // Use FindAsync then explicit load if needed to avoid multiple joins when unnecessary
                var student = await _context.Set<StudentMaster>()
                    .AsNoTracking()
                    .Include(sm => sm.StateMaster!)
                    .Include(sm => sm.CityMaster!)
                    .Include(sm => sm.CourseMaster!)
                    .FirstOrDefaultAsync(s => s.StudentId == id);

                return student!;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving StudentMaster id {Id}", id);
                throw;
            }
        }

        public async Task AddStudentMasterAsync(StudentMaster studentMaster)
        {
            if (studentMaster == null) throw new ArgumentNullException(nameof(studentMaster));

            try
            {
                await _context.Set<StudentMaster>().AddAsync(studentMaster);
                await _context.SaveChangesAsync();
                _logger?.LogInformation("Added StudentMaster (Id: {Id})", studentMaster.StudentId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding StudentMaster {@StudentMaster}", studentMaster);
                throw;
            }
        }

        public async Task UpdateStudentMasterAsync(StudentMaster studentMaster)
        {
            if (studentMaster == null) throw new ArgumentNullException(nameof(studentMaster));

            try
            {
                // Attach then mark modified to ensure proper update even if not tracked
                var entry = _context.Set<StudentMaster>().Local.FirstOrDefault(e => e.StudentId == studentMaster.StudentId);
                if (entry == null)
                {
                    _context.Attach(studentMaster);
                    _context.Entry(studentMaster).State = EntityState.Modified;
                }
                else
                {
                    _context.Entry(entry).CurrentValues.SetValues(studentMaster);
                }

                await _context.SaveChangesAsync();
                _logger?.LogInformation("Updated StudentMaster (Id: {Id})", studentMaster.StudentId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger?.LogError(ex, "Concurrency error updating StudentMaster {@StudentMaster}", studentMaster);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating StudentMaster {@StudentMaster}", studentMaster);
                throw;
            }
        }

        public async Task DeleteStudentMasterAsync(int id)
        {
            try
            {
                var entity = await _context.Set<StudentMaster>().FindAsync(id);
                if (entity == null)
                {
                    _logger?.LogWarning("Delete attempted but StudentMaster not found for id {Id}", id);
                    return;
                }

                _context.Set<StudentMaster>().Remove(entity);
                await _context.SaveChangesAsync();
                _logger?.LogInformation("Deleted StudentMaster (Id: {Id})", id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting StudentMaster id {Id}", id);
                throw;
            }
        }

        public async Task<List<StateMaster>> GetAllStateAsync()
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
                return new List<StateMaster>();
            }
        }

        public async Task<List<CityMaster>> GetAllCityAsync(int stateId)
        {
            try
            {
                // Include navigation properties if they exist on the model
                return await _context.Set<CityMaster>()
                         .AsNoTracking()
                         .Where(c => c.StateId == stateId)
                         .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving all StudentMasters");
                return new List<CityMaster>();
            }
        }

        public async Task<List<CourseMaster>> GetAllCourseAsync()
        {
            try
            {
                // Include navigation properties if they exist on the model
                return await _context.Set<CourseMaster>()
                         .AsNoTracking()
                         .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving all StudentMasters");
                return new List<CourseMaster>();
            }
        }
    }
}