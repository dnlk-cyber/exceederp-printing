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
 using ExceedERP.Core.Domain.Insurance;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    
     [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
    public class InsuranceCategoryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
      
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var look = db.Lookups.Where(x => x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS).ToList();
            ViewData["InsuranceType"] = look;
        }
        public ActionResult InsuranceCategory_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
          
           var insurancecategory = db.InsuranceCategory.Where(x => x.InsuranceTypeId.ToString() == id);
            DataSourceResult result = insurancecategory.ToDataSourceResult(request, insuranceCategory => new {
                InsuranceCategoryId = insuranceCategory.InsuranceCategoryId,
                InsuranceTypeId = insuranceCategory.InsuranceTypeId,
                Name = insuranceCategory.Name,
                Remark = insuranceCategory.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceCategory_Create([DataSourceRequest]DataSourceRequest request, InsuranceCategory insuranceCategory, int idTwo)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new InsuranceCategory
                {
                    Name = insuranceCategory.Name,
                    InsuranceTypeId = idTwo,
                    Remark = insuranceCategory.Remark
                };

                db.InsuranceCategory.Add(entity);
                db.SaveChanges();
                insuranceCategory.InsuranceCategoryId = entity.InsuranceCategoryId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Catagory", "Success", User.Identity.Name, User.Identity.Name + " add Insurance Catagory at Date " + DateTime.Today + " insurance Type Group " + entity.Name, Modules.Insurance);

                #endregion
            }

            return Json(new[] { insuranceCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceCategory_Update([DataSourceRequest]DataSourceRequest request, InsuranceCategory insuranceCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new InsuranceCategory
                {
                    InsuranceCategoryId = insuranceCategory.InsuranceCategoryId,
                    Name = insuranceCategory.Name,
                    InsuranceTypeId = insuranceCategory.InsuranceTypeId,
                    Remark = insuranceCategory.Remark
                };

                db.InsuranceCategory.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Catagory", "Success", User.Identity.Name, User.Identity.Name + " update Insurance Catagory at Date " + DateTime.Today + " insurance Type Group " + entity.Name, Modules.Insurance);

                #endregion
            }

            return Json(new[] { insuranceCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceCategory_Destroy([DataSourceRequest]DataSourceRequest request, InsuranceCategory insuranceCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new InsuranceCategory
                {
                    InsuranceCategoryId = insuranceCategory.InsuranceCategoryId,
                    Name = insuranceCategory.Name,
                    InsuranceTypeId = insuranceCategory.InsuranceTypeId,
                    Remark = insuranceCategory.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  insrance category information of category " + insuranceCategory.Name);
                #endregion
                db.InsuranceCategory.Attach(entity);
                db.InsuranceCategory.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { insuranceCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
