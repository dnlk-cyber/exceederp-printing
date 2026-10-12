//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using ExceedERP.Core.Domain.Common;
//using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
//using ExceedERP.DataAccess.Context;

//namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
//{
//    [DataObject]
//    public class InternalVacancyObjectDataSource
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        [DataObjectMethod(DataObjectMethodType.Select)]
//        public List<Vacancy> GetInternalVacancy()
//        {
//            List<Vacancy> list = new List<Vacancy>();
//            var vacancy = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.INTERNAL
//                                                       || x.RecruSource == RecruitmentSource.INTERNALPROMOTION
//                                                       || x.RecruSource == RecruitmentSource.INTERNALTRANSFER).ToList();

//            var config =
//                db.Configurations.Where(x => x.Category == "Promotion" && x.Reference == "VACANCY_LIFE_TIME_INDAY")
//                    .FirstOrDefault();
//            if (config != null)
//            {
//                int settingValue = Convert.ToInt32(config.CurrentValue);
//                list = new List<Vacancy>();
//                foreach (var v in vacancy)
//                {

//                    if ((DateTime.Now.Date - v.To.Date).TotalDays <= settingValue)
//                    {
//                        list.Add(v);
//                    }
//                }

//                return list;
//            }
//            if (!vacancy.Any())
//            {
//                return list;
//            }

//            return vacancy;
//        }
//        [DataObjectMethod(DataObjectMethodType.Select)]
//        public List<Vacancy> GetInternalVacancyCode()
//        {
//            var vacancy = db.VaccancyPlans
//                         .Where(x => x.RecruSource == RecruitmentSource.INTERNAL
//                                || x.RecruSource == RecruitmentSource.INTERNALPROMOTION
//                                || x.RecruSource == RecruitmentSource.INTERNALTRANSFER)
//                               .ToList();


//            return vacancy;
//        }
//    }
//}