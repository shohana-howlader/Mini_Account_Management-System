namespace Mini_Account_Management_System.Models.ViewModel
{
    public class UserRoleMappingViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        // Optional for Edit form prepopulation
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
