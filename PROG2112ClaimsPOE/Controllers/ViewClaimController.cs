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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
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
        [HttpPost]
        [Route("ViewClaim/TestPost")]
        public IActionResult TestPost()
        {
            Console.WriteLine("POST hit!");
            return Content("POST hit!");
        }

        public IActionResult Create()
        {
            return View();
        }


        // ======================================
        // CREATE — POST
        // ======================================
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimModel model, IFormFile uploadFile)
        {
            try
            {
                Console.WriteLine("CreateTest POST triggered");

                // Calculate payment
                int payment = model.HoursWorked * model.HourlyRate;

                // If no file selected, set a default name
                string uploadedFileName = uploadFile?.FileName ?? "nofile";

                int status = (int)ClaimStatus.Pending;

                string sql = @"
            INSERT INTO ClaimTable 
            (LectureName, LectureSurname, SubjectName, LecturerEmail, SubjectCode, CampusLocation,
             HoursWorked, HourlyRate, Payment, Message, Statues, UploadedFileName)
            VALUES 
            (@LectureName, @LectureSurname, @SubjectName, @LecturerEmail, @SubjectCode, @CampusLocation,
             @HoursWorked, @HourlyRate, @Payment, @Message, @Status, @UploadedFileName)
        ";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new[]
                    {
                new Microsoft.Data.SqlClient.SqlParameter("@LectureName", model.LectureName),
                new Microsoft.Data.SqlClient.SqlParameter("@LectureSurname", model.LectureSurname),
                new Microsoft.Data.SqlClient.SqlParameter("@SubjectName", model.SubjectName),
                new Microsoft.Data.SqlClient.SqlParameter("@LecturerEmail", model.LecturerEmail),
                new Microsoft.Data.SqlClient.SqlParameter("@SubjectCode", model.SubjectCode),
                new Microsoft.Data.SqlClient.SqlParameter("@CampusLocation", model.CampusLocation),
                new Microsoft.Data.SqlClient.SqlParameter("@HoursWorked", model.HoursWorked),
                new Microsoft.Data.SqlClient.SqlParameter("@HourlyRate", model.HourlyRate),
                new Microsoft.Data.SqlClient.SqlParameter("@Payment", payment),
                new Microsoft.Data.SqlClient.SqlParameter("@Message", model.Message ?? ""),
                new Microsoft.Data.SqlClient.SqlParameter("@Status", status),
                new Microsoft.Data.SqlClient.SqlParameter("@UploadedFileName", uploadedFileName)
                    }
                );

                Console.WriteLine("Claim inserted via CreateTest.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting dummy claim: " + ex.Message);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}
