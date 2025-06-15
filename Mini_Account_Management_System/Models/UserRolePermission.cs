namespace Mini_Account_Management_System.Models
{
    public class UserRolePermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int ScreenId { get; set; }

        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
        public Screen Screen { get; set; }
    }

}