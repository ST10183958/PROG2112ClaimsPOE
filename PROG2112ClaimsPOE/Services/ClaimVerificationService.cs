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

        // Ensure server-calculated payment
        var computed = claim.HoursWorked * claim.HourlyRate;
        if (claim.Payment != computed)
            result.Messages.Add($"FinalPayment mismatch. Computed: {computed}.");

        // Auto-reject invalid numbers
        if (claim.HoursWorked < 0 || claim.HourlyRate < 0 || claim.Payment < 0)
        {
            result.IsRejected = true;
            result.Messages.Add("Negative numeric values are invalid.");
            return Task.FromResult(result);
        }

        // Auto-approve only if *completely clean*
        if (result.Messages.Count == 0)
        {
            result.IsApproved = true;
            result.Messages.Add("Auto-approved: meets all automatic criteria.");
            return Task.FromResult(result);
        }

        // Otherwise requires manual review
        result.NeedsReview = true;
        return Task.FromResult(result);
    }

}
