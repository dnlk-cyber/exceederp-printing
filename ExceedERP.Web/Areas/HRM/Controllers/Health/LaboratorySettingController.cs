using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    public class LaboratorySettingController : Controller
    {
        // GET: HRM/LaboratorySetting
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        private void PopulateViewData()
        {
            //ViewData["employees"] = db.Person_Employee.ToList();
            //ViewData["drugList"] = db.DrugLists.ToList();
            //ViewData["drugUnit"] = db.DrugUnits.ToList();
            ViewData["LabTests"] = db.LaboratoryTests.ToList();
        }
        public ActionResult LaboratorySetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            var laboratoryCatagories = db.LaboratoryCatagories.ToList();
            DataSourceResult result = laboratoryCatagories.ToDataSourceResult(request, laboratory => new
            {
                LaboratoryCatagoryId = laboratory.LaboratoryCatagoryId,
                Catagory = laboratory.Catagory,
                DateCreated = laboratory.DateCreated,
                DateModified = laboratory.DateModified,
                ModifiedBy = laboratory.ModifiedBy,
                CreatedBy = laboratory.CreatedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratorySetting_Create([DataSourceRequest]DataSourceRequest request, LaboratoryCatagory laboratory)
        {

            if (ModelState.IsValid)
            {
                var entity = new LaboratoryCatagory
                {
                    Catagory = laboratory.Catagory,
                    DateCreated = DateTime.Today,
                    DateModified = laboratory.DateModified,
                    ModifiedBy = laboratory.ModifiedBy,
                    CreatedBy = laboratory.CreatedBy,
                };

                db.LaboratoryCatagories.Add(entity);
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratorySetting", "Success", User.Identity.Name, User.Identity.Name + " add new Laboratory Setting called "+entity.Catagory + " at date " + DateTime.Today, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { laboratory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratorySetting_Update([DataSourceRequest]DataSourceRequest request, LaboratoryCatagory laboratory)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryCatagory
                {
                    LaboratoryCatagoryId = laboratory.LaboratoryCatagoryId,
                    Catagory = laboratory.Catagory,
                    DateCreated = laboratory.DateCreated,
                    DateModified = DateTime.Today,
                    ModifiedBy = laboratory.ModifiedBy,
                    CreatedBy = laboratory.CreatedBy,
                };


                db.LaboratoryCatagories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratorySetting", "Success", User.Identity.Name, User.Identity.Name + " update Laboratory Setting called " + entity.Catagory + " at date " + DateTime.Today, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { laboratory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratorySetting_Destroy([DataSourceRequest]DataSourceRequest request, LaboratoryCatagory laboratory)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryCatagory
                {
                    LaboratoryCatagoryId = laboratory.LaboratoryCatagoryId,
                    Catagory = laboratory.Catagory,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Laboratory Setting called " + entity.Catagory);
                #endregion
                db.LaboratoryCatagories.Attach(entity);
                db.LaboratoryCatagories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { laboratory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}