using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Admin
{
   public class UserGroupNRoleVm
    {
        public UserGroupNRoleVm()
        {
            Groups = new List<UserGroupVm>();
            Roles = new List<UserRoleVm>();
        }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string WorkUnit { get; set; }
        public List<UserGroupVm> Groups { get; set; }
        public List<UserRoleVm> Roles { get; set; }
    }

    public class UserGroupVm
    {
        public string GroupName { get; set; }
    }

    public class UserRoleVm
    {
        public string RoleName { get; set; }
    }
}
