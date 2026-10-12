using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class TrainingOneByOneObjectDataSourse:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingOneByeOneViewModel> GetTrainingsOneByOne(string year, string trainingType, string training, string branch, string orgId, string provider)
        {
            List<TrainingOneByeOneViewModel> trainingLists = new List<TrainingOneByeOneViewModel>();
            
            TrainingType? tType = null;
            switch(trainingType)
            {
                case "TRAINING":
                    tType = TrainingType.TRAINING;
                    break;
                case "EDUCATION":
                    tType = TrainingType.EDUCATION;
                    break;                       

            }

            var trainings = db.CourseTrackings.ToList();

            if (year != null)
            {
                int.TryParse(year, out int GlFisicalYearId);
                trainings = trainings.Where(x => x.GlFiscalYearId == GlFisicalYearId).ToList();
            }
            if (tType != null)
            {
                trainings = trainings.Where(x => x.TrainingType == tType).ToList();
            }
            if (training != null)
            {
                int.TryParse(training, out int LookupId);
                trainings = trainings.Where(x => x.LookupId == LookupId).ToList();
            }
            if(branch != null)
            {
                trainings = trainings.Where(x => x.OrgBranchName == branch).ToList() ;
            }
            if(provider != null)
            {
                int.TryParse(provider, out int mainCompanyId);
                trainings = trainings.Where(x=>x.MainCompanyId == mainCompanyId).ToList();
            }
           
            foreach (var train in trainings)
            {
                var tr = new TrainingOneByeOneViewModel();

                var look = db.Lookups.FirstOrDefault(x => x.LookupId == train.LookupId);
                if (look != null)
                {
                    tr.Training = look.Description;
                }
                var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == train.GlFiscalYearId);
                if (fiscalYear != null)
                {
                    tr.Year = fiscalYear.Name;
                }
                var prov = db.Companies.FirstOrDefault(x => x.MainCompanyId == train.MainCompanyId);
                if(prov != null)
                {
                    tr.Provider = prov.TradeName;
                }

                tr.Cost = train.Cost;

                tr.TrainingEmployees = new List<TrainingEmployee>();

                var trainees = db.Trainee.Where(x => x.CourseTrackingId == train.CourseTrackingId).ToList();
                if (orgId != null)
                {
                    int.TryParse(orgId, out int orgStructureId);
                    var empIds = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(db.Placements.ToList(),orgStructureId).Select(x=>x.EmployeeId).ToList();
                    trainees = trainees.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                }
               
                foreach (var tra in trainees)
                {
                    var trainee = new TrainingEmployee();

                    trainee.CostSingle = tr.Cost/trainees.Count;

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == tra.EmployeeId);
                    if (person != null)
                    {
                        trainee.EmployeeName = person.FullNameAmharic;
                    }
                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == tra.EmployeeId && x.isCurrent);
                    if (placement != null)
                    {
                        var bra = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        if (bra != null)
                        {
                            trainee.Branch = bra.AmharicName;
                        }
                        var depo = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (depo != null)
                        {
                            trainee.WorkUnit = depo.Name;
                        }
                    }

                    bool check = tra.IsCompleteTraining;
                    if(check)
                    {
                        trainee.Status = "Complete";
                    }
                    else
                    {
                        trainee.Status = "In Complete";
                    }                   

                    tr.TrainingEmployees.Add(trainee);
                }

                if(tr.TrainingEmployees.Any())
                {
                    tr.TotalEmployee = tr.TrainingEmployees.Count;
                    trainingLists.Add(tr);
                }             
            }

            return trainingLists;
        }
        public void Dispose()
        {
            db.Dispose();
        }

    }
}
