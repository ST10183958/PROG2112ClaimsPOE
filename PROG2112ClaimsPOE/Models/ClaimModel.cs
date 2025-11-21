using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG2112ClaimsPOE.Models
{

    
    public class ClaimModel
    {

    [Key]
    public int Id { get; set; }
    public string LectureName { get; set; }
    public string LectureSurname { get; set; }
    public string SubjectName { get; set; }
    public string LecturerEmail { get; set; }
    public string SubjectCode { get; set; }
    public string CampusLocation { get; set; }
    public int HoursWorked { get; set; }
    public int HourlyRate { get; set; }

    public int Payment { get; set; }
    public string Message { get; set; }

        public ClaimStatus Statues { get; set; } = ClaimStatus.Pending;


        public string UploadedFileName { get; set; }
    }
}


