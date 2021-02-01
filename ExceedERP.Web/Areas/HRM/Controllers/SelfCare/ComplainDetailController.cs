using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Core.Domain.Global;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class ComplainDetailController : Controller
    {
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
       
        public ActionResult ComplainDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ComplainDetail> complains = db.ComplainDetails.Where(x => x.ComplainId == id);
            DataSourceResult result = complains.ToDataSourceResult(request, complain => new
            {
                ComplainId = complain.ComplainId,
                Remark = complain.Remark,
                ComplainDetailId = complain.ComplainDetailId,
                Response = complain.Response,
                Date = complain.Date,
                DateAm = complain.DateAm
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainDetail_Create([DataSourceRequest]DataSourceRequest request, ComplainDetail complain,int id)
        {
            ModelState.Clear();
            var dateam = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day,DateTime.Today.Month,DateTime.Today.Year);
            if (ModelState.IsValid)
            {
                 
                    var entity = new ComplainDetail
                    {
                        ComplainId = id,
                        Response = complain.Response,
                        Date = DateTime.Today,
                        DateAm = dateam,
                        Remark = complain.Remark
                    };

                    db.ComplainDetails.Add(entity);
                    db.SaveChanges();
                    complain.ComplainId = entity.ComplainId;
               
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainDetail_Update([DataSourceRequest]DataSourceRequest request, ComplainDetail complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
               
                    var entity = new ComplainDetail
                    {
                        ComplainId = complain.ComplainId,
                        ComplainDetailId = complain.ComplainDetailId,
                        Response = complain.Response,
                        Date = complain.Date,
                        DateAm = complain.DateAm,
                        Remark = complain.Remark
                    };

                    db.ComplainDetails.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
               
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainDetail_Destroy([DataSourceRequest]DataSourceRequest request, ComplainDetail complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ComplainDetail
                {
                    ComplainId = complain.ComplainId,
                    ComplainDetailId = complain.ComplainDetailId,
                    Response = complain.Response,
                    Date = complain.Date,
                    DateAm = complain.DateAm,
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
