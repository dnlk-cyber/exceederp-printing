﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
 using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [Authorize]
    public class ClearanceProviderDetailsController : BaseController
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
            ViewData["clearanceType"] = db.Lookups.Where(x=>x.LookupType ==LookUpTypes.CLEARANCE_TYPE).ToList();
            ViewData["JobTitle"] = db.Lookups.Where(x=>x.LookupType ==LookUpTypes.JOB_TITLE).ToList();
        }
        public JsonResult GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var allDepo = db.MainCompanyStructures.ToList();
            allDepartment.AddRange(allDepo);
            
            ViewData["Departments"] = allDepartment;
            return Json(db.MainCompanyStructures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClearanceProviderDetails_Read([DataSourceRequest]DataSourceRequest request, int cPId)
        {
            IQueryable<ClearanceProviderDetail> clearanceprovideres = db.ClearanceProviderDetails.Where(x=>x.ClearanceProviderId==cPId);
            DataSourceResult result = clearanceprovideres.ToDataSourceResult(request, clearanceProvider => new {
                ClearanceProviderDetailId = clearanceProvider.ClearanceProviderDetailId,
                ClearanceProviderId = clearanceProvider.ClearanceProviderId,
                OrgStructureId = clearanceProvider.OrgStructureId,
                LookupId = clearanceProvider.LookupId,
                HeadId = clearanceProvider.HeadId,
                DateCreated = clearanceProvider.DateCreated,
                CreatedBy = clearanceProvider.CreatedBy,
                ModifiedBy = clearanceProvider.ModifiedBy,
                LastModified = clearanceProvider.LastModified
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProviderDetails_Create([DataSourceRequest]DataSourceRequest request, ClearanceProviderDetail clearanceProvider,int cPId)
        {
            ModelState.Clear();
            var exists = db.ClearanceProviderDetails.Where(x =>x.OrgStructureId ==clearanceProvider.OrgStructureId
                                                               &&x.ClearanceProviderId==clearanceProvider.ClearanceProviderId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed!!");
            }
            clearanceProvider.ClearanceProviderId = cPId;
            //if(db.ClearanceProvideres.Any(x=>x.OrgStructureId == clearanceProvider.OrgStructureId && x.ClearanceProviderId == cPId))
            //{
            //    ModelState.AddModelError("exists", "work Unit of the !!");

            //}
            if (ModelState.IsValid)
            {
                clearanceProvider.DateCreated = DateTime.Now;
                clearanceProvider.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceProvider);
                db.ClearanceProviderDetails.Add(entity);
                db.SaveChanges();
                clearanceProvider.ClearanceProviderId = entity.ClearanceProviderId;
                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance Provider ", "Success", User.Identity.Name, User.Identity.Name + " Create Clearance Provider Named " + entity.ClearanceProviderId,Modules.HRM);                
                #endregion
            }
            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProviderDetails_Update([DataSourceRequest]DataSourceRequest request, ClearanceProviderDetail clearanceProvider)
        {
            ModelState.Clear();
            var exists = db.ClearanceProviderDetails.Where(x => x.OrgStructureId == clearanceProvider.OrgStructureId  && x.ClearanceProviderId != clearanceProvider.ClearanceProviderId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "The same Record exists!!");
            }
            if (ModelState.IsValid)
            {
                clearanceProvider.LastModified = DateTime.Now;
                clearanceProvider.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceProvider);
                db.ClearanceProviderDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance Provider ", "Success", User.Identity.Name, User.Identity.Name + " Update Clearance Provider Info of " + entity.ClearanceProviderId,Modules.HRM);                
                #endregion
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProviderDetails_Destroy([DataSourceRequest]DataSourceRequest request, ClearanceProviderDetail clearanceProvider)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(clearanceProvider);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Clearance provider " + clearanceProvider.ClearanceProviderId);
                #endregion
                db.ClearanceProviderDetails.Attach(entity);
                db.ClearanceProviderDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }

        private ClearanceProviderDetail GetMappedObject(ClearanceProviderDetail clearanceProvider)
        {
            var entity = new ClearanceProviderDetail
            {
                ClearanceProviderDetailId = clearanceProvider.ClearanceProviderDetailId,
                ClearanceProviderId = clearanceProvider.ClearanceProviderId,
                OrgStructureId = clearanceProvider.OrgStructureId,
                HeadId = clearanceProvider.HeadId,
                LookupId = clearanceProvider.LookupId,
                DateCreated = clearanceProvider.DateCreated,
                CreatedBy = clearanceProvider.CreatedBy,
                ModifiedBy = clearanceProvider.ModifiedBy,
                LastModified = clearanceProvider.LastModified
            };
            return entity;
        }
        public ActionResult ClearanceApproval(int id)
        {
            var clearance = db.ClearanceProvideres.Find(id);
            if (clearance != null)
            {
                if (clearance.IsSendForApproval 
                    && clearance.IsDepartmentOnlineApproved 
                    && !clearance.IsOnlineApproved)
                {
                    clearance.IsOnlineApproved = true;
                    clearance.OnlineApprovedTime = DateTime.Now;
                    clearance.OnlineApprovedBy = User.Identity.Name;
                    db.Entry(clearance).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "You successfully approved this Clearance.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                var formattedData1 = new
                {
                    Message = "This Clearance is already approved Once. you can't approve again.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);

            }

            var formattedData2 = new
            {
                Message = "Record is not Found.",
                Status = "FAIL"
            };
            return Json(formattedData2, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
