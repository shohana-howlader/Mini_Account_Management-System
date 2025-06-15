using System.ComponentModel.DataAnnotations;

namespace Mini_Account_Management_System.Models
{
    public class Screen
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ScreenName { get; set; }

        [StringLength(255)]
        public string URL { get; set; }

        public ICollection<UserRolePermission> UserRolePermissions { get; set; } = new List<UserRolePermission>();
    }

}