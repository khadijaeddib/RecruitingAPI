using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingAPI.Models
{
    public class Candidate
    {
            [Key]
            public int idCand { get; set; }

            public string candImagePath { get; set; }

            [NotMapped]
            [Display(Name = "Choisir photo de profile")]
            public IFormFile candImage { get; set; }

            public string lName { get; set; }

            public string fName { get; set; }

            [EmailAddress]
            public string email { get; set; }

            public int age { get; set; }

            public string phone { get; set; }

            public string address { get; set; }

            public string cin { get; set; }

            public string studyDegree { get; set; }

            public string diploma { get; set; }

            public string spec { get; set; }

            public string expYears { get; set; }

            public string lmPath { get; set; }

            [NotMapped]
            [Display(Name = "Choisir LM")]
            public IFormFile lmFile { get; set; }

            public string cvPath { get; set; }

            [NotMapped]
            [Display(Name = "Choisir CV")]
            public IFormFile cvFile { get; set; }

            [MinLength(10, ErrorMessage ="Please enter at leat 10 characters")]
            public string pass { get; set; }

            [NotMapped]
            public string confirmPass { get; set; } = "";

            public string role { get; set; } = "Candidate";

            public string token { get; set; }

    }

}
