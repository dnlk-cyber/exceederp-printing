using System.Collections.Generic;

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

    public class UsersInGroupAndRoleVm
    {        
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string WorkUnit { get; set; }
    }

    public class GroupVM
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }

    public class RoleVm
    {
        public string GroupId { get; set; }
        public string RoleId { get; set; }
        public string Name { get; set; }

    }
}
