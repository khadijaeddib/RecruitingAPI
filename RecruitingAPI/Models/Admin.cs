using System.ComponentModel.DataAnnotations;

namespace RecruitingAPI.Models
{
    public class Admin
    {
        [Key]
        public int idAdmin { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        public string pass { get; set; }

        public string role { get; set; } = "Admin";

        public string token { get; set; }

    }
}
