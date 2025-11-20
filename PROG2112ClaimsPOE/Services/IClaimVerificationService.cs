using System.Threading.Tasks;
using System.Threading.Tasks;
using PROG2112ClaimsPOE.Models;

namespace PROG2112ClaimsPOE.Services
{
    public interface IClaimVerificationService
    {
        Task<VerificationResult> VerifyAsync(ClaimModel claim);
    }
}
