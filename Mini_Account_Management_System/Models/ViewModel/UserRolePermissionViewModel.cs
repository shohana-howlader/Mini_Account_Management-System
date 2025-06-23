using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Mini_Account_Management_System.Models.ViewModel
{
    public class UserRolePermissionViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int ScreenId { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string ScreenName { get; set; }
        public string URL { get; set; }
    }

    public class UserRolePermissionDto
    {
        [Required]
        public int ScreenId { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class CreateUserRolePermissionViewModel
    {
        // Make these nullable integers instead of int
        public int? UserId { get; set; }
        public int? RoleId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one permission must be provided")]
        public List<UserRolePermissionDto> Permissions { get; set; } = new List<UserRolePermissionDto>();
    }

    public class EditUserRolePermissionViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int ScreenId { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string ScreenName { get; set; }
    }
}