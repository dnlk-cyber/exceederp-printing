﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class TaxController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Taxs_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<Tax> taxs = db.Taxs;
        //    DataSourceResult result = taxs.ToDataSourceResult(request, tax => new {
        //        TaxID = tax.TaxID,
        //        Category = tax.Category,
        //        Description = tax.Description,
        //        Amount = tax.Amount,
        //        CreatedBy = tax.CreatedBy,
        //        DateCreated = tax.DateCreated,
        //        LastModified = tax.LastModified,
        //        ModifiedBy = tax.ModifiedBy,
        //        OrgBranchName = tax.OrgBranchName,
        //        Remark = tax.Remark
        //    });

        //    return Json(result);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Taxs_Create([DataSourceRequest]DataSourceRequest request, Tax tax)
        //{
        //    List<Tax> taxLists = db.Taxs.Where(ocl => ocl.Description == tax.Description).ToList();

        //    if (taxLists.Any())
        //    {
        //        ModelState.AddModelError("Invalid Index Value", "Tax Already Exists!");
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var entity = new Tax
        //            {
        //                Category = tax.Category,
        //                Description = tax.Description,
        //                Amount = tax.Amount,
        //                CreatedBy = User.Identity.Name,
        //                DateCreated = DateTime.Now,
        //                Remark = tax.Remark
        //            };

        //            db.Taxs.Add(entity);
        //            db.SaveChanges();
        //            tax.TaxID = entity.TaxID;
        //        }
        //    }
        //    return Json(new[] { tax }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Taxs_Update([DataSourceRequest]DataSourceRequest request, Tax tax)
        //{
        //    List<Tax> taxLists = db.Taxs.Where(ocl => ocl.Description == tax.Description).ToList();

        //    if (taxLists.Any())
        //    {
        //        ModelState.AddModelError("Invalid Index Value", "Tax Already Exists!");
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var entity = new Tax
        //            {
        //                TaxID = tax.TaxID,
        //                Category = tax.Category,
        //                Description = tax.Description,
        //                Amount = tax.Amount,
        //                Remark = tax.Remark
        //            };

        //            db.Taxs.Attach(entity);
        //            db.Entry(entity).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //    }

        //    return Json(new[] { tax }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Taxs_Destroy([DataSourceRequest]DataSourceRequest request, Tax tax)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new Tax
        //        {
        //            TaxID = tax.TaxID,
        //            Category = tax.Category,
        //            Description = tax.Description,
        //            Amount = tax.Amount,
        //            Remark = tax.Remark
        //        };

        //        db.Taxs.Attach(entity);
        //        db.Taxs.Remove(entity);
        //        db.SaveChanges();
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Tax number " + tax.TaxID);
      
        //    }

        //    return Json(new[] { tax }.ToDataSourceResult(request, ModelState));
        //}


        //public ActionResult TaxDDBind()
        //{

        //    var customers = db.Taxs.Select(s => new BindingModel
        //    {
        //        Name = s.Description,
        //        Id = s.TaxID.ToString()
        //    });

        //    return Json(customers, JsonRequestBehavior.AllowGet);
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
