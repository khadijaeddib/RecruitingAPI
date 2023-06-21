using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingAPI.Models
{
    public class Candidature
    {
        [Key]
        public int idCandidature { get; set; }

        public string status { get; set; }

        public DateTime dateCand { get; set; }

        public string motivation { get; set; }

        public int idCand { get; set; }

        [ForeignKey("idCand")]
        public Candidate Candidate { get; set; }

        public int idOffer { get; set; }

        [ForeignKey("idOffer")]
        public Offer Offer { get; set; }

    }
}
