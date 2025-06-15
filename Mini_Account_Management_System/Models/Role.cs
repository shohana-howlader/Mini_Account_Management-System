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

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<UserRolePermission> UserRolePermissions { get; set; } = new List<UserRolePermission>();
    }
}