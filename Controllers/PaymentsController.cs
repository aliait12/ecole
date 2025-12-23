using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Repositories;

namespace SchoolManagementSystem.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var payments = await _paymentRepository.GetAll().ToListAsync();
            return View(payments);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _paymentRepository.GetByIdAsync(id.Value);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,Amount,PaymentDate,Status,TransactionId,PaymentMethod")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                await _paymentRepository.AddPaymentAsync(payment);
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _paymentRepository.GetByIdAsync(id.Value);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,Amount,PaymentDate,Status,TransactionId,PaymentMethod")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _paymentRepository.UpdateAsync(payment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PaymentExists(payment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _paymentRepository.GetByIdAsync(id.Value);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment != null)
            {
                await _paymentRepository.DeleteAsync(payment);
            }

            return RedirectToAction(nameof(Index));
        }


        // Checks if the payment exists
        private async Task<bool> PaymentExists(int id)
        {
            return await _paymentRepository.ExistAsync(id);
        }

        // GET: Payments/Pending
        public async Task<IActionResult> Pending()
        {
            var pendingPayments = await _paymentRepository.GetPendingPaymentsAsync();
            return View(pendingPayments);
        }
    }
}