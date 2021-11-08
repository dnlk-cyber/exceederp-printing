
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{

    [AuthorizeRoles(HrmConstants.HRDisciplineUser, HrmConstants.HRAdmin)]
    public class DisciplineMeasuresController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        string ErrorMessage = "";
        public ActionResult Index()
        {
            return View();
        }
        private bool CheckDisciplineValidation(int id)
        {
            ErrorMessage = "";


            #region DisciplineMeasures
            var DisciplineMeasures = db.DisciplineMeasuresDetail.Where(x => x.MeasuresId == id).FirstOrDefault();
            if (DisciplineMeasures != null)
            {

                ErrorMessage = "You cannot delete this Discipline Measures.Please delete Measure detail information first.";
                return false;


            }
            #endregion
            #region DisciplineMeasuresType
            var DisciplineMeasuresType = db.DisciplineMeasuresType.Where(x => x.MeasuresId == id).FirstOrDefault();
            if (DisciplineMeasuresType != null)
            {

                ErrorMessage = "You cannot delete this Discipline Measures.Please delete Measure type information first.";
                return false;


            }
            #endregion
            #region DisciplineRecord
            var DisciplineRecord = db.DisciplineRecord.Where(x => x.MeasuresId == id).FirstOrDefault();
            if (DisciplineRecord != null)
            {

                ErrorMessage = "You cannot delete this Discipline Measures.Please delete Employees Discipline Record information first.";
                return false;


            }
            #endregion
            return true;
        }

        [AuthorizeRoles(HrmConstants.HRDisciplineUser, HrmConstants.HRAdmin)]
        public ActionResult DisciplineMeasures_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DisciplineMeasures> disciplinemeasures = db.DisciplineMeasures;
            DataSourceResult result = disciplinemeasures.ToDataSourceResult(request, disciplineMeasures => new
            {
                MeasuresId = disciplineMeasures.MeasuresId,
                MeasuresType = disciplineMeasures.MeasuresType,

                Remark = disciplineMeasures.Remark
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasures_Create([DataSourceRequest]DataSourceRequest request, DisciplineMeasures disciplineMeasures)
        {
            if (ModelState.IsValid)
            {
                var checkExstance = db.DisciplineMeasures.ToList();
                var jobName = disciplineMeasures.MeasuresType;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.MeasuresType.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Measure Name already exists in the database!!");
                }
                else
                {
                    var entity = new DisciplineMeasures
                    {
                        MeasuresType = disciplineMeasures.MeasuresType,


                        Remark = disciplineMeasures.Remark
                    };

                    db.DisciplineMeasures.Add(entity);
                    db.SaveChanges();
                    disciplineMeasures.MeasuresId = entity.MeasuresId;

                    #region operation log

                    // var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "DisciplineMeasures", "Success", User.Identity.Name, User.Identity.Name + " add displine Measure called " + entity.MeasuresType, Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { disciplineMeasures }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasures_Update([DataSourceRequest]DataSourceRequest request, DisciplineMeasures disciplineMeasures)
        {
            if (ModelState.IsValid)
            {
                var entity = new DisciplineMeasures
                {
                    MeasuresId = disciplineMeasures.MeasuresId,
                    MeasuresType = disciplineMeasures.MeasuresType,


                    Remark = disciplineMeasures.Remark
                };

                db.DisciplineMeasures.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                // var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplineMeasures", "Success", User.Identity.Name, User.Identity.Name + " update displine Measure called " + entity.MeasuresType, Modules.HRM);

                #endregion
            }

            return Json(new[] { disciplineMeasures }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasures_Destroy([DataSourceRequest]DataSourceRequest request, DisciplineMeasures disciplineMeasures)
        {
            if (ModelState.IsValid)
            {
                var entity = new DisciplineMeasures
                {
                    MeasuresId = disciplineMeasures.MeasuresId,
                    MeasuresType = disciplineMeasures.MeasuresType,


                    Remark = disciplineMeasures.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline Measures information of type " + disciplineMeasures.MeasuresType);
                #endregion
                db.DisciplineMeasures.Attach(entity);
                db.DisciplineMeasures.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { disciplineMeasures }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
