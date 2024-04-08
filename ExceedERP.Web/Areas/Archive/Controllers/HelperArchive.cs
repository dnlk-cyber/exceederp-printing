using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public static class HelperArchive
    {
        public static DepartmentOrSubjectType GetFolderStructureType()
        {
            ExceedDbContext db = new ExceedDbContext();
            string username = HttpContext.Current.User.Identity.Name;
            var depId = UserInformationHelper.GetEmployeeDepartmentId(username);
            var folder = db.DepartmentOrSubjectTypeSelectionForFolderCreations.FirstOrDefault(x => x.Owner_OrgStructureId == depId);
            if (folder != null)
            {
                return folder.DepartmentOrSubjectType;
            }

            return (DepartmentOrSubjectType) (- 1);
        }
    }
}