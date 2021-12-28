using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin)]
    public class EvaluationCriteriaActionTypeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
          
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["PeriodList"] = db.GLFiscalYears.ToList();
        }
      
        public ActionResult EvaluationCriteriaActionType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EvaluationCriteriaActionType> evaluationCriteriaActionTypes = db.EvaluationCriteriaActionTypes;
            DataSourceResult result = evaluationCriteriaActionTypes.ToDataSourceResult(request, evaluationCriteriaActionType => new
            {
                EvaluationCriteriaActionTypeId = evaluationCriteriaActionType.EvaluationCriteriaActionTypeId,
                PerformanceCriteriaActionType = evaluationCriteriaActionType.PerformanceCriteriaActionType,
                IsActive = evaluationCriteriaActionType.IsActive,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaActionType_Create([DataSourceRequest]DataSourceRequest request, EvaluationCriteriaActionType evaluationCriteriaActionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new EvaluationCriteriaActionType
                {
                    PerformanceCriteriaActionType = evaluationCriteriaActionType.PerformanceCriteriaActionType,
                    IsActive = evaluationCriteriaActionType.IsActive,
                };

                db.EvaluationCriteriaActionTypes.Add(entity);
                db.SaveChanges();                
                
            }

            return Json(new[] { evaluationCriteriaActionType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaActionType_Update([DataSourceRequest]DataSourceRequest request, EvaluationCriteriaActionType evaluationCriteriaActionType)
        {
            if (ModelState.IsValid)
            {
                if(evaluationCriteriaActionType.IsActive)
                {
                    var changeOther = db.EvaluationCriteriaActionTypes.FirstOrDefault(x=>x.EvaluationCriteriaActionTypeId != evaluationCriteriaActionType.EvaluationCriteriaActionTypeId);
                    if(changeOther != null)
                    {
                        changeOther.IsActive = false;

                        db.Entry(changeOther).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    var changeOther = db.EvaluationCriteriaActionTypes.FirstOrDefault(x => x.EvaluationCriteriaActionTypeId != evaluationCriteriaActionType.EvaluationCriteriaActionTypeId);
                    if (changeOther != null)
                    {
                        changeOther.IsActive = true;

                        db.Entry(changeOther).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                var entity = new EvaluationCriteriaActionType
                {
                    EvaluationCriteriaActionTypeId = evaluationCriteriaActionType.EvaluationCriteriaActionTypeId,
                    PerformanceCriteriaActionType = evaluationCriteriaActionType.PerformanceCriteriaActionType,
                    IsActive = evaluationCriteriaActionType.IsActive,
                };

                db.EvaluationCriteriaActionTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                               

            }

            return Json(new[] { evaluationCriteriaActionType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaActionType_Destroy([DataSourceRequest]DataSourceRequest request, EvaluationCriteriaActionType evaluationCriteriaActionType)
        {
            if (ModelState.IsValid)
            {

                var entity = new EvaluationCriteriaActionType
                {
                    EvaluationCriteriaActionTypeId = evaluationCriteriaActionType.EvaluationCriteriaActionTypeId,
                    PerformanceCriteriaActionType = evaluationCriteriaActionType.PerformanceCriteriaActionType,
                    IsActive = evaluationCriteriaActionType.IsActive,
                };
              
                db.EvaluationCriteriaActionTypes.Attach(entity);
                db.EvaluationCriteriaActionTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { evaluationCriteriaActionType }.ToDataSourceResult(request, ModelState));
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
