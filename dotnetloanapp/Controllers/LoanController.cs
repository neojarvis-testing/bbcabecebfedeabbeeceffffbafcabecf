using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStoreDBFirst.Models;

namespace BookStoreDBFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly LoanApplicationDbContext _context;

        public LoanController(LoanApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetAllLoans()
        {
            var loans = await _context.Loans.ToListAsync();
            return Ok(loans);
        }
        [HttpGet("{id}")]
public async Task<ActionResult<Loan>> GetLoanById(int id)
{
    if (id <= 0)
    {
        return BadRequest("Not a valid Loan id");
    }

 

    var loan = await _context.Loans.FindAsync(id);

 

    if (loan == null)
    {
        return NotFound("Loan not found");
    }

 

    return Ok(loan);
}
// [HttpGet("LoanTypes")]
// public async Task<ActionResult<IEnumerable<string>>> Get()
// {
//     // Project the LoanTitle property using Select
//     var loanTypes = await _context.Loans
//         .OrderBy(x => x.LoanType)
//         .Select(x => x.LoanType)
//         .ToListAsync();

//     return loanTypes;
//}
        [HttpPost]
        public async Task<ActionResult> AddLoan(Loan Loan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return detailed validation errors
            }
            await _context.Loans.AddAsync(Loan);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid Loan id");

            var Loan = await _context.Loans.FindAsync(id);
              _context.Loans.Remove(Loan);
                await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}")]
public async Task<IActionResult> UpdateLoan(int id, Loan updatedLoan)
{
    if (id <= 0)
    {
        return BadRequest("Not a valid Loan id");
    }

    var existingLoan = await _context.Loans.FindAsync(id);

    if (existingLoan == null)
    {
        return NotFound("Loan not found");
    }

    // Update the existingLoan with the values from updatedLoan
    existingLoan.LoanType = updatedLoan.LoanType;
    existingLoan.Description = updatedLoan.Description;
    existingLoan.InterestRate = updatedLoan.InterestRate;
    existingLoan.MaximumAmount = updatedLoan.MaximumAmount;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!LoanExists(id))
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

private bool LoanExists(int id)
{
    return _context.Loans.Any(e => e.LoanID == id);
}

    }
}
