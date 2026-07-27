using StudentManagement.Business.Interfaces;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int? userId, string action)
        {
            if (!userId.HasValue) return;

            await _context.AuditLogs.AddAsync(new AuditLog
            {
                UserId = userId.Value,
                Action = action,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }
}
