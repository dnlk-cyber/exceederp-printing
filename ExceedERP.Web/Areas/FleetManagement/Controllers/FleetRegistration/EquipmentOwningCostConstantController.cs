﻿using System;
using System.Collections.Generic; 
 
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
      [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class EquipmentOwningCostConstantController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OwningandOperatingCostConstants_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OwningandOperatingCostConstant> owningandoperatingcostconstants = db.OwningandOperatingCostConstants;
            DataSourceResult result = owningandoperatingcostconstants.ToDataSourceResult(request, owningandOperatingCostConstant => new {
                OwningandOperatingCostConstantId = owningandOperatingCostConstant.OwningandOperatingCostConstantId,
                ConstantType = owningandOperatingCostConstant.ConstantType,
                Value = owningandOperatingCostConstant.Value,
                Remark = owningandOperatingCostConstant.Remark,
                DateCreated = owningandOperatingCostConstant.DateCreated,
                LastModified = owningandOperatingCostConstant.LastModified,
                CreatedBy = owningandOperatingCostConstant.CreatedBy,
                ModifiedBy = owningandOperatingCostConstant.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OwningandOperatingCostConstants_Create([DataSourceRequest]DataSourceRequest request, OwningandOperatingCostConstant owningandOperatingCostConstant)
        {
            if (ModelState.IsValid)
            {
                var OwningConstant = db.OwningandOperatingCostConstants.Where(x => x.ConstantType == owningandOperatingCostConstant.ConstantType).ToList();
                if(OwningConstant.Any())
                {
                    ModelState.AddModelError("Duplicate", "Contstant Type Already Registered");
                    return Json(new[] { owningandOperatingCostConstant }.ToDataSourceResult(request, ModelState));
                }
                var entity = new OwningandOperatingCostConstant
                {
                    ConstantType = owningandOperatingCostConstant.ConstantType,
                    Value = owningandOperatingCostConstant.Value,
                    Remark = owningandOperatingCostConstant.Remark,
                    DateCreated =DateTime.Now,
                      CreatedBy = User.Identity.Name,
                     
                };

                db.OwningandOperatingCostConstants.Add(entity);
                db.SaveChanges();
                owningandOperatingCostConstant.OwningandOperatingCostConstantId = entity.OwningandOperatingCostConstantId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Owning and operating cost constatnt registration", "Success", User.Identity.Name, $"created owning and operating cost constatnt {entity.OwningandOperatingCostConstantId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { owningandOperatingCostConstant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OwningandOperatingCostConstants_Update([DataSourceRequest]DataSourceRequest request, OwningandOperatingCostConstant owningandOperatingCostConstant)
        {
            if (ModelState.IsValid)
            {
                var entity = new OwningandOperatingCostConstant
                {
                    OwningandOperatingCostConstantId = owningandOperatingCostConstant.OwningandOperatingCostConstantId,
                    ConstantType = owningandOperatingCostConstant.ConstantType,
                    Value = owningandOperatingCostConstant.Value,
                    Remark = owningandOperatingCostConstant.Remark,
                    DateCreated = owningandOperatingCostConstant.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = owningandOperatingCostConstant.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.OwningandOperatingCostConstants.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Owning and operating cost constants registration", "Success", User.Identity.Name, $"updated owning and operating cost constants {entity.OwningandOperatingCostConstantId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { owningandOperatingCostConstant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OwningandOperatingCostConstants_Destroy([DataSourceRequest]DataSourceRequest request, OwningandOperatingCostConstant owningandOperatingCostConstant)
        {
            if (ModelState.IsValid)
            {
                var entity = new OwningandOperatingCostConstant
                {
                    OwningandOperatingCostConstantId = owningandOperatingCostConstant.OwningandOperatingCostConstantId,
                    ConstantType = owningandOperatingCostConstant.ConstantType,
                    Value = owningandOperatingCostConstant.Value,
                    Remark = owningandOperatingCostConstant.Remark,
                    DateCreated = owningandOperatingCostConstant.DateCreated,
                    LastModified = owningandOperatingCostConstant.LastModified,
                    CreatedBy = owningandOperatingCostConstant.CreatedBy,
                    ModifiedBy = owningandOperatingCostConstant.ModifiedBy
                };

                db.OwningandOperatingCostConstants.Attach(entity);
                db.OwningandOperatingCostConstants.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted owning and operating cost constants {entity.OwningandOperatingCostConstantId}");
                #endregion
            }

            return Json(new[] { owningandOperatingCostConstant }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
