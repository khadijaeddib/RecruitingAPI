using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingAPI.Models
{
    public class Candidate
    {
            [Key]
            public int idCand { get; set; }

            public string imageCandPath { get; set; }

            [NotMapped]
            [Display(Name = "Choisir photo de profile")]
            public IFormFile imageCand { get; set; }

            public string lName { get; set; }

            public string fName { get; set; }

            public string email { get; set; }

            public int age { get; set; }

            public string phone { get; set; }

            public string address { get; set; }

            public string studyDegree { get; set; }

            public string diploma { get; set; }

            public string spec { get; set; }

            public string expYears { get; set; }

            public string LMPath { get; set; }

            [NotMapped]
            [Display(Name = "Choisir LM")]
            public IFormFile LMFile { get; set; }

            public string CVPath { get; set; }

            [NotMapped]
            [Display(Name = "Choisir CV")]
            public IFormFile CVFile { get; set; }

            public string pass { get; set; }

            public string confirmPass { get; set; } = "";

            public string role { get; set; } = "Candidate";
        
    }

}
