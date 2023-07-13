using System.ComponentModel.DataAnnotations;

namespace RecruitingAPI.Models
{
    public class Contact
    {
        [Key]
        public int idContact { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
    }
}
