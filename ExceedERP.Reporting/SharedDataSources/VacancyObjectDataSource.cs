using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class VacancyObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetVacancy()
        {
            List<Vacancy> list = new List<Vacancy>();
            var vacancy = db.VaccancyPlans.ToList();

            var config =
                db.Configurations.Where(x => x.Category == "Promotion" && x.Reference == "VACANCY_LIFE_TIME_INDAY")
                    .FirstOrDefault();
            if (config != null)
            {
                int settingValue = Convert.ToInt32(config.CurrentValue);
                list = new List<Vacancy>();
                foreach (var v in vacancy)
                {

                    if ((DateTime.Now.Date - v.To.Date).TotalDays <= settingValue)
                    {
                        list.Add(v);
                    }
                }

                return list;
            }
            if (!vacancy.Any())
            {
                return list;
            }

            return vacancy;
        }
    }
}