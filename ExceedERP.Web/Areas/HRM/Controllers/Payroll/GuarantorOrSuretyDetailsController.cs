using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
    public class GuarantorOrSuretyDetailsController : BaseController
    {

       
        private ExceedDbContext db = new ExceedDbContext();
      
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult GuarantorOrSuretyDetails_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
           
            IQueryable<GuarantorOrSuretyDetail> guarantorOrSuretyDetails = db.GuarantorOrSuretyDetails.Where(x=>x.GuarantorOrSuretyId==id);

            DataSourceResult result = guarantorOrSuretyDetails.ToDataSourceResult(request, guarantorOrSuretyDetail => new
            {
                GuarantorOrSuretyDetailId = guarantorOrSuretyDetail.GuarantorOrSuretyDetailId,
                GuarantorOrSuretyId = guarantorOrSuretyDetail.GuarantorOrSuretyId,
                FullName = guarantorOrSuretyDetail.FullName,
                Organiztion = guarantorOrSuretyDetail.Organiztion,
                FromDate = guarantorOrSuretyDetail.FromDate,
                FromDateAm = guarantorOrSuretyDetail.FromDateAm,
                ToDate = guarantorOrSuretyDetail.ToDate,
                ToDateAm = guarantorOrSuretyDetail.ToDateAm,
                IsActive = guarantorOrSuretyDetail.IsActive,
                OrgBranchName = guarantorOrSuretyDetail.OrgBranchName,
                DateCreated = guarantorOrSuretyDetail.DateCreated,
                LastModified = guarantorOrSuretyDetail.LastModified,
                CreatedBy = guarantorOrSuretyDetail.CreatedBy,
                ModifiedBy = guarantorOrSuretyDetail.ModifiedBy
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuarantorOrSuretyDetails_Create([DataSourceRequest]DataSourceRequest request, GuarantorOrSuretyDetail guarantorOrSuretyDetail,int id)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(guarantorOrSuretyDetail.FromDateAm))
                {
                    ModelState.AddModelError("", "From date field is mandatory");
                    return Json(new[] { guarantorOrSuretyDetail }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    guarantorOrSuretyDetail.FromDate = HrHelper.ParseEthiopicDate(guarantorOrSuretyDetail.FromDateAm);

                    if (!string.IsNullOrWhiteSpace(guarantorOrSuretyDetail.ToDateAm))
                        guarantorOrSuretyDetail.ToDate = HrHelper.ParseEthiopicDate(guarantorOrSuretyDetail.ToDateAm);
                }

            }
            else
            {

                guarantorOrSuretyDetail.FromDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(guarantorOrSuretyDetail.FromDate.Day, guarantorOrSuretyDetail.FromDate.Month, guarantorOrSuretyDetail.FromDate.Year);
                if(guarantorOrSuretyDetail.ToDate.HasValue)
                    guarantorOrSuretyDetail.ToDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(guarantorOrSuretyDetail.ToDate.Value.Day, guarantorOrSuretyDetail.ToDate.Value.Month, guarantorOrSuretyDetail.ToDate.Value.Year);

            }
            guarantorOrSuretyDetail.GuarantorOrSuretyId = id;
            if (ModelState.IsValid)
            {

                guarantorOrSuretyDetail.DateCreated = DateTime.Now;
                guarantorOrSuretyDetail.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(guarantorOrSuretyDetail);
                db.GuarantorOrSuretyDetails.Add(entity);
                db.SaveChanges();
                guarantorOrSuretyDetail.GuarantorOrSuretyDetailId = entity.GuarantorOrSuretyDetailId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "guarantorOrSuretyDetail", "Success", User.Identity.Name, User.Identity.Name + " add new guarantorOrSuretyDetail at date" + DateTime.Today, Modules.HRM);

                #endregion
               

            }
            return Json(new[] { guarantorOrSuretyDetail }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuarantorOrSuretyDetails_Update([DataSourceRequest] DataSourceRequest request, GuarantorOrSuretyDetail guarantorOrSuretyDetail)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(guarantorOrSuretyDetail.FromDateAm))
                {
                    ModelState.AddModelError("", "From date field is mandatory");
                    return Json(new[] { guarantorOrSuretyDetail }.ToDataSourceResult(request, ModelState));
                }
              
                guarantorOrSuretyDetail.FromDate = HrHelper.ParseEthiopicDate(guarantorOrSuretyDetail.FromDateAm);
                if (!string.IsNullOrWhiteSpace(guarantorOrSuretyDetail.ToDateAm))
                    guarantorOrSuretyDetail.ToDate = HrHelper.ParseEthiopicDate(guarantorOrSuretyDetail.ToDateAm);
                

            }
            else
            {

                guarantorOrSuretyDetail.FromDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(guarantorOrSuretyDetail.FromDate.Day, guarantorOrSuretyDetail.FromDate.Month, guarantorOrSuretyDetail.FromDate.Year);

                if (guarantorOrSuretyDetail.ToDate.HasValue)
                    guarantorOrSuretyDetail.ToDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(guarantorOrSuretyDetail.ToDate.Value.Day, guarantorOrSuretyDetail.ToDate.Value.Month, guarantorOrSuretyDetail.ToDate.Value.Year);

            }
            if (ModelState.IsValid)
            {
                guarantorOrSuretyDetail.LastModified = DateTime.Now;
                guarantorOrSuretyDetail.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(guarantorOrSuretyDetail);
                db.GuarantorOrSuretyDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "guarantorOrSuretyDetail", "Success", User.Identity.Name, $"updated guarantorOrSuretyDetail allonace", Modules.HRM);
            }

            return Json(new[] { guarantorOrSuretyDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuarantorOrSuretyDetails_Destroy([DataSourceRequest]DataSourceRequest request, GuarantorOrSuretyDetail guarantorOrSuretyDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                guarantorOrSuretyDetail.LastModified = DateTime.Now;
                guarantorOrSuretyDetail.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(guarantorOrSuretyDetail);
                #region delete log

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted guarantorOrSuretyDetail allonace  information of employee " + guarantorOrSuretyDetail.GuarantorOrSuretyDetailId);
                
                #endregion
                db.GuarantorOrSuretyDetails.Attach(entity);
                db.GuarantorOrSuretyDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guarantorOrSuretyDetail }.ToDataSourceResult(request, ModelState));
        }

        private GuarantorOrSuretyDetail GetMappedObject(GuarantorOrSuretyDetail guarantorOrSuretyDetail)
        {
            var entity = new GuarantorOrSuretyDetail
            {
                GuarantorOrSuretyDetailId = guarantorOrSuretyDetail.GuarantorOrSuretyDetailId,
                GuarantorOrSuretyId = guarantorOrSuretyDetail.GuarantorOrSuretyId,
                FullName = guarantorOrSuretyDetail.FullName,
                Organiztion = guarantorOrSuretyDetail.Organiztion,
                FromDate = guarantorOrSuretyDetail.FromDate,
                FromDateAm = guarantorOrSuretyDetail.FromDateAm,
                ToDate = guarantorOrSuretyDetail.ToDate,
                ToDateAm = guarantorOrSuretyDetail.ToDateAm,
                IsActive = guarantorOrSuretyDetail.IsActive,
                OrgBranchName = guarantorOrSuretyDetail.OrgBranchName,
                DateCreated = guarantorOrSuretyDetail.DateCreated,
                LastModified = guarantorOrSuretyDetail.LastModified,
                CreatedBy = guarantorOrSuretyDetail.CreatedBy,
                ModifiedBy = guarantorOrSuretyDetail.ModifiedBy

            };
            return entity;
        }
      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

   
}
