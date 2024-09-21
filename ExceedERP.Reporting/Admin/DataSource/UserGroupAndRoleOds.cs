using ExceedERP.Core.Domain.Admin;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Admin.DataSource
{
   
    [DataObject]
    public class UserGroupAndRoleOds
    {
       
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<UserGroupNRoleVm> GetUserGroupsAndRoles(string userName)
        {
           return UserHelper.GetUserGroupsAndRoles(userName);
        }
    }
}
