using System.ComponentModel.DataAnnotations;

namespace WebApiOgrenicem.DTOs
{
    public class CreateAccountDto
    {
        [Required]
        public string Name { get; set; }
        public decimal? TargetAmount { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AnnualInterestRate { get; set; }
        public decimal? MonthlyContribution { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
    }

    public class UpdateAccountDto
    {
        [Required]
        public string Name { get; set; }
        public decimal? TargetAmount { get; set; }
        public decimal? AnnualInterestRate { get; set; }
        public decimal? MonthlyContribution { get; set; }
    }

    public class AccountResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public decimal? TargetAmount { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AnnualInterestRate { get; set; }
        public decimal? MonthlyContribution { get; set; }
        public DateTime StartDate { get; set; }
    }
}
