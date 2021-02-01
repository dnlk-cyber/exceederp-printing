using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class InVacVMODS : List<InVacVM>
    {


        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InVacVM> GetInternalVacancy()
        {
            List<InVacVM> vacancyLists = new List<InVacVM>();
            InVacVM internalVacancy = new InVacVM();
            var vacancy = db.VaccancyPlans;
            var recruitmentVacancy = db.RecruitmentVacancy;
            var recruitmentPlan = db.RecruitmentPlans;

            if (vacancy.Any())
            {
                foreach (var vac in vacancy)
                {
                    internalVacancy.VacancyCode = vac.VaccancyCode;
                    var recruit = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vac.RecruitmentPlanId);
                    if (recruit != null)
                    {
                        internalVacancy.JobTitle = recruit.JobtitleName;
                    }

                    vacancyLists.Add(internalVacancy);
                }
            }

            return vacancyLists;
        }

    }
}


