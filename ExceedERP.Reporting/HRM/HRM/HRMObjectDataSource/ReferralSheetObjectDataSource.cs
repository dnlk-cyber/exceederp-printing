using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    public class ReferralSheetObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReferralSheet> GetReferralSheet(string hospital, DateTime from, DateTime to)
        {
            List<ReferralSheet> ReferralSheetList = new List<ReferralSheet>();
            ReferralSheet ReferralSheet = new ReferralSheet();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            var placemts = db.Placements.Where(x => x.isCurrent);
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            var referedPatints = db.ReferralSheet.ToList();
            if (hospital != null)
            {
                referedPatints = db.ReferralSheet.Where(x => x.MainCompanyId.ToString() == hospital).ToList();
            }
            var patints = db.PatientCard.ToList();

            foreach (var p in referedPatints)
            {
                DateTime referDate = new DateTime(p.Date.Year, p.Date.Month, p.Date.Day);
                if (referDate >= dateOne && referDate <= dateTow)
                {
                    ReferralSheet = new ReferralSheet();
                    if (patints.Any())
                    {
                        var onePatinet = patints.Where(x => x.PatientId == p.PatientId).FirstOrDefault();
                        if (onePatinet != null)
                        {
                            var employee = employees.Where(x => x.EmployeeId == onePatinet.EmployeeId);
                            ReferralSheet.CardNo = onePatinet.PatientCardId;
                            ReferralSheet.Age = onePatinet.Age;
                            ReferralSheet.Sex = onePatinet.Sex;

                            ReferralSheet.Diagnosis = p.Diagnosis;
                            ReferralSheet.Doctor = p.RecommendedBy;
                            ReferralSheet.Hospital = p.MainCompany.TradeName;
                            ReferralSheet.FullName = onePatinet.Employee.FirstName + " " + onePatinet.Employee.MiddleName + " " + onePatinet.Employee.LastName;
                            var dept = placemts.Where(x => x.EmployeeId == onePatinet.EmployeeId).FirstOrDefault();
                            if (dept != null)
                            {
                                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == dept.OrgStructureId);
                                if(mainCompany != null)
                                {
                                    ReferralSheet.Departement = mainCompany.Name;
                                }                               
                            }
                            ReferralSheet.Date = p.DateAmharic;
                            ReferralSheetList.Add(ReferralSheet);
                        }
                    }
                }

            }

            return ReferralSheetList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}