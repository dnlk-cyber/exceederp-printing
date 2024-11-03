using System.Data;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class TrainingEvaluationCriteriaController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrainingEvaluationCriterias_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TrainingEvaluationCriteria> trainingevaluationcriterias = db.TrainingEvaluationCriterias.Where(x=>x.TrainingEvaluationSettingCategoryId == id);
            DataSourceResult result = trainingevaluationcriterias.ToDataSourceResult(request, trainingEvaluationCriteria => new {
                TrainingEvaluationCriteriaId = trainingEvaluationCriteria.TrainingEvaluationCriteriaId,
                TrainingEvaluationSettingCategoryId = trainingEvaluationCriteria.TrainingEvaluationSettingCategoryId,
                Criteria = trainingEvaluationCriteria.Criteria,
                Weight = trainingEvaluationCriteria.Weight,
                Result = trainingEvaluationCriteria.Result,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriterias_Create([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteria trainingEvaluationCriteria, int id)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluationCriteria
                {
                    TrainingEvaluationSettingCategoryId = id,
                    Criteria = trainingEvaluationCriteria.Criteria,
                    Weight = trainingEvaluationCriteria.Weight,
                    Result = trainingEvaluationCriteria.Result,
                };

                db.TrainingEvaluationCriterias.Add(entity);
                db.SaveChanges();
                trainingEvaluationCriteria.TrainingEvaluationCriteriaId = entity.TrainingEvaluationCriteriaId;
            }

            return Json(new[] { trainingEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriterias_Update([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteria trainingEvaluationCriteria)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluationCriteria
                {
                    TrainingEvaluationCriteriaId = trainingEvaluationCriteria.TrainingEvaluationCriteriaId,
                    TrainingEvaluationSettingCategoryId = trainingEvaluationCriteria.TrainingEvaluationSettingCategoryId,
                    Criteria = trainingEvaluationCriteria.Criteria,
                    Weight = trainingEvaluationCriteria.Weight,
                    Result = trainingEvaluationCriteria.Result,
                };

                db.TrainingEvaluationCriterias.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriterias_Destroy([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteria trainingEvaluationCriteria)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluationCriteria
                {
                    TrainingEvaluationCriteriaId = trainingEvaluationCriteria.TrainingEvaluationCriteriaId,
                    TrainingEvaluationSettingCategoryId = trainingEvaluationCriteria.TrainingEvaluationSettingCategoryId,
                    Criteria = trainingEvaluationCriteria.Criteria,
                    Weight = trainingEvaluationCriteria.Weight,
                    Result = trainingEvaluationCriteria.Result,
                };

                db.TrainingEvaluationCriterias.Attach(entity);
                db.TrainingEvaluationCriterias.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
