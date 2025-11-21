// Controllers/Api/ClaimsApiController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG2112ClaimsPOE.Data;
using PROG2112ClaimsPOE.Models;
using PROG2112ClaimsPOE.Services;

[Route("api/[controller]")]
[ApiController]   // only managers can call these endpoints
public class ClaimsApiController : ControllerBase
{
    private readonly ClaimDbContext _context;
    private readonly IClaimVerificationService _verifier;

    public ClaimsApiController(ClaimDbContext context, IClaimVerificationService verifier)
    {
        _context = context;
        _verifier = verifier;
    }

    // GET: api/ClaimsApi/Verify/5  (runs verification for a single claim)
    [HttpGet("Verify/{id}")]
    public async Task<IActionResult> Verify(int id)
    {
        var claim = await _context.ClaimTable.FindAsync(id);
        if (claim == null) return NotFound();

        var result = await _verifier.VerifyAsync(claim);

        // Log
        _context.ApprovalLogs.Add(new ApprovalLog
        {
            ClaimId = id,
            Actor = User.Identity?.Name ?? "System",
            Action = "Verified",
            Notes = string.Join("; ", result.Messages)
        });

        // Apply automatic changes if applicable
        if (result.IsApproved)
        {
            claim.Statues = ClaimStatus.AutoApproved;
            _context.ApprovalLogs.Add(new ApprovalLog
            {
                ClaimId = id,
                Actor = "System",
                Action = "AutoApproved",
                Notes = "Auto approved by verification rules"
            });
        }
        else if (result.IsRejected)
        {
            claim.Statues = ClaimStatus.AutoApproved;
            _context.ApprovalLogs.Add(new ApprovalLog
            {
                ClaimId = id,
                Actor = "System",
                Action = "AutoApproved",
                Notes = "Auto approved by verification rules"
            });
        }
        else if (result.NeedsReview)
        {
            claim.Statues = ClaimStatus.NeedsReview;
            _context.ApprovalLogs.Add(new ApprovalLog
            {
                ClaimId = id,
                Actor = "System",
                Action = "AutoApproved",
                Notes = "Auto approved by verification rules"
            });
        }

        await _context.SaveChangesAsync();

        return Ok(result);
    }

    [HttpPost("ManagerApprove/{id}")]
    public async Task<IActionResult> ManagerApprove(int id, [FromBody] string notes)
    {
        var claim = await _context.ClaimTable.FindAsync(id);
        if (claim == null) return NotFound();

        // Update the status to Approved
        claim.Statues = ClaimStatus.Approved;

        // Create a log for this approval action
        _context.ApprovalLogs.Add(new ApprovalLog
        {
            ClaimId = id,
            Actor = User.Identity?.Name ?? "Manager",
            Action = "ManagerApproved",
            Notes = notes,
            Timestamp = DateTime.Now // Make sure to set the current time
        });

        // Explicitly mark the claim entity as modified
        _context.Entry(claim).State = EntityState.Modified;

        // Save changes to the database
        await _context.SaveChangesAsync();

        return Ok();
    }


    [HttpPost("ManagerReject/{id}")]
    public async Task<IActionResult> ManagerReject(int id, [FromBody] string notes)
    {
        var claim = await _context.ClaimTable.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Statues = ClaimStatus.Rejected;
        _context.ApprovalLogs.Add(new ApprovalLog
        {
            ClaimId = id,
            Actor = User.Identity?.Name ?? "Manager",
            Action = "ManagerRejected",
            Notes = notes
        });

        await _context.SaveChangesAsync();
        return Ok();
    }
}
