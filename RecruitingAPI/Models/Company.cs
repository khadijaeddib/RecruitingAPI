using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingAPI.Models
{
    public class Company
    {
        [Key]
        public int idCo { get; set; }

        public string logoPath { get; set; } 

        [NotMapped]
        [Display(Name = "Choisir logo")]
        public IFormFile logoImage { get; set; }

        public string name { get; set; } 

        public string website { get; set; } 

        public string businessSector { get; set; } 

        public string description { get; set; }

        [Phone]
        public string phone { get; set; }

        [EmailAddress]
        public string email { get; set; } 

        public string address { get; set; } 
    }
}
