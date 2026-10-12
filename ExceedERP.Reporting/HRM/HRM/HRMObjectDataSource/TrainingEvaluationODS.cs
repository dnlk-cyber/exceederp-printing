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
    public class TrainingEvaluationODS:IDisposable
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
                                case TrainingScore.Very_High:
                                    evaluation.Excellent = "✓✓✓✓✓";
                                    break;
                                case TrainingScore.High:
                                    evaluation.VeryGood = "✓✓✓✓";
                                    break;
                                case TrainingScore.Medium:
                                    evaluation.Good = "✓✓✓";
                                    break;
                                case TrainingScore.Low:
                                    evaluation.Low = "✓✓";
                                    break;
                                case TrainingScore.Very_Low:
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
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
