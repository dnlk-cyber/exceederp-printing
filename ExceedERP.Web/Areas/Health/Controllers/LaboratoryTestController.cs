using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
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

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class LaboratoryTestController : Controller
    {
        // GET: HRM/LaboratoryTest
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
        }
        public ActionResult LaboratoryTest_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var tests = db.LaboratoryTests.Where(x => x.LaboratoryCatagoryId == id);
            DataSourceResult result = tests.ToDataSourceResult(request, laboratoryTests => new
            {
                LaboratoryCatagoryId = laboratoryTests.LaboratoryCatagoryId,
                LaboratoryTestsId = laboratoryTests.LaboratoryTestsId,
                Reference = laboratoryTests.Reference,
                Result = laboratoryTests.Result,
                Test = laboratoryTests.Test,
                DateCreated = laboratoryTests.DateCreated,
                CreatedBy = laboratoryTests.CreatedBy,
                DateModified = laboratoryTests.DateModified,
                ModifiedBy = laboratoryTests.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryTest_Create([DataSourceRequest]DataSourceRequest request, LaboratoryTests laboratoryTests, int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new LaboratoryTests
                {
                    LaboratoryCatagoryId = id,
                    Reference = laboratoryTests.Reference,
                    Result = laboratoryTests.Result,
                    Test = laboratoryTests.Test,
                    DateCreated = laboratoryTests.DateCreated,
                    CreatedBy = laboratoryTests.CreatedBy,
                    DateModified = laboratoryTests.DateModified,
                    ModifiedBy = laboratoryTests.ModifiedBy,
                };

                db.LaboratoryTests.Add(entity);
                db.SaveChanges();
                laboratoryTests.LaboratoryTestsId = entity.LaboratoryTestsId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratoryTests", "Success", User.Identity.Name, User.Identity.Name + " add Laboratory Tests called "+entity.Test+" at date " + DateTime.Today, Modules.Health);

                #endregion
            }

            return Json(new[] { laboratoryTests }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryTest_Update([DataSourceRequest]DataSourceRequest request, LaboratoryTests laboratoryTests)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryTests
                {
                    LaboratoryCatagoryId = laboratoryTests.LaboratoryCatagoryId,
                    LaboratoryTestsId = laboratoryTests.LaboratoryTestsId,
                    Reference = laboratoryTests.Reference,
                    Result = laboratoryTests.Result,
                    Test = laboratoryTests.Test,
                    DateCreated = laboratoryTests.DateCreated,
                    CreatedBy = laboratoryTests.CreatedBy,
                    DateModified = laboratoryTests.DateModified,
                    ModifiedBy = laboratoryTests.ModifiedBy,
                };


                db.LaboratoryTests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratoryTests", "Success", User.Identity.Name, User.Identity.Name + " update Laboratory Tests called " + entity.Test + " at date " + DateTime.Today, Modules.Health);

                #endregion
            }

            return Json(new[] { laboratoryTests }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryTest_Destroy([DataSourceRequest]DataSourceRequest request, LaboratoryTests laboratoryTests)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryTests
                {
                    LaboratoryCatagoryId = laboratoryTests.LaboratoryCatagoryId,
                    LaboratoryTestsId = laboratoryTests.LaboratoryTestsId,
                    Reference = laboratoryTests.Reference,
                    Result = laboratoryTests.Result,
                    Test = laboratoryTests.Test,
                    DateCreated = laboratoryTests.DateCreated,
                    CreatedBy = laboratoryTests.CreatedBy,
                    DateModified = laboratoryTests.DateModified,
                    ModifiedBy = laboratoryTests.ModifiedBy,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Laboratory Test called "+entity.Test);
                #endregion
                db.LaboratoryTests.Attach(entity);
                db.LaboratoryTests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { laboratoryTests }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}