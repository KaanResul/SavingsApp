using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebApiOgrenicem.Data;
using WebApiOgrenicem.DTOs;
using WebApiOgrenicem.Models;

namespace WebApiOgrenicem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(AppDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Account
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts()
        {
            var userId = _userManager.GetUserId(User); // AuthController'da Name claim'ine UserName atamıştık. İsterseniz NameIdentifier ile Id de tutabilirsiniz.

            // Not: İdealde UserId claim'i NameIdentifier olarak tutulur. 
            // AuthController.cs'de: new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) yerine bir de Id eklemek veya UserName üzerinden gitmek
            // Şimdilik UserName üzerinden kullanıcıyı bulup Id'sine erişmemiz gerekebilir VEYA direkt veritabanındaki UserId string mi, Guid mi?
            // IdentityUser varsayılan olarak Guid string kullanır.
            // AuthController'da ClaimTypes.Name olarak UserName attık.
            // Fakat Account tablosunda UserId string tutuyoruz.
            // En temizi: AuthController Login metodunda UserId'yi de claim olarak eklemektir.
            // Şimdilik UserName ile eşleşen kullanıcıyı bulalım.

            // Ancak performans için Claim'e UserId eklemek daha iyidir.
            // Ben şimdilik AuthController'ı değiştirmeden UserName ile user bulup id'sini alacağım.
            // VEYA: AuthController'ı güncelleyelim.
            
            // Kullanıcı adını alalım
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null) return Unauthorized();

            var accounts = await _context.Accounts
                .Where(a => a.UserId == user.Id)
                .Select(a => new AccountResponseDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Name = a.Name,
                    TargetAmount = a.TargetAmount,
                    CurrentBalance = a.CurrentBalance,
                    AnnualInterestRate = a.AnnualInterestRate,
                    MonthlyContribution = a.MonthlyContribution,
                    StartDate = a.StartDate
                })
                .ToListAsync();

            return Ok(accounts);
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponseDto>> GetAccount(int id)
        {
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return Unauthorized();

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);

            if (account == null)
            {
                return NotFound();
            }

            return new AccountResponseDto
            {
                Id = account.Id,
                UserId = account.UserId,
                Name = account.Name,
                TargetAmount = account.TargetAmount,
                CurrentBalance = account.CurrentBalance,
                AnnualInterestRate = account.AnnualInterestRate,
                MonthlyContribution = account.MonthlyContribution,
                StartDate = account.StartDate
            };
        }

        // POST: api/Account
        [HttpPost]
        public async Task<ActionResult<AccountResponseDto>> PostAccount(CreateAccountDto accountDto)
        {
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return Unauthorized();

            var account = new Account
            {
                UserId = user.Id,
                Name = accountDto.Name,
                TargetAmount = accountDto.TargetAmount,
                CurrentBalance = accountDto.CurrentBalance ?? 0,
                AnnualInterestRate = accountDto.AnnualInterestRate,
                MonthlyContribution = accountDto.MonthlyContribution,
                StartDate = accountDto.StartDate
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var responseDto = new AccountResponseDto
            {
                Id = account.Id,
                UserId = account.UserId,
                Name = account.Name,
                TargetAmount = account.TargetAmount,
                CurrentBalance = account.CurrentBalance,
                AnnualInterestRate = account.AnnualInterestRate,
                MonthlyContribution = account.MonthlyContribution,
                StartDate = account.StartDate
            };

            return CreatedAtAction("GetAccount", new { id = account.Id }, responseDto);
        }

        // PUT: api/Account/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, UpdateAccountDto accountDto)
        {
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return Unauthorized();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);

            if (account == null)
            {
                return NotFound();
            }

            account.Name = accountDto.Name;
            account.TargetAmount = accountDto.TargetAmount;
            account.AnnualInterestRate = accountDto.AnnualInterestRate;
            account.MonthlyContribution = accountDto.MonthlyContribution;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userName = _userManager.GetUserName(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return Unauthorized();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
