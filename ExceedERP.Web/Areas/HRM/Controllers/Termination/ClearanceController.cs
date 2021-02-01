
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Termination
{
    [AuthorizeRoles(HrmConstants.HRTerminationUser)]
    public class ClearanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Clearances_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Clearance> clearances = db.Clearances.Where( c=> c.EmployeeId ==id);
            DataSourceResult result = clearances.ToDataSourceResult(request, clearance => new {
                EmployeeId = clearance.EmployeeId ,
                ClearanceId = clearance.ClearanceId,
                ReasonOfTermination = clearance.ReasonOfTermination,
                TerminatedSince = clearance.TerminatedSince,
                Remark = clearance.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Clearances_Create([DataSourceRequest]DataSourceRequest request, Clearance clearance,int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new Clearance
                {
                    EmployeeId = id,
                    ReasonOfTermination = clearance.ReasonOfTermination,
                    TerminatedSince = clearance.TerminatedSince,
                    Remark = clearance.Remark
                };

                db.Clearances.Add(entity);
                db.SaveChanges();
                clearance.ClearanceId = entity.ClearanceId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Clearance", "Success", User.Identity.Name, User.Identity.Name + "add Clearance Info for " + person.EmployeeCode,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Clearances_Update([DataSourceRequest]DataSourceRequest request, Clearance clearance)
        {
            if (ModelState.IsValid)
            {
                var entity = new Clearance
                {
                    ClearanceId = clearance.ClearanceId,
                    EmployeeId=clearance .EmployeeId,
                    ReasonOfTermination = clearance.ReasonOfTermination,
                    TerminatedSince = clearance.TerminatedSince,
                    Remark = clearance.Remark
                };

                db.Clearances.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Clearance", "Success", User.Identity.Name, User.Identity.Name + "Update Clearance Info of " + person.EmployeeCode,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Clearances_Destroy([DataSourceRequest]DataSourceRequest request, Clearance clearance)
        {
            if (ModelState.IsValid)
            {
                var entity = new Clearance
                {
                    ClearanceId = clearance.ClearanceId,
                    EmployeeId=clearance .EmployeeId,
                    ReasonOfTermination = clearance.ReasonOfTermination,
                    TerminatedSince = clearance.TerminatedSince,
                    Remark = clearance.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted clearance information of employee " + clearance.EmployeeCode);
                #endregion
                db.Clearances.Attach(entity);
                db.Clearances.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
