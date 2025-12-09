using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiOgrenicem.Data;
using WebApiOgrenicem.DTOs;
using WebApiOgrenicem.Models;

namespace WebApiOgrenicem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TransactionController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Transaction
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetTransactions()
        {
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return Unauthorized();

            var transactions = await _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.UserId == user.Id) // Sadece kullanıcının işlemlerini getir
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType,
                    Description = t.Description
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponseDto>> GetTransaction(int id)
        {
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return Unauthorized();

            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (transaction == null)
            {
                return NotFound();
            }

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account.Name,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                TransactionType = transaction.TransactionType,
                Description = transaction.Description
            };
        }

        // POST: api/Transaction
        [HttpPost]
        public async Task<ActionResult<TransactionResponseDto>> PostTransaction(CreateTransactionDto transactionDto)
        {
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return Unauthorized();

            // 1. Validate Account Ownership
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == transactionDto.AccountId && a.UserId == user.Id);

            if (account == null)
            {
                return BadRequest("Invalid Account ID or Account does not belong to user.");
            }

            // 2. Create Transaction
            var transaction = new Transaction
            {
                UserId = user.Id,
                AccountId = transactionDto.AccountId,
                Amount = transactionDto.Amount,
                TransactionType = transactionDto.TransactionType,
                Description = transactionDto.Description,
                TransactionDate = transactionDto.TransactionDate
            };

            // 3. Update Account Balance (Optional but recommended logic)
            if (transactionDto.TransactionType.Equals("Deposit", StringComparison.OrdinalIgnoreCase))
            {
                if (account.CurrentBalance == null) account.CurrentBalance = 0;
                account.CurrentBalance += transactionDto.Amount;
            }
            else if (transactionDto.TransactionType.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase))
            {
                if (account.CurrentBalance == null) account.CurrentBalance = 0;
                
                if (account.CurrentBalance < transactionDto.Amount)
                {
                    return BadRequest("Insufficient funds.");
                }
                account.CurrentBalance -= transactionDto.Amount;
            }

            _context.Transactions.Add(transaction);
            
            // Account balance changed, so it will be updated too
            await _context.SaveChangesAsync();

            var responseDto = new TransactionResponseDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                AccountName = account.Name,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                TransactionType = transaction.TransactionType,
                Description = transaction.Description
            };

            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, responseDto);
        }
    }
}
