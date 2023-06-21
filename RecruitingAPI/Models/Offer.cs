using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitingAPI.Models
{
    public class Offer
    {
            [Key]
            public int idOffer { get; set; }

            public string title { get; set; }

            public string studyDegree { get; set; }

            public string diploma { get; set; }

            public string businessSector { get; set; }

            public string expYears { get; set; }

            public string contractType { get; set; }

            public string city { get; set; }

            public string availability { get; set; }

            public int hiredNum { get; set; }

            public float salary { get; set; }

            public string description { get; set; }

            public string[] skills { get; set; }

            public string[] missions { get; set; }

            public string[] languages { get; set; }

            public DateTime pubDate { get; set; }

            public DateTime endDate { get; set; }

            public int idRec { get; set; }

            [ForeignKey("idRec")]
            public Recruiter Recruiter { get; set; }

    }

}
