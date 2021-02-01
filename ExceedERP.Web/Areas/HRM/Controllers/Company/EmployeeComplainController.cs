using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class EmployeeComplainController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
            ViewData["TypeList"] = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COMPLAIN_TYPE);
            ViewData["RefersTo"] = db.MainCompanyStructures.ToList();
    }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult Complains_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Complain> complains = db.Complains.Where(x => employees.Contains(x.EmployeeId.ToString()));
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
        public ActionResult Complains_Create([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
                else
                {
                    var entity = new Complain
                    {
                        Date = Date,
                        DateAmharic = DateAmharic,
                        ComplainDescription = complain.ComplainDescription,
                        LookupId = complain.LookupId,
                        Remark = complain.Remark,
                        EmployeeId = complain.EmployeeId,
                        RefersTo = complain.RefersTo
                    };

                    db.Complains.Add(entity);
                    db.SaveChanges();
                    complain.ComplainId = entity.ComplainId;
                }
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }
        //public ActionResult ComplainResponse_Create([DataSourceRequest]DataSourceRequest request, ComplainResponse response, int id)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {

        //        var entity = new ComplainResponse
        //        {
        //            ComplainId = id,
        //            Response = response.Response
        //        };

        //        db.ComplainResponses.Add(entity);
        //        db.SaveChanges();

        //    }

        //    return Json(new[] { response }.ToDataSourceResult(request, ModelState));
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Complains_Update([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = complain.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new Complain
                {
                    ComplainId = complain.ComplainId,
                    EmployeeId = complain.EmployeeId,
                    Date = Date,
                    DateAmharic = DateAmharic,
                    ComplainDescription = complain.ComplainDescription,
                    Remark = complain.Remark,
                    LookupId = complain.LookupId,
                    RefersTo = complain.RefersTo
                };

                db.Complains.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Complains_Destroy([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Complain
                {
                    ComplainId = complain.ComplainId,
                    Date = complain.Date,
                    DateAmharic = complain.DateAmharic,
                    ComplainDescription = complain.ComplainDescription,
                    Remark = complain.Remark,
                    LookupId = complain.LookupId,
                    EmployeeId = complain.EmployeeId,
                    RefersTo = complain.RefersTo
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Complain of employee " + complain.Person_Employee.FirstName + " " + complain.Person_Employee.MiddleName + " " + complain.Person_Employee.LastName);
                #endregion
                db.Complains.Attach(entity);
                db.Complains.Remove(entity);
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
