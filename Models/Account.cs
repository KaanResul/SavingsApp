using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiOgrenicem.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign Key to IdentityUser
        public string Name { get; set; }
        public decimal? TargetAmount { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AnnualInterestRate { get; set; }
        public decimal? MonthlyContribution { get; set; }  
        public DateTime StartDate { get; set; } 
        
        // Navigation Properties
        public ICollection<Transaction> Transactions { get; set; }
    }
}