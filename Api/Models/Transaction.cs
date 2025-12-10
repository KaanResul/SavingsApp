using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiOgrenicem.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int AccountId { get; set; } // Fixed type mismatch (was string)
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } // "Deposit", "Withdrawal" etc.
        public string Description { get; set; }

        // Navigation Property
        public Account Account { get; set; }
    }
}