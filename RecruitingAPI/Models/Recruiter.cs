using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingAPI.Models
{
    public class Recruiter
    {
        [Key]
        public int idRec { get; set; }

        public string recImagePath { get; set; }

        [NotMapped]
        [Display(Name = "Choisir image")]
        public IFormFile recImage { get; set; }

        public string lName { get; set; }

        public string fName { get; set; }

        public string email { get; set; }

        public string phone { get; set; }

        public int age { get; set; }

        public string address { get; set; }

        public string career { get; set; }

        public string pass { get; set; }

        public string role { get; set; } = "Recruiter";

        public int idCo { get; set; }

        [ForeignKey("idCo")]
        public Company Company { get; set; }

        [MaxLength(1000)]
        public string token { get; set; }

    }
}
