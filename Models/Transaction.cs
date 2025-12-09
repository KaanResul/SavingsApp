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
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public Account Account { get; set; }
    }
}