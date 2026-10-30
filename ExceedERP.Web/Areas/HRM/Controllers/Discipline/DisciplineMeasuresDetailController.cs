
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
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
 
    [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
    public class DisciplineMeasuresDetailController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            GetMeasureDetail();
            return View();
        }

        private void GetMeasureDetail()
        {
            var measureDetails = db.DisciplineMeasuresDetail;
            ViewData["MeasuresDetails"] = measureDetails;
            var measurs = db.DisciplineMeasures;
            ViewData["Measures"] = measurs;
        }
  
        [AuthorizeRoles(HrmConstants.HRDisciplineUser, HrmConstants.HRAdmin)]
        public ActionResult DisciplineMeasuresDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<DisciplineMeasuresDetail> disciplinemeasuresdetail = db.DisciplineMeasuresDetail.Where(d => d.MeasuresId == id);
            DataSourceResult result = disciplinemeasuresdetail.ToDataSourceResult(request, disciplineMeasuresDetail => new
            {
                DetailMeasuresId = disciplineMeasuresDetail.DetailMeasuresId,
                MeasuresId = disciplineMeasuresDetail.MeasuresId,
                Rank = disciplineMeasuresDetail.Rank,
                Level = disciplineMeasuresDetail.Level,
                Description = disciplineMeasuresDetail.Description,
                Remark = disciplineMeasuresDetail.Remark
            });

            return Json(result);
        }
   
        [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuresDetail_Create([DataSourceRequest]DataSourceRequest request, DisciplineMeasuresDetail disciplineMeasuresDetail, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new DisciplineMeasuresDetail
                {
                    MeasuresId = id,
                    Level = disciplineMeasuresDetail.Level,
                    Rank = disciplineMeasuresDetail.Rank,
                    Description = disciplineMeasuresDetail.Description,
                    Remark = disciplineMeasuresDetail.Remark
                };

                db.DisciplineMeasuresDetail.Add(entity);
                db.SaveChanges();
                disciplineMeasuresDetail.DetailMeasuresId = entity.DetailMeasuresId;

                #region operation log

                // var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplineMeasuresDetail", "Success", User.Identity.Name, User.Identity.Name + " update displine Measure Level called " + entity.Level, Modules.HRM);

                #endregion
            }

            return Json(new[] { disciplineMeasuresDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuresDetail_Update([DataSourceRequest]DataSourceRequest request, DisciplineMeasuresDetail disciplineMeasuresDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new DisciplineMeasuresDetail
                {
                    DetailMeasuresId = disciplineMeasuresDetail.DetailMeasuresId,
                    MeasuresId = disciplineMeasuresDetail.MeasuresId,
                    Level = disciplineMeasuresDetail.Level,
                    Rank = disciplineMeasuresDetail.Rank,
                    Description = disciplineMeasuresDetail.Description,
                    Remark = disciplineMeasuresDetail.Remark
                };

                db.DisciplineMeasuresDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                // var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplineMeasuresDetail", "Success", User.Identity.Name, User.Identity.Name + " update displine Measure Level called " + entity.Level, Modules.HRM);

                #endregion
            }

            return Json(new[] { disciplineMeasuresDetail }.ToDataSourceResult(request, ModelState));
        }
  
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuresDetail_Destroy([DataSourceRequest]DataSourceRequest request, DisciplineMeasuresDetail disciplineMeasuresDetail)
        {

         
            var DisciplineRecord = db.DisciplineRecord.Where(x => x.DetailMeasuresId == disciplineMeasuresDetail.DetailMeasuresId).FirstOrDefault();
            if (DisciplineRecord != null)
            {

                ModelState.AddModelError("", "You cannot delete this Measure Detail.Please delete Employees Discipline Record information first.");
                return Json(new[] { disciplineMeasuresDetail }.ToDataSourceResult(request, ModelState));

            }
              
            if (ModelState.IsValid)
            {
               
               
                    var entity = new DisciplineMeasuresDetail
                    {
                        DetailMeasuresId = disciplineMeasuresDetail.DetailMeasuresId,
                        MeasuresId = disciplineMeasuresDetail.MeasuresId,
                        Level = disciplineMeasuresDetail.Level,
                        Rank = disciplineMeasuresDetail.Rank,
                        Description = disciplineMeasuresDetail.Description,
                        Remark = disciplineMeasuresDetail.Remark
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline Measures detail information of name " + disciplineMeasuresDetail.Description);
                    #endregion
                    db.DisciplineMeasuresDetail.Attach(entity);
                    db.DisciplineMeasuresDetail.Remove(entity);
                    db.SaveChanges();
                
            }
            return Json(new[] { disciplineMeasuresDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
