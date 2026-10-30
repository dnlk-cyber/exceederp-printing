
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using System;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
  
    [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
    public class DisciplineMeasuresTypeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
  
         [AuthorizeRoles(HrmConstants.HRDisciplineUser, HrmConstants.HRAdmin)]
        public ActionResult DisciplineMeasuresType_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<DisciplineMeasuresType> disciplinemeasures = db.DisciplineMeasuresType.Where(x=>x.DetailMeasuresId == id);
            DataSourceResult result = disciplinemeasures.ToDataSourceResult(request, disciplineMeasures => new {
                MeasuresId = disciplineMeasures.MeasuresId,
                DetailMeasuresId = disciplineMeasures.DetailMeasuresId,
                DisciplineTypeId = disciplineMeasures.DisciplineTypeId,
                ExpireYear = disciplineMeasures.ExpireYear,
                PercentageEffectOnPromotion = disciplineMeasures.PercentageEffectOnPromotion,
                Month = disciplineMeasures.Month,
                IsDeductedFromSalary = disciplineMeasures.IsDeductedFromSalary,
                SalaryDeductionPercentage = disciplineMeasures.SalaryDeductionPercentage,
                Day = disciplineMeasures.Day,
                Remark = disciplineMeasures.Remark,
                IsByMoney = disciplineMeasures.IsByMoney,
                Amount = disciplineMeasures.Amount,
                Rank = disciplineMeasures.Rank,
                DateCreated = disciplineMeasures.DateCreated,
                CreatedBy = disciplineMeasures.CreatedBy,
                LastModified = disciplineMeasures.LastModified,
                ModifiedBy = disciplineMeasures.ModifiedBy
            });

            return Json(result);
        }
        
         [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuresType_Create([DataSourceRequest]DataSourceRequest request, DisciplineMeasuresType disciplineMeasures, int id)
        {
            disciplineMeasures.DetailMeasuresId = id;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(disciplineMeasures);
                db.DisciplineMeasuresType.Add(entity);
                db.SaveChanges();
                disciplineMeasures.DisciplineTypeId = entity.DisciplineTypeId;
                #region operation log
                // var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplineMeasuresType", "Success", User.Identity.Name, User.Identity.Name + " add displine Measure Type expire year called " + entity.ExpireYear + "at date" + DateTime.Today, Modules.HRM);

                #endregion
            }

            return Json(new[] { disciplineMeasures }.ToDataSourceResult(request, ModelState));
        }
        
         [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuresType_Update([DataSourceRequest]DataSourceRequest request, DisciplineMeasuresType disciplineMeasures)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(disciplineMeasures);
                db.DisciplineMeasuresType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplineMeasuresType", "Success", User.Identity.Name, User.Identity.Name + " add displine Measure Type expire year called " + entity.ExpireYear+"at date"+DateTime.Today, Modules.HRM);

                #endregion
            }

            return Json(new[] { disciplineMeasures }.ToDataSourceResult(request, ModelState));
        }
       
         [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuresType_Destroy([DataSourceRequest]DataSourceRequest request, DisciplineMeasuresType disciplineMeasures)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(disciplineMeasures);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline measures type information of Discipline Type Id " + disciplineMeasures.DisciplineTypeId);
                #endregion
                db.DisciplineMeasuresType.Attach(entity);
                db.DisciplineMeasuresType.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { disciplineMeasures }.ToDataSourceResult(request, ModelState));
        }
        private DisciplineMeasuresType GetMappedObject(DisciplineMeasuresType disciplineMeasures)
        {
            var entity = new DisciplineMeasuresType
            {
                MeasuresId = disciplineMeasures.MeasuresId,
                DetailMeasuresId = disciplineMeasures.DetailMeasuresId,
                DisciplineTypeId = disciplineMeasures.DisciplineTypeId,
                ExpireYear = disciplineMeasures.ExpireYear,
                PercentageEffectOnPromotion = disciplineMeasures.PercentageEffectOnPromotion,
                IsDeductedFromSalary = disciplineMeasures.IsDeductedFromSalary,
                SalaryDeductionPercentage = disciplineMeasures.SalaryDeductionPercentage,
                Month = disciplineMeasures.Month,
                Day = disciplineMeasures.Day,
                Remark = disciplineMeasures.Remark,
                IsByMoney = disciplineMeasures.IsByMoney,
                Amount = disciplineMeasures.Amount,
                Rank = disciplineMeasures.Rank,
                DateCreated = disciplineMeasures.DateCreated,
                CreatedBy = disciplineMeasures.CreatedBy,
                LastModified = disciplineMeasures.LastModified,
                ModifiedBy = disciplineMeasures.ModifiedBy
            };

            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
