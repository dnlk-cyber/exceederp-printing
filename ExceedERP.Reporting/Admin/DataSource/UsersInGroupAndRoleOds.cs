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
    public class UsersInGroupAndRoleOds
    {
        
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<UsersInGroupAndRoleVm> GetUsersInGroupAndRoleOds(string groupid,string roleId)
        {
           return UserHelper.GetUsersInGroupAndRoles(groupid,roleId);
        }
    }
}
