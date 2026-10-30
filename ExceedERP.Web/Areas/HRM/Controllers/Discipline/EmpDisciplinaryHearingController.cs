
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
    
     [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
    public class EmpDisciplinaryHearingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime HearingDate { get; set; }
        public static string HearingDateAmharic { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "HearingDateAmharic")
            {
                HearingDateAmharic = value;
                var splitDate = HearingDateAmharic.Split('/');
                HearingDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult EmpDisciplinaryHearing_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EmpDisciplinaryHearing> empdisciplinaryhearing = db.EmpDisciplinaryHearing.Where(e => e.DisciplinaryCaseId == id);
            DataSourceResult result = empdisciplinaryhearing.ToDataSourceResult(request, empDisciplinaryHearing => new {
                DisciplinaryHearingId = empDisciplinaryHearing.DisciplinaryHearingId,
              
                DisciplinaryCaseId = empDisciplinaryHearing.DisciplinaryCaseId,
                HearingDate = empDisciplinaryHearing.HearingDate,
                HearingOutcome = empDisciplinaryHearing.HearingOutcome,
                Remark = empDisciplinaryHearing.Remark,

                HearingDateAmharic = empDisciplinaryHearing.HearingDateAmharic
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpDisciplinaryHearing_Create([DataSourceRequest]DataSourceRequest request, EmpDisciplinaryHearing empDisciplinaryHearing, int id)
        {
            ModelState.Clear();
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(empDisciplinaryHearing.HearingDateAmharic))
                {
                    ModelState.AddModelError("", " Hearing Date field is mandatory");
                    return Json(new[] { empDisciplinaryHearing }.ToDataSourceResult(request, ModelState));
                }

                empDisciplinaryHearing.HearingDate = HrHelper.ParseEthiopicDate(empDisciplinaryHearing.HearingDateAmharic);


            }
            else
            {

                empDisciplinaryHearing.HearingDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(empDisciplinaryHearing.HearingDate.Day, empDisciplinaryHearing.HearingDate.Month, empDisciplinaryHearing.HearingDate.Year);

            }
            if (HearingDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Hearing Date should be less or equal to the current date!!");
                return Json(new[] { empDisciplinaryHearing }.ToDataSourceResult(request, ModelState));

            }
            empDisciplinaryHearing.DisciplinaryCaseId = id;
            if (ModelState.IsValid)
            {
               
               
                var entity = new EmpDisciplinaryHearing
                {

                    DisciplinaryCaseId = id,
                    HearingDate = empDisciplinaryHearing.HearingDate,
                    HearingOutcome = empDisciplinaryHearing.HearingOutcome,
                    Remark = empDisciplinaryHearing.Remark,
                    HearingDateAmharic = empDisciplinaryHearing.HearingDateAmharic
                };

                db.EmpDisciplinaryHearing.Add(entity);
                db.SaveChanges();
                empDisciplinaryHearing.DisciplinaryHearingId = entity.DisciplinaryHearingId;

                #region operation log

                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "EmpDisciplinaryHearing", "Success", User.Identity.Name, User.Identity.Name + " add employee Displine Hearing hearing date " + entity.HearingDateAmharic, Modules.HRM);

                #endregion
                
            }

            return Json(new[] { empDisciplinaryHearing }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpDisciplinaryHearing_Update([DataSourceRequest]DataSourceRequest request, EmpDisciplinaryHearing empDisciplinaryHearing)
        {
            ModelState.Clear();
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(empDisciplinaryHearing.HearingDateAmharic))
                {
                    ModelState.AddModelError("", " Hearing  field is mandatory");
                    return Json(new[] { empDisciplinaryHearing }.ToDataSourceResult(request, ModelState));
                }

                empDisciplinaryHearing.HearingDate = HrHelper.ParseEthiopicDate(empDisciplinaryHearing.HearingDateAmharic);


            }
            else
            {

                empDisciplinaryHearing.HearingDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(empDisciplinaryHearing.HearingDate.Day, empDisciplinaryHearing.HearingDate.Month, empDisciplinaryHearing.HearingDate.Year);

            }
            if (HearingDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Hearing Date should be less or equal to the current date!!");
                return Json(new[] { empDisciplinaryHearing }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
               
                var entity = new EmpDisciplinaryHearing
                {
                    DisciplinaryHearingId = empDisciplinaryHearing.DisciplinaryHearingId,

                    DisciplinaryCaseId = empDisciplinaryHearing.DisciplinaryCaseId,
                    HearingDate = empDisciplinaryHearing.HearingDate,
                    HearingOutcome = empDisciplinaryHearing.HearingOutcome,
                    Remark = empDisciplinaryHearing.Remark,
                    HearingDateAmharic = empDisciplinaryHearing.HearingDateAmharic
                };

                db.EmpDisciplinaryHearing.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "EmpDisciplinaryHearing", "Success", User.Identity.Name, User.Identity.Name + " update employee Displine Hearing hearing date " + entity.HearingDateAmharic, Modules.HRM);

                #endregion

                
            }

            return Json(new[] { empDisciplinaryHearing }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpDisciplinaryHearing_Destroy([DataSourceRequest]DataSourceRequest request, EmpDisciplinaryHearing empDisciplinaryHearing)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmpDisciplinaryHearing
                {
                    DisciplinaryHearingId = empDisciplinaryHearing.DisciplinaryHearingId,
                   
                    DisciplinaryCaseId = empDisciplinaryHearing.DisciplinaryCaseId,
                    HearingDate = empDisciplinaryHearing.HearingDate,
                    HearingOutcome = empDisciplinaryHearing.HearingOutcome,
                    Remark = empDisciplinaryHearing.Remark,
                    HearingDateAmharic = empDisciplinaryHearing.HearingDateAmharic
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline hearing information of case Id " + empDisciplinaryHearing.DisciplinaryCaseId);
                #endregion
                db.EmpDisciplinaryHearing.Attach(entity);
                db.EmpDisciplinaryHearing.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { empDisciplinaryHearing }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
