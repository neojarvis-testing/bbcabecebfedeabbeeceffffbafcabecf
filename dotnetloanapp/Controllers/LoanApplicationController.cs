using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStoreDBFirst.Models;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using BookStoreDBFirst.Services;



namespace BookStoreDBFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanApplicationController : ControllerBase
    {
        private readonly LoanApplicationDbContext _context;
        private readonly IAzureStorage _azureStorage;
        private readonly ILogger<LoanApplicationController> _logger;
        private readonly IConfiguration _configuration;


        public LoanApplicationController(LoanApplicationDbContext context, ILogger<LoanApplicationController> logger, IConfiguration configuration, IAzureStorage azureStorage)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _azureStorage = azureStorage;


            // Retrieve the Key Vault URI from configuration
          //  string keyVaultUriString = "https://newkeyvlt24.vault.azure.net/";

            // Check if the URI string is not null before creating the Uri

            // var secretclient = new SecretClient(new Uri(keyVaultUriString), new DefaultAzureCredential());
            // var sqlconnstring = secretclient.GetSecret("secret24");
        }

        [HttpGet("GetAllLoanApplications")]
        public async Task<ActionResult<IEnumerable<LoanApplication>>> GetAllLoanApplications()
        {
            var loanApplications = await _context.LoanApplications.ToListAsync();
            return Ok(loanApplications);
        }

        [HttpPost("AddLoanApplication")]
        public async Task<ActionResult> AddLoanApplication(LoanApplication loanApplication)
        {
            Console.WriteLine("came inside");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return detailed validation errors
            }
            await _context.LoanApplications.AddAsync(loanApplication);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("GetLoanApplicationsByUserId")]
public async Task<ActionResult<IEnumerable<LoanApplication>>> GetLoanApplicationsByUserId(string userId)
{
    if (string.IsNullOrEmpty(userId))
    {
        return BadRequest("UserId is required");
    }

    var loanApplications = await _context.LoanApplications
        .Where(l => l.userId == userId)
        .ToListAsync();

    if (loanApplications == null || !loanApplications.Any())
    {
        return NotFound("No loan applications found for the provided userId");
    }

    return Ok(loanApplications);
}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoanApplication(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid LoanApplication id");

            var loanApplication = await _context.LoanApplications.FindAsync(id);
            _context.LoanApplications.Remove(loanApplication);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{LoanApplicationID}")]
        public async Task<IActionResult> UpdateLoanApplication(int LoanApplicationID, LoanApplication updatedLoanApplication)
        {
            if (LoanApplicationID <= 0)
            {
                return BadRequest("Not a valid LoanApplication id");
            }

            var existingLoanApplication = await _context.LoanApplications.FindAsync(LoanApplicationID);

            if (existingLoanApplication == null)
            {
                return NotFound("LoanApplication not found");
            }

            // Update the existingLoanApplication with the values from updatedLoanApplication
            existingLoanApplication.userName = updatedLoanApplication.userName;
            existingLoanApplication.RequestedAmount = updatedLoanApplication.RequestedAmount;
            existingLoanApplication.SubmissionDate = updatedLoanApplication.SubmissionDate;
            existingLoanApplication.EmploymentStatus = updatedLoanApplication.EmploymentStatus;
            existingLoanApplication.Income = updatedLoanApplication.Income;
            existingLoanApplication.CreditScore = updatedLoanApplication.CreditScore;
 existingLoanApplication.LoanStatus = updatedLoanApplication.LoanStatus;

 existingLoanApplication.LoanType = updatedLoanApplication.LoanType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanApplicationExists(LoanApplicationID))
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

        [HttpGet("list")]
        public async Task<IActionResult> ListBlobs()
        {
            try
            {
                var blobs = await _azureStorage.ListAsync();
                return Ok(blobs);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadBlob([FromForm] List<IFormFile> blobs)
        {
            try
            {
                foreach (var blob in blobs)
                {
                    Console.WriteLine($"File name: {blob.FileName}, Content type: {blob.ContentType}, Size: {blob.Length} bytes");
                }
                var response = await _azureStorage.UploadAsync(blobs);

                if (response.Error)
                {
                    return BadRequest(response.Status);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool LoanApplicationExists(int LoanApplicationID)
        {
            return _context.LoanApplications.Any(e => e.LoanApplicationID == LoanApplicationID);
        }

    }
}
