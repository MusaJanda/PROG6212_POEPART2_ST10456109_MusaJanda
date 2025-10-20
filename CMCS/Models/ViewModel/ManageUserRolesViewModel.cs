using CMCS.Models;
namespace CMCS.Models.ViewModel
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }


    }
}
