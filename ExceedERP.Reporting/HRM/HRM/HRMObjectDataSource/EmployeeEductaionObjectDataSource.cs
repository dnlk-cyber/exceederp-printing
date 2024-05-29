using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEductaionObjectDataSource : List<EmployeeEductaion>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeEductaion> GetEmployeeTraining(string eduLevel, string eduField, string dept, DateTime from, DateTime to, string branchId, string id)
        {
            List<EmployeeEductaion> EmployeeInfoList = new List<EmployeeEductaion>();
            EmployeeEductaion EmployeeInfo = new EmployeeEductaion();
            db = new ExceedDbContext();
            var branches = db.Branch.ToList();
            db = new ExceedDbContext();
            var jobs = db.Lookups.ToList();
            db = new ExceedDbContext();
            var providers = db.Companies.Where(x => x.Category == "1").ToList();
            db = new ExceedDbContext();
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);

            var placements = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (dept != null)
            {
                int.TryParse(dept, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }
            if (id != null)
            {
                int.TryParse(id, out int empId);
                placements = placements.Where(x => x.EmployeeId == empId).ToList();
            }

            #region new code - get report data

            placements = placements.Where(x => x.isCurrent).ToList();
            foreach (var pl in placements)
            {
                var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                if (e != null)
                {
                    EmployeeInfo = new EmployeeEductaion();

                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                    if (e.Gender == 0)
                    {
                        EmployeeInfo.Sex = "ወንድ";
                    }
                    else
                    {
                        EmployeeInfo.Sex = "ሴት";
                    }

                    EmployeeInfo.FullName = e.FullNameAmharic;
                }

                var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);

                if (deptt != null)
                {
                    EmployeeInfo.Departement = deptt.Name;
                }

                var empeducation = db.Education.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                if (eduField != null)
                {
                    int.TryParse(eduField, out int educationField);
                    empeducation = empeducation.Where(x => x.EduField == educationField).ToList();
                }
                if (eduLevel != null)
                {
                    int.TryParse(eduLevel, out int educationLevel);
                    empeducation = empeducation.Where(x => x.EduLevel == educationLevel).ToList();
                }
                foreach (var edu in empeducation)
                {
                    var empedu = new EmployeeEductaionList();
                    var level = jobs.Where(x => x.LookupId == edu.EduLevel).FirstOrDefault();
                    if (level != null)
                    {
                        empedu.EduLevel = level.Description;
                    }
                    var field = jobs.Where(x => x.LookupId == edu.EduField).FirstOrDefault();
                    if (field != null)
                    {
                        empedu.EduField = field.Description;
                    }

                    if (edu.From != null)
                    {
                        DateTime eduform = new DateTime(edu.From.Value.Year, edu.From.Value.Month, edu.From.Value.Day);

                        var dateFrom = XAPI.EthiopicDateTime.GetEthiopicDate(eduform.Day, eduform.Month, eduform.Year);
                        empedu.From = dateFrom;
                    }
                    DateTime eduto = new DateTime(edu.To.Year, edu.To.Month, edu.To.Day);
                    var dateTo = XAPI.EthiopicDateTime.GetEthiopicDate(eduto.Day, eduto.Month, eduto.Year);
                    empedu.To = dateTo;// eduto.ToShortDateString();

                    EmployeeInfo.EmployeeEductaionList.Add(empedu);
                }
                if(EmployeeInfo.EmployeeEductaionList.Any())
                {
                    EmployeeInfoList.Add(EmployeeInfo);
                }
              
            }
            #endregion

            foreach (var emp in EmployeeInfoList)
            {
                if (emp.EmployeeEductaionList.Count > 0)
                    emp.Assigned = true;
                else
                    emp.Assigned = false;
            }

            return EmployeeInfoList;
        }
    }
}