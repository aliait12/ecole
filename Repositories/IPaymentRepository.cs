using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByStudentIdAsync(int studentId);
        Task AddPaymentAsync(Payment payment);
        Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
    }
}
