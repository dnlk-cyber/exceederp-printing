using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class VacancyIdInternalObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetVacancyId(string vacancyCod)
        {
            List<Vacancy> list = new List<Vacancy> { };
            // Vacancy vacancy = new Vacancy();
            if (vacancyCod != null)
            {
                var recruitmentVacancy = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == vacancyCod).ToList();
                if (recruitmentVacancy != null)
                {
                    foreach (var recruitment in recruitmentVacancy)
                    {
                        var vacant = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitment.VacancyId && x.RecruSource == RecruitmentSource.INTERNAL);
                        if (vacant != null)
                        {

                            list.Add(vacant);
                        }
                    }
                }
            }



            return list;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }



    [DataObject]
    public class VacancyIdExternalObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetVacancyId(string vacancyCod)
        {
            List<Vacancy> list = new List<Vacancy> { };

            if (vacancyCod != null)
            {
                var recruitmentVacancy = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == vacancyCod).ToList();
                if (recruitmentVacancy.Any())
                {
                    foreach (var recruitment in recruitmentVacancy)
                    {
                        var vacant = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitment.VacancyId && x.RecruSource == RecruitmentSource.EXTERNAL);
                        if (vacant != null)
                        {

                            list.Add(vacant);
                        }
                    }
                }
            }



            return list;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }


    [DataObject]
    public class VacancyIdRecruitmentSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetRecruitmentSummaryVacancyId(string vacancyCod)
        {
            List<Vacancy> list = new List<Vacancy> { };

            if (vacancyCod != null)
            {
                var recruitmentVacancy = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == vacancyCod).ToList();
                if (recruitmentVacancy.Any())
                {
                    foreach (var recruitment in recruitmentVacancy)
                    {
                        var vacant = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitment.VacancyId);
                        if (vacant != null)
                        {
                            list.Add(vacant);
                        }
                    }
                }
            }

            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetActiveVacancies()
        {
            var vacant = db.VaccancyPlans.Where(x => x.IsActive).ToList();
          
            return vacant;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

}
