using Microsoft.AspNetCore.Mvc;
using PROG2112ClaimsPOE.Models;

namespace PROG2112ClaimsPOE.Controllers
{
    public class ViewClaim : Controller
    {

        private static List<ClaimModel> ClaimsList = new List<ClaimModel>
        {
            new ClaimModel
            {
                Id = "1",
                LectureName = "Flutter",
                LectureSurname = "Shy",
                SubjectName = "Animal Care 101",
                LecturerEmail = "flutter.shy@example.com",
                SubjectCode = "AC101",
                CampusLocation = "Cloudsdale",
                HoursWorked = "12",
                Message = "Completed weekly animal care supervision.",
                Statues = "Pending"
            },
            new ClaimModel
            {
                Id = "2",
                LectureName = "Feather",
                LectureSurname = "Wing",
                SubjectName = "Pegasus Weather Control",
                LecturerEmail = "feather.wing@example.com",
                SubjectCode = "PW203",
                CampusLocation = "Canterlot",
                HoursWorked = "8",
                Message = "Prepared storm simulations.",
                Statues = "Approved"
            }
        };

        public IActionResult Index()
        {
            return View(ClaimsList);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Create(ClaimModel newClaim)
        {
            if (ModelState.IsValid)
            {
                // Simple ID generator
                newClaim.Id = (ClaimsList.Count + 1).ToString();

                ClaimsList.Add(newClaim);

                return RedirectToAction("Index");
            }

            return View(newClaim);
        }
    }
}
