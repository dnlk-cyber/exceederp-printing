using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEductaionObjectDataSource : List<EmployeeEductaion>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeEductaion> GetEmployeeTraining(string eduLevel, string eduField, string dept, DateTime? from, DateTime? to, string branchId,string bussinessUnitId, string id)
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
            DateTime dateTow = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day);
            DateTime dateOne = new DateTime(from.Value.Year, from.Value.Month, from.Value.Day);

            var placements = db.Placements.Where(x=>x.isCurrent).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (bussinessUnitId != null)
            {
                int.TryParse(bussinessUnitId, out int bussinesUnitId);
                placements = placements.Where(x=>x.BusinessUnitId==bussinesUnitId).ToList();
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
            var employeeEducations = db.Education.Where(x => x.From >= from.Value);

            if (from.HasValue&& employeeEducations.Any())
            {
                 employeeEducations = employeeEducations.Where(x => x.From >= from.Value);
                
            }
            if (to.HasValue && employeeEducations.Any())
            {
                employeeEducations = employeeEducations.Where(x => x.To <= to.Value);

            }
            if(employeeEducations.Any())
            {
                var employeeIds = employeeEducations.Select(x => x.EmployeeId);
                if (employeeIds.Any())
                {
                    placements = placements.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();

                }
            }
            #region new code - get report data

            placements = placements.Where(x => x.isCurrent).ToList();
            foreach (var pl in placements)
            {
                var e = db.Person_Employee.Where(x=>x.IsActive).FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                if (e != null)
                {
                    EmployeeInfo = new EmployeeEductaion();

                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                    EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);

                    EmployeeInfo.FullName = e.FullNameAmharic;


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
                    if (from.HasValue && empeducation.Any())
                    {
                        empeducation = empeducation.Where(x => x.From >= from.Value).ToList();

                    }
                    if (to.HasValue && empeducation.Any())
                    {
                        empeducation = empeducation.Where(x => x.To <= to.Value).ToList();

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
                    //if(EmployeeInfo.EmployeeEductaionList.Any())
                    //{
                    //    EmployeeInfoList.Add(EmployeeInfo);
                    //}

                    EmployeeInfoList.Add(EmployeeInfo);
                }
               // return EmployeeInfoList;
            }
            return EmployeeInfoList;
            #endregion

            //foreach (var emp in EmployeeInfoList)
            //{
            //    if (emp.EmployeeEductaionList.Count > 0)
            //        emp.Assigned = true;
            //    else
            //        emp.Assigned = false;
            //}

            // return EmployeeInfoList;

        }
    }
}