using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RecruitingAPI.Models;

namespace RecruitingAPI.Models
{
    public class Interview
    {
        [Key]
        public int idInterview { get; set; }

        public string status { get; set; }

        public DateTime interviewDate { get; set; }

        public string address { get; set; }

        public string interviewFormat { get; set; }

        public int idCandidature { get; set; }

        [ForeignKey("idCandidature")]
        public Candidature Candidature { get; set; }

    }
}

