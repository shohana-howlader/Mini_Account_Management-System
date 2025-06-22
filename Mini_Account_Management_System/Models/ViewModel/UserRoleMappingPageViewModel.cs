namespace Mini_Account_Management_System.Models.ViewModel
{
    public class UserRoleMappingPageViewModel
    {
        public UserRoleMappingViewModel NewMapping { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
        public List<UserRoleMappingViewModel> Mappings { get; set; } = new();
    }
}
