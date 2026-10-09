using System.Collections.Generic;

namespace WebApplicationtodolist.Models
{
    public class AdminUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
