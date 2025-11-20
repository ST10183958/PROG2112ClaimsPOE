// Services/ClaimVerificationService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PROG2112ClaimsPOE.Models;
using PROG2112ClaimsPOE.Services;

public class ClaimVerificationService : IClaimVerificationService
{
    // Example policy thresholds - tune to your needs or load from config
    private const decimal MaxHoursBeforeReview = 40m;
    private const decimal MaxHourlyRateBeforeReview = 500m;
    private const decimal MaxFinalPaymentAutoApprove = 10000m; // below -> ok

    public Task<VerificationResult> VerifyAsync(ClaimModel claim)
    {
        var result = new VerificationResult();

        // Basic sanity checks
        if (claim.HoursWorked <= 0)
            result.Messages.Add("Hours worked must be greater than zero.");

        if (claim.HourlyRate <= 0)
            result.Messages.Add("Hourly rate must be greater than zero.");

        // Compute final payment server-side (ensures integrity)
        var computed = claim.HoursWorked * claim.HourlyRate;
        if (claim.Payment != computed)
            result.Messages.Add($"FinalPayment mismatch. Computed: {computed:C}.");

        // Policy checks
        if (claim.HoursWorked > MaxHoursBeforeReview)
            result.Messages.Add($"Hours worked ({claim.HoursWorked}) exceeds policy threshold ({MaxHoursBeforeReview}).");

        if (claim.HourlyRate > MaxHourlyRateBeforeReview)
            result.Messages.Add($"Hourly rate ({claim.HourlyRate}) exceeds policy threshold ({MaxHourlyRateBeforeReview}).");

        // Decision logic
        // Auto-reject example: negative values or absurd final payment
        if (claim.HoursWorked < 0 || claim.HourlyRate < 0 || claim.Payment < 0)
        {
            result.IsRejected = true;
            result.Messages.Add("Negative numeric values are invalid.");
            return Task.FromResult(result);
        }

        // If nothing flagged and final payment reasonable -> auto-approve
        if (result.Messages.Count == 0 && claim.Payment <= MaxFinalPaymentAutoApprove)
        {
            result.IsApproved = true;
            result.Messages.Add("Auto-approved: meets all automatic criteria.");
            return Task.FromResult(result);
        }

        // If some policy flags exist -> Needs review
        result.NeedsReview = true;
        if (result.Messages.Count == 0)
            result.Messages.Add("Requires manual review.");

        return Task.FromResult(result);
    }
}
