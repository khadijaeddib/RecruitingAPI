using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingAPI.Models
{
    public class ResetPassword
    {
        public string email { get; set; }
        public string newPassword { get; set; }

        [NotMapped]
        public string confirmNewPassword { get; set; } = "";
    }
}
