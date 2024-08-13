using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Insurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
  
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
    public class InsuranceTypeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
  

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var look = db.Lookups.Where(x => x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS).Select(x=>new  { x.LookupId, x.Description}).ToList();
            ViewData["insuranceTypes"] = look;
        }
        public ActionResult InsuranceType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InsuranceType> insurancetype = db.InsuranceType;
            DataSourceResult result = insurancetype.ToDataSourceResult(request, insuranceType => new
            {
                InsuranceTypeId = insuranceType.InsuranceTypeId,
                Name = insuranceType.Name,
                InsuranceTypeGroup = insuranceType.InsuranceTypeGroup,
                LookupId = insuranceType.LookupId,
                Remark = insuranceType.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceType_Create([DataSourceRequest]DataSourceRequest request, InsuranceType insuranceType)
        {
            ModelState.Clear();
            var insurance = db.InsuranceType.FirstOrDefault(x => x.LookupId == insuranceType.LookupId);
            if (insurance != null)
            {
                ModelState.AddModelError("", "This Insurance Type is registered before");
            }

            if (ModelState.IsValid)
            {
                var name = "";
                    var look = db.Lookups.FirstOrDefault(x=>x.LookupId == insuranceType.LookupId);
                if(look!= null)
                {
                    name = look.Description;
                }
                    var entity = new InsuranceType
                    {
                        Name = name,
                        InsuranceTypeGroup = insuranceType.InsuranceTypeGroup,
                        LookupId = insuranceType.LookupId,
                        Remark = insuranceType.Remark
                    };

                    db.InsuranceType.Add(entity);
                    db.SaveChanges();
                    insuranceType.InsuranceTypeId = entity.InsuranceTypeId;
                //}
            }

            return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceType_Update([DataSourceRequest]DataSourceRequest request, InsuranceType insuranceType)
        {
            if (ModelState.IsValid)
            {
                var name = "";
                var look = db.Lookups.FirstOrDefault(x => x.LookupId == insuranceType.LookupId);
                if (look != null)
                {
                    name = look.Description;
                }

                var entity = new InsuranceType
                {
                    InsuranceTypeId = insuranceType.InsuranceTypeId,
                    Name = name,                  
                    InsuranceTypeGroup = insuranceType.InsuranceTypeGroup,
                    LookupId = insuranceType.LookupId,
                    Remark = insuranceType.Remark
                };

                db.InsuranceType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceType_Destroy([DataSourceRequest]DataSourceRequest request, InsuranceType insuranceType)
        {
            if (ModelState.IsValid)
            {
                var entity = new InsuranceType
                {
                    InsuranceTypeId = insuranceType.InsuranceTypeId,
                    Name = insuranceType.Name,
                    InsuranceTypeGroup = insuranceType.InsuranceTypeGroup,
                    LookupId = insuranceType.LookupId,
                    Remark = insuranceType.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted insrance type  information of " + insuranceType.Name);
                #endregion
                db.InsuranceType.Attach(entity);
                db.InsuranceType.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
