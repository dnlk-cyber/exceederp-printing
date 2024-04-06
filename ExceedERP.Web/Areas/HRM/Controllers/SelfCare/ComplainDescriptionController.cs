using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class ComplainDescriptionController : BaseController
    {
        // GET: HRM/ComplainCC
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ComplainDetail
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            ViewData["TypeList"] = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COMPLAIN_TYPE);


            ViewData["CC"] = db.MainCompanyStructures.ToList();
            ViewData["RefersTo"] = db.MainCompanyStructures.ToList();
            ViewData["Employees"] = db.Person_Employee.ToList();
        }

        public ActionResult ComplainDescription_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            IQueryable<Complain> complains = db.Complains;
            DataSourceResult result = complains.ToDataSourceResult(request, complain => new
            {
                ComplainId = complain.ComplainId,
                EmployeeId = complain.EmployeeId,
                Date = complain.Date,
                DateAmharic = complain.DateAmharic,
                ComplainDescription = complain.ComplainDescription,
                Remark = complain.Remark,
                LookupId = complain.LookupId,
                RefersTo = complain.RefersTo
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainDescription_Create([DataSourceRequest]DataSourceRequest request, ComplainDetail complain)
        {
            if (ModelState.IsValid)
            {

                var entity = new ComplainDetail
                {
                    ComplainId = complain.ComplainId,
                    Response = complain.Response,
                    Remark = complain.Remark
                };

                db.ComplainDetails.Add(entity);
                db.SaveChanges();
                complain.ComplainId = entity.ComplainId;

              


            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainDescription_Update([DataSourceRequest]DataSourceRequest request, ComplainDetail complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new ComplainDetail
                {
                    ComplainId = complain.ComplainId,
                    ComplainDetailId = complain.ComplainDetailId,
                    Response = complain.Response,
                    Remark = complain.Remark
                };

                db.ComplainDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainDescription_Destroy([DataSourceRequest]DataSourceRequest request, ComplainDetail complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ComplainDetail
                {
                    ComplainId = complain.ComplainId,
                    ComplainDetailId = complain.ComplainDetailId,
                    Response = complain.Response,
                    Remark = complain.Remark
                };

                db.ComplainDetails.Attach(entity);
                db.ComplainDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}