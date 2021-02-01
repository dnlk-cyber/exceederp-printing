﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.HRM.Company;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class ClearanceProviderController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            GetDepartments();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Department"] = db.MainCompanyStructures.ToList();
        }
        public JsonResult GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var allDepo = db.MainCompanyStructures.ToList();
            allDepartment.AddRange(allDepo);
            //var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
            //if (departments != null)
            //{

            //    var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
            //    if (deptlevelOne.Any())
            //    {
            //        foreach (var a in deptlevelOne)
            //        {
            //            var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
            //            if (departmentLevelTwo.Any())
            //            {
            //                foreach (var b in departmentLevelTwo)
            //                {
            //                    allDepartment.Add(b);
            //                }
            //            }
            //            allDepartment.Add(a);
            //        }
            //    }
            //}
            ViewData["Departments"] = allDepartment;
            return Json(allDepartment.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult ClearanceProvideres_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ClearanceProvider> clearanceprovideres = db.ClearanceProvideres;
            DataSourceResult result = clearanceprovideres.ToDataSourceResult(request, clearanceProvider => new {
                ClearanceProviderId = clearanceProvider.ClearanceProviderId,
                WorkUnit = clearanceProvider.WorkUnit,
                Name = clearanceProvider.Name,
                Remark = clearanceProvider.Remark,
                DateCreated = clearanceProvider.DateCreated,
                LastModified = clearanceProvider.LastModified,
                CreatedBy = clearanceProvider.CreatedBy,
                ModifiedBy = clearanceProvider.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProvideres_Create([DataSourceRequest]DataSourceRequest request, ClearanceProvider clearanceProvider)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //not working

                //var salsries = db.ClearanceProvideres.Where(x => x.Name.ToUpper(XAPI.Common.exceedculture) == clearanceProvider.Name.ToUpper(XAPI.Common.exceedculture) || x.WorkUnit.ToUpper(XAPI.Common.exceedculture) == clearanceProvider.WorkUnit.ToUpper(XAPI.Common.exceedculture)).FirstOrDefault();
                //if (salsries != null)
                //{
                //    ModelState.AddModelError("", "Work unit or name must be unique!!");
                //}
                var entity = new ClearanceProvider
                {
                    WorkUnit = clearanceProvider.WorkUnit,
                    Name = clearanceProvider.Name,
                    Remark = clearanceProvider.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = clearanceProvider.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = clearanceProvider.ModifiedBy
                };

                db.ClearanceProvideres.Add(entity);
                db.SaveChanges();
                clearanceProvider.ClearanceProviderId = entity.ClearanceProviderId;

                #region operation log
               
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Clearance Provider ", "Success", User.Identity.Name, User.Identity.Name + " Create Clearance Provider Named " + entity.WorkUnit,Modules.HRM);
                
                
                #endregion
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }
      
         [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProvideres_Update([DataSourceRequest]DataSourceRequest request, ClearanceProvider clearanceProvider)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ClearanceProvider
                {
                    ClearanceProviderId = clearanceProvider.ClearanceProviderId,
                    WorkUnit = clearanceProvider.WorkUnit,
                    Name = clearanceProvider.Name,
                    Remark = clearanceProvider.Remark,
                    DateCreated = clearanceProvider.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = clearanceProvider.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ClearanceProvideres.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Clearance Provider ", "Success", User.Identity.Name, User.Identity.Name + " Update Clearance Provider Info of " + entity.WorkUnit,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProvideres_Destroy([DataSourceRequest]DataSourceRequest request, ClearanceProvider clearanceProvider)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ClearanceProvider
                {
                    ClearanceProviderId = clearanceProvider.ClearanceProviderId,
                    WorkUnit = clearanceProvider.WorkUnit,
                    Name = clearanceProvider.Name,
                    Remark = clearanceProvider.Remark,
                    DateCreated = clearanceProvider.DateCreated,
                    LastModified = clearanceProvider.LastModified,
                    CreatedBy = clearanceProvider.CreatedBy,
                    ModifiedBy = clearanceProvider.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Clearance provider " + clearanceProvider.Name);
                #endregion
                db.ClearanceProvideres.Attach(entity);
                db.ClearanceProvideres.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
