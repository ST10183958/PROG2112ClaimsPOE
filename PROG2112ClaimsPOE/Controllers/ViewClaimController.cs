using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PROG2112ClaimsPOE.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PROG2112ClaimsPOE.Controllers
{
    public class ViewClaimController : Controller
    {

        private readonly IWebHostEnvironment _env;

        public ViewClaimController(IWebHostEnvironment env)
        {
            _env = env;
        }


        private static List<ClaimModel> ClaimsList = new List<ClaimModel>();

        public IActionResult Index()
        {
            return View(ClaimsList);
        }

        // Manager section //

        public IActionResult ManagerView()
        {
            var pendingClaims = ClaimsList
                .Where(c => c.Statues == "Pending")
                .ToList();
            return View(pendingClaims);
        }

        public IActionResult Accept(string id)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Statues = "Accepted";
            }

            return RedirectToAction("ManagerView");
        }

        public IActionResult Reject(string id)
        {
            var claim = ClaimsList.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Statues = "Rejected";
            }

            return RedirectToAction("ManagerView");
        }


        // Lecturer Section

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ClaimModel model, IFormFile uploadFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Handle file upload if there *is* a file
            if (uploadFile != null)
            {
                // Size Check: max 5MB
                if (uploadFile.Length > 5 * 1024 * 1024)
                {
                    ViewBag.Error = "File size cannot exceed 5 MB.";
                    return View(model);
                }

                // Allowed types
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var ext = Path.GetExtension(uploadFile.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                {
                    ViewBag.Error = "Only PDF, DOCX, and XLSX files are allowed.";
                    return View(model);
                }

                // Ensure upload directory exists
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Create unique filename
                string uniqueFileName = $"{Guid.NewGuid()}{ext}";
                string filePath = Path.Combine(uploadPath, uniqueFileName);

                // Save file securely
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(fs);
                }

                // Save filename to model
                model.UploadedFileName = uniqueFileName;
            }

            // Add claim to the list
            model.Id = (ClaimsList.Count + 1).ToString();
            model.Statues = "Pending";
            ClaimsList.Add(model);

            return RedirectToAction("Index");
        }
    }
}
