using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class UserListViewModel
    {
        public ApplicationUser User { get; set; } = new();
        public IList<string> Roles { get; set; } = new List<string>();
    }
}