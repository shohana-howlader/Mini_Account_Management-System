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

    public class CreateUserRolePermissionViewModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int ScreenId { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
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

    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalScreens { get; set; }
        public int TotalPermissions { get; set; }
    }
}
