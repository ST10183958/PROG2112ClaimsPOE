using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG2112ClaimsPOE.Data;
using PROG2112ClaimsPOE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace PROG2112ClaimsPOE.Controllers
{
    public class HRManagementController : Controller
    {
        private readonly ClaimDbContext _context;

        public HRManagementController(ClaimDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> LecturerList()
        {
            var lecturers = await _context.ClaimTable
                .Select(c => new LecturerModel
                {
                    Id = c.Id,
                    LectureName = c.LectureName,
                    LectureSurname = c.LectureSurname,
                    LecturerEmail = c.LecturerEmail,
                    SubjectName = c.SubjectName,
                    CampusLocation = c.CampusLocation
                })
                .Distinct() // Ensure no duplicate lecturers if necessary
                .ToListAsync();

            return View(lecturers);
        }

        // View all claims (approved ones)
        public async Task<IActionResult> ApprovedClaims()
        {
            var approvedClaims = await _context.ClaimTable
                .Where(c => (int)c.Statues == (int)ClaimStatus.AutoApproved) // Cast ClaimStatus to int
                .ToListAsync();
            return View(approvedClaims);
        }


        // Generate a report or invoice for claims
        public IActionResult GenerateInvoice(int claimId)
        {
            var claim = _context.ClaimTable
                .FirstOrDefault(c => c.Id == claimId);

            if (claim == null)
                return NotFound();

            // Generate invoice/report logic here (you can use a library like Crystal Reports or LINQ to create it)
            return View(claim); // Return view or download the report
        }

        // Update lecturer information
        public IActionResult UpdateLecturer(int id)
        {
            var lecturer = _context.ClaimTable
                .Where(c => c.Id == id)
                .FirstOrDefault();

            if (lecturer == null)
                return NotFound();

            var model = new LecturerModel
            {
                Id = lecturer.Id,
                LectureName = lecturer.LectureName,
                LectureSurname = lecturer.LectureSurname,
                LecturerEmail = lecturer.LecturerEmail,
                SubjectName = lecturer.SubjectName,
                CampusLocation = lecturer.CampusLocation
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLecturer(LecturerModel model)
        {
            if (ModelState.IsValid)
            {
                var lecturer = await _context.ClaimTable
                    .FirstOrDefaultAsync(c => c.Id == model.Id);

                if (lecturer != null)
                {
                    lecturer.LectureName = model.LectureName;
                    lecturer.LectureSurname = model.LectureSurname;
                    lecturer.LecturerEmail = model.LecturerEmail;
                    lecturer.SubjectName = model.SubjectName;
                    lecturer.CampusLocation = model.CampusLocation;

                    _context.Update(lecturer);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ApprovedClaims");
                }
            }

            return View(model);
        }
    }
}
