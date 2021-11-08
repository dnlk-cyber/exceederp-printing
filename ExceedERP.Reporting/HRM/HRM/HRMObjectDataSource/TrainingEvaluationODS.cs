using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TrainingEvaluationODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingEvaluationCategory> GetEvaluation(string courseTrackingId, string empId)
        {
            List<TrainingEvaluationCategory> trainingEvaluationList = new List<TrainingEvaluationCategory>();

            if (courseTrackingId != null && empId != null)
            {
                int.TryParse(courseTrackingId, out int courseTraId);
                int.TryParse(empId, out int employeeId);


                var categories = db.TrainingEvaluationCriteriaCategories.ToList();

                foreach (var cat in categories)
                {
                    var category = new TrainingEvaluationCategory();

                    category.TrainingEvaluations = new List<TrainingEvaluationVM>();

                    category.Category = cat.Name;

                    var evaluationCriterias = db.TrainingEvaluationCriterias.Where(x => x.TrainingEvaluationSettingCategoryId == cat.TrainingEvaluationSettingCategoryId).ToList();

                    foreach (var ev in evaluationCriterias)
                    {
                        var evaluation = new TrainingEvaluationVM();

                        var evalual = db.TrainingEvaluations.FirstOrDefault(x => x.CriteriaId == ev.TrainingEvaluationCriteriaId && x.CourseTrackingId == courseTraId && x.EmployeeId == employeeId);

                        if (evalual != null)
                        {
                            evaluation.Criteria = ev.Criteria;

                            switch (evalual.Score)
                            {
                                case TrainingScore.በጣም_ከፍተኛ:
                                    evaluation.Excellent = "✓";
                                    break;
                                case TrainingScore.ከፍተኛ:
                                    evaluation.VeryGood = "✓";
                                    break;
                                case TrainingScore.መካከለኛ:
                                    evaluation.Good = "✓";
                                    break;
                                case TrainingScore.ዝቅተኛ:
                                    evaluation.Low = "✓";
                                    break;
                                case TrainingScore.በጣም_ዝቅተኛ:
                                    evaluation.VeryLow = "✓";
                                    break;
                            }

                        }

                        category.TrainingEvaluations.Add(evaluation);

                    }

                    trainingEvaluationList.Add(category);
                }

            }

            return trainingEvaluationList;
        }
    }
}
