using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Repositories
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        private readonly SchoolDbContext _context;

        public AlertRepository(SchoolDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAlertAsync(Alert alert)
        {
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Alert>> GetActiveAlertsAsync()
        {
            return await _context.Alerts
                                 .Where(a => !a.IsResolved)
                                 .Include(a => a.Employee)
                                 .ToListAsync();
        }

        public async Task<Alert> GetAlertByIdAsync(int id)
        {
            return await _context.Alerts.FindAsync(id);
        }

        public async Task MarkAsResolvedAsync(int id)
        {
            var alert = await GetAlertByIdAsync(id);
            if (alert != null)
            {
                alert.IsResolved = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Alert>> GetAlertsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Alerts
                                 .Where(a => a.EmployeeId == employeeId)
                                 .Include(a => a.Employee) // Include employee data
                                 .ToListAsync();
        }

    }
}
