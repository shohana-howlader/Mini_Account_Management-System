using System.ComponentModel.DataAnnotations;

namespace Mini_Account_Management_System.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

       
       
    }
}