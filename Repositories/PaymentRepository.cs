using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly SchoolDbContext _context;

        // Remplacez le constructeur pour lever l'ambiguïté en spécifiant le nom du paramètre
        public PaymentRepository(SchoolDbContext context) : base(context: context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStudentIdAsync(int studentId)
        {
            return await _context.Payments
                .Where(p => p.StudentId == studentId)
                .ToListAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == "Pendente")
                .ToListAsync();
        }
    }
}
