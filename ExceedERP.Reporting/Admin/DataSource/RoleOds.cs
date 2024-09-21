using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Admin;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Admin
{
   
    [DataObject]
    public class RoleDataSource
    {
       
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RoleVm> GetRoles()
        {

            return UserHelper.GetRoles();
        }


    }

    
}
