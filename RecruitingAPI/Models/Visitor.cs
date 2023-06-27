using System.ComponentModel.DataAnnotations;

namespace RecruitingAPI.Models
{
    public class Visitor
    {
            [Key]
            public int idVisitor { get; set; }
            public DateTime VisitDate { get; set; }


    }
}
