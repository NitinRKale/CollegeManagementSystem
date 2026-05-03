using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace CollegeWebApplication.Data
{
    public class CollegeWebDbContext : IdentityDbContext<ApplicationUser>
    {
        public CollegeWebDbContext(DbContextOptions<CollegeWebDbContext> options) : base(options)
        {

        }

        public DbSet<StudentInfo> StudentsInfo { get; set; }

        public DbSet<CountryMaster> CountryMaster { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<StateMaster> StateMaster { get; set; }

        public DbSet<CityMaster> CityMaster { get; set; }
        public DbSet<CourseMaster> CourseMaster { get; set; }

        public DbSet<StudentMaster> StudentMaster { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentMaster>().Property<DateTime?>("CreateDate");
            modelBuilder.Entity<StudentMaster>().Property<DateTime?>("UpdatedDate");
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries())
            {
                // Only apply timestamps for StudentMaster
                if (entry.Entity is StudentMaster)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreateDate").CurrentValue = DateTime.UtcNow;
                        //entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow; // ensure not MinValue
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow;
                    }
                }
            }

            AddAuditLogs();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries())
            {
                // Only apply timestamps for StudentMaster
                if (entry.Entity is StudentMaster)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreateDate").CurrentValue = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow;
                    }
                }
            }

            AddAuditLogs();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditLogs()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList();

            if (!entries.Any())
                return;

            var auditEntries = new List<AuditLog>();
            foreach (var entry in entries)
            {
                var audit = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow
                };

                // Key values
                var keyProps = entry.Properties.Where(p => p.Metadata.IsPrimaryKey());
                var keyValues = keyProps.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                audit.KeyValues = JsonSerializer.Serialize(keyValues);

                if (entry.State == EntityState.Added)
                {
                    var newValues = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    audit.NewValues = JsonSerializer.Serialize(newValues);
                }
                else if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object?>();
                    var newValues = new Dictionary<string, object?>();
                    foreach (var prop in entry.Properties)
                    {
                        oldValues[prop.Metadata.Name] = prop.OriginalValue;
                        newValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    audit.OldValues = JsonSerializer.Serialize(oldValues);
                    audit.NewValues = JsonSerializer.Serialize(newValues);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    var oldValues = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    audit.OldValues = JsonSerializer.Serialize(oldValues);
                }

                auditEntries.Add(audit);
            }

            if (auditEntries.Any())
            {
                // Add audit entries to the context so they get saved with the rest of changes.
                AuditLogs.AddRange(auditEntries);
            }
        }
    }
}


#region Old code 
/*
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CollegeWebApplication.Data
{
    public class CollegeWebDbContext: IdentityDbContext<ApplicationUser>
    {
        public CollegeWebDbContext(DbContextOptions<CollegeWebDbContext> options) : base(options)
        {

        }

        public DbSet<StudentInfo> StudentsInfo { get; set; }

        public DbSet<CountryMaster> CountryMaster { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<StateMaster> StateMaster { get; set; }

        public DbSet<CityMaster> CityMaster { get; set; }
        public DbSet<CourseMaster> CourseMaster { get; set; }

        public DbSet<StudentMaster> StudentMaster { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentMaster>().Property<DateTime?>("CreateDate");
            modelBuilder.Entity<StudentMaster>().Property<DateTime?>("UpdatedDate");
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreateDate").CurrentValue = DateTime.UtcNow;
                    //entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow; // ensure not MinValue
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow;
                }
            }

            AddAuditLogs();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreateDate").CurrentValue = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow;
                }
            }

            AddAuditLogs();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditLogs()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList();

            if (!entries.Any())
                return;

            var auditEntries = new List<AuditLog>();
            foreach (var entry in entries)
            {
                var audit = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow
                };

                // Key values
                var keyProps = entry.Properties.Where(p => p.Metadata.IsPrimaryKey());
                var keyValues = keyProps.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                audit.KeyValues = JsonSerializer.Serialize(keyValues);

                if (entry.State == EntityState.Added)
                {
                    var newValues = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    audit.NewValues = JsonSerializer.Serialize(newValues);
                }
                else if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object?>();
                    var newValues = new Dictionary<string, object?>();
                    foreach (var prop in entry.Properties)
                    {
                        oldValues[prop.Metadata.Name] = prop.OriginalValue;
                        newValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    audit.OldValues = JsonSerializer.Serialize(oldValues);
                    audit.NewValues = JsonSerializer.Serialize(newValues);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    var oldValues = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    audit.OldValues = JsonSerializer.Serialize(oldValues);
                }

                auditEntries.Add(audit);
            }

            if (auditEntries.Any())
            {
                // Add audit entries to the context so they get saved with the rest of changes.
                AuditLogs.AddRange(auditEntries);
            }
        }
    }
}
*/
#endregion Old code
