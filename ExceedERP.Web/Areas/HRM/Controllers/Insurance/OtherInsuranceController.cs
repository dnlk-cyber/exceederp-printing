﻿using System;
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
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Insurance
{
  
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin,HrmConstants.HRInsuranceUser)]
    public class OtherInsuranceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
   
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var catagories = db.InsuranceCategory;
            ViewData["InsuranceCategory"] = catagories;
           
            var types = db.InsuranceType.ToList();
            List<InsuranceType> insuranceList = new List<InsuranceType>();
            foreach(var type in types)
            {
                InsuranceType insurance = new InsuranceType();
                 var lookup = db.Lookups.FirstOrDefault(x =>x.LookupId == type.LookupId &&  x.LookupType == Core.Domain.Common.LookUpTypes.INSURANCE_CLASS_OF_BUSINESS);
                if(lookup != null)
                {
                    insurance.InsuranceTypeId = type.InsuranceTypeId;
                    insurance.Name = lookup.Description;
                }
                insuranceList.Add(insurance);
            }
            ViewData["InsuranceType"] = insuranceList;
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromInsuranceAmhric")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToInsuranceAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
   
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
        public ActionResult OtherInsurance_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OtherInsurance> otherinsurance = db.OtherInsurance;
            DataSourceResult result = otherinsurance.ToDataSourceResult(request, otherInsurance => new
            {
                OtherInsuranceId = otherInsurance.OtherInsuranceId,
                InsuranceTypeId = otherInsurance.InsuranceTypeId,
                From = otherInsurance.From,
                To = otherInsurance.To,
                Name = otherInsurance.Name,
                FromInsuranceAmhric = otherInsurance.FromInsuranceAmhric,
                ToInsuranceAmharic = otherInsurance.ToInsuranceAmharic,
                SumofInsured = otherInsurance.SumofInsured,
                Rate = otherInsurance.Rate,
                Premium = otherInsurance.Premium,
                Stamp = otherInsurance.Stamp,
                ThirdParty = otherInsurance.ThirdParty,
                Total = otherInsurance.Total,
                Remark = otherInsurance.Remark,
                CreatedBy = otherInsurance.CreatedBy,
                DateCreated = otherInsurance.DateCreated,
                ModifiedBy = otherInsurance.ModifiedBy,
                LastModified = otherInsurance.LastModified,
                InsuranceCategoryId = otherInsurance.InsuranceCategoryId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
   
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        public ActionResult OtherInsurance_Create([DataSourceRequest]DataSourceRequest request, OtherInsurance otherInsurance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null || ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date values are mandatory!!");
                }
                else
                {

                    var name = "";
                    var type = db.InsuranceType.FirstOrDefault(x=>x.InsuranceTypeId == otherInsurance.InsuranceTypeId);
                    if (type!=null)
                    {
                        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == type.LookupId);
                        if (lookup != null)
                        {
                            name = lookup.Description;
                        }
                    }
                    var entity = new OtherInsurance
                    {
                        InsuranceTypeId = otherInsurance.InsuranceTypeId,
                        InsuranceCategoryId = otherInsurance.InsuranceCategoryId,
                        Name = name,
                        From = From,
                        To = To,
                        FromInsuranceAmhric = FromAmharic,
                        ToInsuranceAmharic = ToAmharic,
                        SumofInsured = otherInsurance.SumofInsured,
                        Rate = otherInsurance.Rate,
                        Premium = otherInsurance.Rate * otherInsurance.SumofInsured,
                        Stamp = otherInsurance.Stamp,
                        ThirdParty = otherInsurance.ThirdParty,
                        Total = otherInsurance.Total,
                        Remark = otherInsurance.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = otherInsurance.ModifiedBy,
                        LastModified = otherInsurance.LastModified
                    };
                    entity.Total = entity.Premium + entity.Stamp + entity.ThirdParty;
                    db.OtherInsurance.Add(entity);
                    db.SaveChanges();
                    otherInsurance.OtherInsuranceId = entity.OtherInsuranceId;

                    #region operation log                

                    UserHelper.OperationLog(Request.UserHostAddress, "OtherInsurance", "Success", User.Identity.Name, User.Identity.Name + " add Other Insurance at Date " + DateTime.Today + " Name " + entity.Name, Modules.HealthAndInsurance);

                    #endregion
                }
            }

            return Json(new[] { otherInsurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
      
        [AuthorizeRoles( HrmConstants.HRInsuranceUser)]
        public ActionResult OtherInsurance_Update([DataSourceRequest]DataSourceRequest request, OtherInsurance otherInsurance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = otherInsurance.FromInsuranceAmhric;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = otherInsurance.ToInsuranceAmharic;
                    var splitDate = ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new OtherInsurance
                {
                    OtherInsuranceId = otherInsurance.OtherInsuranceId,
                    InsuranceTypeId = otherInsurance.InsuranceTypeId,
                    Name = otherInsurance.Name,
                    From = otherInsurance.From,
                    To = otherInsurance.To,
                    FromInsuranceAmhric = FromAmharic,
                    ToInsuranceAmharic = ToAmharic,
                    SumofInsured = otherInsurance.SumofInsured,
                    Rate = otherInsurance.Rate,
                    Premium = otherInsurance.Rate * otherInsurance.SumofInsured,
                    Stamp = otherInsurance.Stamp,
                    ThirdParty = otherInsurance.ThirdParty,
                    Total = otherInsurance.Total,
                    Remark = otherInsurance.Remark,
                    CreatedBy = otherInsurance.CreatedBy,
                    DateCreated = otherInsurance.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    InsuranceCategoryId = otherInsurance.InsuranceCategoryId,
                };

                entity.Total = entity.Premium + entity.Stamp + entity.ThirdParty;
                db.OtherInsurance.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "OtherInsurance", "Success", User.Identity.Name, User.Identity.Name + " update Other Insurance at Date " + DateTime.Today + " Name " + entity.Name, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { otherInsurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
     
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
        public ActionResult OtherInsurance_Destroy([DataSourceRequest]DataSourceRequest request, OtherInsurance otherInsurance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new OtherInsurance
                {
                    OtherInsuranceId = otherInsurance.OtherInsuranceId,
                    InsuranceTypeId = otherInsurance.InsuranceTypeId,
                    Name = otherInsurance.Name,
                    From = otherInsurance.From,
                    To = otherInsurance.To,
                    FromInsuranceAmhric = otherInsurance.FromInsuranceAmhric,
                    ToInsuranceAmharic = otherInsurance.ToInsuranceAmharic,
                    SumofInsured = otherInsurance.SumofInsured,
                    Rate = otherInsurance.Rate,
                    Premium = otherInsurance.Premium,
                    Stamp = otherInsurance.Stamp,
                    ThirdParty = otherInsurance.ThirdParty,
                    Total = otherInsurance.Total,
                    Remark = otherInsurance.Remark,
                    CreatedBy = otherInsurance.CreatedBy,
                    DateCreated = otherInsurance.DateCreated,
                    ModifiedBy = otherInsurance.ModifiedBy,
                    LastModified = otherInsurance.LastModified,
                    InsuranceCategoryId = otherInsurance.InsuranceCategoryId,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted other insrance type  information of type id " + otherInsurance.InsuranceTypeId);
                #endregion
                db.OtherInsurance.Attach(entity);
                db.OtherInsurance.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { otherInsurance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
