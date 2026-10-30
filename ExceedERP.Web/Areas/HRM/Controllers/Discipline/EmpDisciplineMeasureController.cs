
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
    public class EmpDisciplineMeasureController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DecisionDateAm")
            {
               string  DecisionDateAmharic = value;
                var splitDate = DecisionDateAmharic.Split('/');
               DateTime DecisionDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult EmpDisciplineMeasure_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EmpDisciplineMeasure> empdisciplinemeasure = db.EmpDisciplineMeasure.Where( e=> e.DisciplinaryCaseId ==id);
            DataSourceResult result = empdisciplinemeasure.ToDataSourceResult(request, empDisciplineMeasure => new {
                EmpDisciplineMeasureId = empDisciplineMeasure.EmpDisciplineMeasureId,
             
                DisciplinaryCaseId = empDisciplineMeasure.DisciplinaryCaseId,
                MeasuresId = empDisciplineMeasure.MeasuresId,
                DetailMeasuresId = empDisciplineMeasure.DetailMeasuresId,
                DecisionDate = empDisciplineMeasure.DecisionDate,
                IsExpired = empDisciplineMeasure.IsExpired,
                IsReversedByAppeal = empDisciplineMeasure.IsReversedByAppeal,
                Remark = empDisciplineMeasure.Remark,
                Month = empDisciplineMeasure.Month,
                DecisionDateAm = empDisciplineMeasure.DecisionDateAm
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpDisciplineMeasure_Create([DataSourceRequest]DataSourceRequest request, EmpDisciplineMeasure empDisciplineMeasure, int id)
        {
            ModelState.Clear();
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(empDisciplineMeasure.DecisionDateAm))
                {
                    ModelState.AddModelError("", " Decision Date field is mandatory");
                    return Json(new[] { empDisciplineMeasure }.ToDataSourceResult(request, ModelState));
                }

                empDisciplineMeasure.DecisionDate = HrHelper.ParseEthiopicDate(empDisciplineMeasure.DecisionDateAm);


            }
            else
            {

                empDisciplineMeasure.DecisionDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(empDisciplineMeasure.DecisionDate.Day, empDisciplineMeasure.DecisionDate.Month, empDisciplineMeasure.DecisionDate.Year);

            }
            if (empDisciplineMeasure.DecisionDate.Date > DateTime.Now.Date.Date)
            {

                ModelState.AddModelError("", "Decision Date should be less or equal to the current date!!");
                return Json(new[] { empDisciplineMeasure }.ToDataSourceResult(request, ModelState));

            }
            empDisciplineMeasure.DisciplinaryCaseId = id;
            if (ModelState.IsValid)
            {
                
                var entity = new EmpDisciplineMeasure
                {

                    DisciplinaryCaseId = id,
                    MeasuresId = empDisciplineMeasure.MeasuresId,
                    DetailMeasuresId = empDisciplineMeasure.DetailMeasuresId,
                    DecisionDate = empDisciplineMeasure.DecisionDate,
                    IsExpired = empDisciplineMeasure.IsExpired,
                    IsReversedByAppeal = empDisciplineMeasure.IsReversedByAppeal,
                    Remark = empDisciplineMeasure.Remark,
                    Month = empDisciplineMeasure.Month,
                    DecisionDateAm = empDisciplineMeasure.DecisionDateAm
                };

                db.EmpDisciplineMeasure.Add(entity);
                db.SaveChanges();
                empDisciplineMeasure.EmpDisciplineMeasureId = entity.EmpDisciplineMeasureId;

                #region operation log

                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "EmpDisciplinaryMeasure", "Success", User.Identity.Name, User.Identity.Name + " add employee Displine measure decision Date " + entity.DecisionDateAm, Modules.HRM);

                #endregion

                
            }

            return Json(new[] { empDisciplineMeasure }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpDisciplineMeasure_Update([DataSourceRequest]DataSourceRequest request, EmpDisciplineMeasure empDisciplineMeasure)
        {
            ModelState.Clear();
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(empDisciplineMeasure.DecisionDateAm))
                {
                    ModelState.AddModelError("", " Decision Date field is mandatory");
                    return Json(new[] { empDisciplineMeasure }.ToDataSourceResult(request, ModelState));
                }

                empDisciplineMeasure.DecisionDate = HrHelper.ParseEthiopicDate(empDisciplineMeasure.DecisionDateAm);


            }
            else
            {

                empDisciplineMeasure.DecisionDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(empDisciplineMeasure.DecisionDate.Day, empDisciplineMeasure.DecisionDate.Month, empDisciplineMeasure.DecisionDate.Year);

            }
            DateTime tommorow = DateTime.Now.AddDays(1);
            if (empDisciplineMeasure.DecisionDate >= tommorow.Date)
            {

                ModelState.AddModelError("", "Decision Date should be less or equal to the current date!!");
                return Json(new[] { empDisciplineMeasure }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                
                var entity = new EmpDisciplineMeasure
                {
                    EmpDisciplineMeasureId = empDisciplineMeasure.EmpDisciplineMeasureId,

                    DisciplinaryCaseId = empDisciplineMeasure.DisciplinaryCaseId,
                    MeasuresId = empDisciplineMeasure.MeasuresId,
                    DetailMeasuresId = empDisciplineMeasure.DetailMeasuresId,
                    DecisionDate = empDisciplineMeasure.DecisionDate,
                    IsExpired = empDisciplineMeasure.IsExpired,
                    Month = empDisciplineMeasure.Month,
                    IsReversedByAppeal = empDisciplineMeasure.IsReversedByAppeal,
                    Remark = empDisciplineMeasure.Remark,
                    DecisionDateAm = empDisciplineMeasure.DecisionDateAm
                };

                db.EmpDisciplineMeasure.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "EmpDisciplinaryMeasure", "Success", User.Identity.Name, User.Identity.Name + " update employee Displine measure decision Date " + entity.DecisionDateAm, Modules.HRM);

                #endregion
                
            }

            return Json(new[] { empDisciplineMeasure }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpDisciplineMeasure_Destroy([DataSourceRequest]DataSourceRequest request, EmpDisciplineMeasure empDisciplineMeasure)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmpDisciplineMeasure
                {
                    EmpDisciplineMeasureId = empDisciplineMeasure.EmpDisciplineMeasureId,
                 
                    DisciplinaryCaseId = empDisciplineMeasure.DisciplinaryCaseId,
                    MeasuresId = empDisciplineMeasure.MeasuresId,
                    DetailMeasuresId = empDisciplineMeasure.DetailMeasuresId,
                    DecisionDate = empDisciplineMeasure.DecisionDate,
                    IsExpired = empDisciplineMeasure.IsExpired,
                    IsReversedByAppeal = empDisciplineMeasure.IsReversedByAppeal,
                    Month = empDisciplineMeasure.Month,
                    Remark = empDisciplineMeasure.Remark,
                    DecisionDateAm = empDisciplineMeasure.DecisionDateAm
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline measure information of case Id " + empDisciplineMeasure.DisciplinaryCaseId);
                #endregion
                db.EmpDisciplineMeasure.Attach(entity);
                db.EmpDisciplineMeasure.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { empDisciplineMeasure }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
