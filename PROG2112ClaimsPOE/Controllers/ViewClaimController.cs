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
            var pendingClaims = await _context.ClaimTable
                .Where(c => c.Statues == "Pending")
                .ToListAsync();

            return View(pendingClaims);
        }

        // ACCEPT
        public async Task<IActionResult> Accept(int id)
        {
            var claim = await _context.ClaimTable.FindAsync(id);
            if (claim != null)
            {
                claim.Statues = "Accepted";
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
                claim.Statues = "Rejected";
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

                string fileName = Guid.NewGuid().ToString() + ext;
                string filePath = Path.Combine(uploadPath, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(fs);
                }

                model.UploadedFileName = fileName;
            }

            model.Statues = "Pending";

            // Save to DB
            _context.ClaimTable.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
