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
    public class GroupDataSource
    {
        
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GroupVM> GetGroups()
        {

            return UserHelper.GetGroups();
        }


    }

   
}
