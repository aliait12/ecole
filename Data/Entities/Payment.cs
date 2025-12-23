using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Data.Entities
{
    public class Payment : IEntity
    {
        public int Id { get; set; }


        public int StudentId { get; set; }

        public Student Student { get; set; }


        [Range(0.01, 10000.00)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }


        public DateTime PaymentDate { get; set; }

        [MaxLength(20)] 
        public required string Status { get; set; }

        [MaxLength(50)]
        public required string TransactionId { get; set; }

        [MaxLength(20)]
        public required string PaymentMethod { get; set; }
    }
}
