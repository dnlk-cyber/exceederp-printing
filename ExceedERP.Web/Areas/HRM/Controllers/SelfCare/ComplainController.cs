using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]

    public class ComplainController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        public static int EmployeeId { get; set; }

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            ViewData["TypeList"] = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COMPLAIN_TYPE);
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            ViewData["CC"] = db.MainCompanyStructures.ToList();
            ViewData["Orgs"] = db.MainCompanyStructures.ToList();
            ViewData["Employees"] = db.Person_Employee.ToList();
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
            IQueryable<Complain> complains = db.Complains.Where(x => x.EmployeeId == EmployeeId);
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
        public ActionResult Complainss_Create([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();

            //if (complain.DateAmharic != null)
            //{
            //    var splitDate = complain.DateAmharic.Split('/');
            //    complain.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            //}
            //else
            //{
            //    ModelState.AddModelError("", "Date Is Required");
            //}
            //if (complain.Date > DateTime.Now)
            //{

            //    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
            //}
            complain.Date = DateTime.Now;
            complain.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            complain.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (ModelState.IsValid)
            {
                var entity = new Complain
                {
                    Date = complain.Date,
                    EmployeeId = complain.EmployeeId,
                    DateAmharic = complain.DateAmharic,
                    ComplainDescription = complain.ComplainDescription,
                    LookupId = complain.LookupId,
                    RefersTo = complain.RefersTo,
                    Remark = complain.Remark
                };

                db.Complains.Add(entity);
                db.SaveChanges();
                complain.ComplainId = entity.ComplainId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Complain ", "Success", User.Identity.Name, User.Identity.Name + " add new Complain at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Complains_Update([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();

            //if (complain.DateAmharic != null)
            //{
            //    var splitDate = complain.DateAmharic.Split('/');
            //    complain.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            //}
            //else
            //{
            //    ModelState.AddModelError("", "Date Is Required");
            //}
            //if (complain.Date > DateTime.Now)
            //{

            //    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
            //}
            complain.Date = DateTime.Now;
            complain.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            complain.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);


            if (ModelState.IsValid)
            {
                var entity = new Complain
                {
                    ComplainId = complain.ComplainId,
                    Date = Date,
                    DateAmharic = DateAmharic,
                    EmployeeId = complain.EmployeeId,
                    ComplainDescription = complain.ComplainDescription,
                    Remark = complain.Remark,
                    RefersTo = complain.RefersTo,
                    LookupId = complain.LookupId
                };

                db.Complains.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Complain ", "Success", User.Identity.Name, User.Identity.Name + " update Complain at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

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
                    EmployeeId = complain.EmployeeId,
                    Date = complain.Date,
                    DateAmharic = complain.DateAmharic,
                    ComplainDescription = complain.ComplainDescription,
                    Remark = complain.Remark,
                    RefersTo = complain.RefersTo,
                    LookupId = complain.LookupId
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted complain of employee code  " + complain.EmployeeId);
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
