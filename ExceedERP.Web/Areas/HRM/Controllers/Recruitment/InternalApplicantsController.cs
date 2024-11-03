using System.Data;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class InternalApplicantsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public string GetApplicant(int? empId, int intAppId)
        {
            if(empId != null)
            {
                var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == empId);
                if(person != null)
                {
                    return person.FullNameaEnglish;
                }
            }
            else
            {
                var app = db.InternalApplicants.FirstOrDefault(x=>x.InternalApplicantsId == intAppId);
                if(app != null)
                {
                    return app.ApplicantName;
                }
            }
            return "";
        }
        public ActionResult InternalApplicants_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<InternalApplicants> internalapplicants = db.InternalApplicants.Where(x=>x.ApplicationFormId == id);
            DataSourceResult result = internalapplicants.ToDataSourceResult(request, internalApplicants => new {
                InternalApplicantsId = internalApplicants.InternalApplicantsId,
                ApplicationFormId = internalApplicants.ApplicationFormId,
                EmployeeId = internalApplicants.EmployeeId,
                ApplicantName = GetApplicant(internalApplicants.EmployeeId, internalApplicants.InternalApplicantsId)
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
