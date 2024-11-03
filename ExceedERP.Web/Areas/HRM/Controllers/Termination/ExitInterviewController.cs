
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
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
    public class ExitInterviewController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        
        [AuthorizeRoles(HrmConstants.HRTerminationUser, HrmConstants.HRAdmin)]
        public ActionResult ExitInterviews_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ExitInterview> exitinterviews = db.ExitInterviews.Where( e=> e.EmployeeId ==id);
            DataSourceResult result = exitinterviews.ToDataSourceResult(request, exitInterview => new {
                ExitIntervId = exitInterview.ExitIntervId,
                EmployeeId = exitInterview.EmployeeId,
                ToKeep = exitInterview.ToKeep,
                ToRemove = exitInterview.ToRemove,
                Reason = exitInterview.Reason,
                DueToWork = exitInterview.DueToWork,
                Sicknees = exitInterview.Sicknees,
                MainReason = exitInterview.MainReason,
                OtherReason = exitInterview.OtherReason,
                Remark = exitInterview.Remark
            });

            return Json(result);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExitInterviews_Create([DataSourceRequest]DataSourceRequest request, ExitInterview exitInterview, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExitInterview
                {
                    EmployeeId = id,
                    ToKeep = exitInterview.ToKeep,
                    ToRemove = exitInterview.ToRemove,
                    Reason = exitInterview.Reason,
                    DueToWork = exitInterview.DueToWork,
                    Sicknees = exitInterview.Sicknees,
                    MainReason = exitInterview.MainReason,
                    OtherReason = exitInterview.OtherReason,
                    Remark = exitInterview.Remark
                };

                db.ExitInterviews.Add(entity);
                db.SaveChanges();
                exitInterview.ExitIntervId = entity.ExitIntervId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Exit Interview", "Success", User.Identity.Name, User.Identity.Name + "add Exit Interview Info for " + person.EmployeeCode,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { exitInterview }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExitInterviews_Update([DataSourceRequest]DataSourceRequest request, ExitInterview exitInterview)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExitInterview
                {
                    ExitIntervId = exitInterview.ExitIntervId,
                    EmployeeId = exitInterview.EmployeeId,
                    ToKeep = exitInterview.ToKeep,
                    ToRemove = exitInterview.ToRemove,
                    Reason = exitInterview.Reason,
                    DueToWork = exitInterview.DueToWork,
                    Sicknees = exitInterview.Sicknees,
                    MainReason = exitInterview.MainReason,
                    OtherReason = exitInterview.OtherReason,
                    Remark = exitInterview.Remark
                };

                db.ExitInterviews.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                    if (name != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Exit Interview", "Success", User.Identity.Name, name.EmployeeCode + "update Exit Interview Info of " + person.EmployeeCode,Modules.HRM);

                    }
                }
                #endregion
            }

            return Json(new[] { exitInterview }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExitInterviews_Destroy([DataSourceRequest]DataSourceRequest request, ExitInterview exitInterview)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExitInterview
                {
                    ExitIntervId = exitInterview.ExitIntervId,
                    EmployeeId = exitInterview.EmployeeId,
                    ToKeep = exitInterview.ToKeep,
                    ToRemove = exitInterview.ToRemove,
                    Reason = exitInterview.Reason,
                    DueToWork = exitInterview.DueToWork,
                    Sicknees = exitInterview.Sicknees,
                    MainReason = exitInterview.MainReason,
                    OtherReason = exitInterview.OtherReason,
                    Remark = exitInterview.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted exit interview information of employee " + exitInterview.EmployeeCode);
                #endregion
                db.ExitInterviews.Attach(entity);
                db.ExitInterviews.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { exitInterview }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
