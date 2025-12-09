using System.ComponentModel.DataAnnotations;

namespace WebApiOgrenicem.DTOs
{
    public class CreateTransactionDto
    {
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        public string TransactionType { get; set; } // "Deposit", "Withdrawal"
        
        public string Description { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }

    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
    }
}
