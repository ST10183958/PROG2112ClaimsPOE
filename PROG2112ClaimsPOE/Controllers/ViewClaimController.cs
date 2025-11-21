using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG2112ClaimsPOE.Data;
using PROG2112ClaimsPOE.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PROG2112ClaimsPOE.Controllers
{
    public class ViewClaimController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ClaimDbContext _context;

        public ViewClaimController(IWebHostEnvironment env, ClaimDbContext context)
        {
            _env = env;
            _context = context;
        }

        // ======================================
        // INDEX — View All Claims
        // ====================================== 
        public async Task<IActionResult> Index()
        {
            var claims = await _context.ClaimTable.ToListAsync();
            return View(claims);
        }

        // ======================================
        // MANAGER VIEW — Only Pending Claims
        // ======================================
        public async Task<IActionResult> ManagerView()
        {
            var pending = await _context.ClaimTable
                .Where(c => c.Statues == ClaimStatus.Pending || c.Statues == ClaimStatus.NeedsReview)
                .ToListAsync();

            return View(pending);
        }

        // ACCEPT
        public async Task<IActionResult> Accept(int id)
        {
            var claim = await _context.ClaimTable.FindAsync(id);
            if (claim != null)
            {
                claim.Statues = ClaimStatus.Approved; ;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManagerView");
        }

        // REJECT
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.ClaimTable.FindAsync(id);
            if (claim != null)
            {
                claim.Statues = ClaimStatus.Rejected;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManagerView");
        }

        // ======================================
        // CREATE — GET
        // ======================================
        public IActionResult Create()
        {
            return View();
        }

        // ======================================
        // CREATE — POST
        // ======================================
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ClaimModel model, IFormFile uploadFile)
        {
            // Server-side validation
            if (model.HoursWorked <= 0)
                ModelState.AddModelError("HoursWorked", "Hours worked must be greater than 0.");

            if (model.HourlyRate <= 0)
                ModelState.AddModelError("HourlyRate", "Hourly rate must be greater than 0.");

            // Re-check model validity
            if (!ModelState.IsValid)
                return View(model);

            // Handle file upload
            if (uploadFile != null)
            {
                var ext = Path.GetExtension(uploadFile.FileName).ToLower();
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };

                if (!allowed.Contains(ext))
                {
                    ViewBag.Error = "Only PDF, DOCX, XLSX allowed.";
                    return View(model);
                }

                if (uploadFile.Length > 5 * 1024 * 1024)
                {
                    ViewBag.Error = "File too large. Max 5MB.";
                    return View(model);
                }

                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string fileName = Guid.NewGuid() + ext;
                string filePath = Path.Combine(uploadPath, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(fs);
                }

                model.UploadedFileName = fileName;
            }

            // ***** Auto Calculation Logic *****
            model.Payment = model.HoursWorked * model.HourlyRate;
            model.Statues = ClaimStatus.Pending;

            // Save to DB
            _context.ClaimTable.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
