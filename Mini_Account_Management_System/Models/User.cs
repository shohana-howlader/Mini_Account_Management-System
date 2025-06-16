using System.ComponentModel.DataAnnotations;

namespace Mini_Account_Management_System.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        
    }

   
}
