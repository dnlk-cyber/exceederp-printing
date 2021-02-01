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
    public class TrainingOneByOneObjectDataSourse
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingOneByeOneViewModel> GetTrainingsOneByOne(string year, string trainingType, string training, string branch)
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
                trainings = trainings.Where(x => x.GlFiscalYearId.ToString() == year).ToList();
            }
            if (tType != null)
            {
                trainings = trainings.Where(x => x.TrainingType == tType).ToList();
            }
            if (training != null)
            {
                trainings = trainings.Where(x => x.LookupId.ToString() == training).ToList();
            }
            if(branch != null)
            {
                trainings = trainings.Where(x => x.OrgBranchName.ToString() == branch).ToList() ;
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
                tr.TrainingEmployees = new List<TrainingEmployee>();

                var trainees = db.Trainee.Where(x => x.CourseTrackingId == train.CourseTrackingId).ToList();
                foreach (var tra in trainees)
                {
                    var trainee = new TrainingEmployee();

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

                    tr.TrainingEmployees.Add(trainee);
                }

                trainingLists.Add(tr);
            }

            return trainingLists;
        }
    }
}
