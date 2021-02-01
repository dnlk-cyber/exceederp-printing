﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Finance.GL;
﻿using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
 using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Insurance
{
 
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
    public class FixedAssetAccidentRecordController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            GetPeriod();
            return View();
        }
        private void PopulateViewData()
        {
            var assetList = db.Fleets;
            ViewData["AssetList"] = assetList.Select(p => new { FleetsID = p.FleetsID, Name = p.EquipmentName });
            var branches = db.Branch.Where(x => x.BranchTypes == BranchTypes.PROJECT);
            ViewData["branches"] = branches;
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public string GetPlateNumber(Guid assetId)
        {
            var asset = db.Fleets.Where(x => x.FleetsID == assetId).FirstOrDefault();
            if (asset != null)
            {
                return asset.PlateNumber;
            }
            return "";
        }
  
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
        public ActionResult FixedAssetAccidentRecords_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetAccidentRecord> fixedassetaccidentrecords = db.FixedAssetAccidentRecords;
            DataSourceResult result = fixedassetaccidentrecords.ToDataSourceResult(request, fixedAssetAccidentRecord => new
            {
                FixedAssetAccidentRecordId = fixedAssetAccidentRecord.FixedAssetAccidentRecordId,
                PlateNumber = fixedAssetAccidentRecord.PlateNumber,
                FleetsID = fixedAssetAccidentRecord.FleetsID,
                BranchId = fixedAssetAccidentRecord.BranchId,
                GlFiscalYearId = fixedAssetAccidentRecord.GlFiscalYearId,
                Amount = fixedAssetAccidentRecord.Amount,
                Remark = fixedAssetAccidentRecord.Remark,
                OrgBranchName = fixedAssetAccidentRecord.OrgBranchName,
                DateCreated = fixedAssetAccidentRecord.DateCreated,
                LastModified = fixedAssetAccidentRecord.LastModified,
                CreatedBy = fixedAssetAccidentRecord.CreatedBy,
                ModifiedBy = fixedAssetAccidentRecord.ModifiedBy
            });

            return Json(result);
        }
       
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetAccidentRecords_Create([DataSourceRequest]DataSourceRequest request, FixedAssetAccidentRecord fixedAssetAccidentRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetAccidentRecord
                {
                    FleetsID = fixedAssetAccidentRecord.FleetsID,
                    BranchId = fixedAssetAccidentRecord.BranchId,
                    GlFiscalYearId = fixedAssetAccidentRecord.GlFiscalYearId,
                    PlateNumber = GetPlateNumber(fixedAssetAccidentRecord.FleetsID),
                    Amount = fixedAssetAccidentRecord.Amount,
                    Remark = fixedAssetAccidentRecord.Remark,
                    OrgBranchName = fixedAssetAccidentRecord.OrgBranchName,
                    DateCreated = fixedAssetAccidentRecord.DateCreated,
                    LastModified = fixedAssetAccidentRecord.LastModified,
                    CreatedBy = fixedAssetAccidentRecord.CreatedBy,
                    ModifiedBy = fixedAssetAccidentRecord.ModifiedBy
                };

                db.FixedAssetAccidentRecords.Add(entity);
                db.SaveChanges();
                fixedAssetAccidentRecord.FixedAssetAccidentRecordId = entity.FixedAssetAccidentRecordId;
            }

            return Json(new[] { fixedAssetAccidentRecord }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRInsuranceUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetAccidentRecords_Update([DataSourceRequest]DataSourceRequest request, FixedAssetAccidentRecord fixedAssetAccidentRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetAccidentRecord
                {
                    FixedAssetAccidentRecordId = fixedAssetAccidentRecord.FixedAssetAccidentRecordId,
                    PlateNumber = GetPlateNumber(fixedAssetAccidentRecord.FleetsID),
                    FleetsID = fixedAssetAccidentRecord.FleetsID,
                    BranchId = fixedAssetAccidentRecord.BranchId,
                    GlFiscalYearId = fixedAssetAccidentRecord.GlFiscalYearId,
                    Amount = fixedAssetAccidentRecord.Amount,
                    Remark = fixedAssetAccidentRecord.Remark,
                    OrgBranchName = fixedAssetAccidentRecord.OrgBranchName,
                    DateCreated = fixedAssetAccidentRecord.DateCreated,
                    LastModified = fixedAssetAccidentRecord.LastModified,
                    CreatedBy = fixedAssetAccidentRecord.CreatedBy,
                    ModifiedBy = fixedAssetAccidentRecord.ModifiedBy
                };

                db.FixedAssetAccidentRecords.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { fixedAssetAccidentRecord }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetAccidentRecords_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetAccidentRecord fixedAssetAccidentRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetAccidentRecord
                {
                    FixedAssetAccidentRecordId = fixedAssetAccidentRecord.FixedAssetAccidentRecordId,
                    PlateNumber = fixedAssetAccidentRecord.PlateNumber,
                    BranchId = fixedAssetAccidentRecord.BranchId,
                    Amount = fixedAssetAccidentRecord.Amount,
                    FleetsID = fixedAssetAccidentRecord.FleetsID,
                    GlFiscalYearId = fixedAssetAccidentRecord.GlFiscalYearId,
                    Remark = fixedAssetAccidentRecord.Remark,
                    OrgBranchName = fixedAssetAccidentRecord.OrgBranchName,
                    DateCreated = fixedAssetAccidentRecord.DateCreated,
                    LastModified = fixedAssetAccidentRecord.LastModified,
                    CreatedBy = fixedAssetAccidentRecord.CreatedBy,
                    ModifiedBy = fixedAssetAccidentRecord.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Fixed asset accident result " + fixedAssetAccidentRecord.PlateNumber);
                #endregion
                db.FixedAssetAccidentRecords.Attach(entity);
                db.FixedAssetAccidentRecords.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { fixedAssetAccidentRecord }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
