using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeLeaveObjectDataSource : List<EmployeeLeave>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeLeave> GetEmployeeLeave(string branchId, string dept, DateTime from, DateTime to, string type, string status)
        {
            List<EmployeeLeave> EmployeeInfoList = new List<EmployeeLeave>();
            EmployeeLeave EmployeeInfo = new EmployeeLeave();
            List<Person_Employee> employees = db.Person_Employee.ToList(); ;

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                var placementEmpIds = db.Placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => placementEmpIds.Contains(x.EmployeeId)).ToList();
            }
            if (dept != null)
            {
                int.TryParse(dept, out int orgId);
                var placements = db.Placements.ToList();
                var placementEmpIds = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements,orgId).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => placementEmpIds.Contains(x.EmployeeId)).ToList();
            }

            if (status == "Active")
            {
                employees = employees.Where(x => x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                employees = employees.Where(x => !x.IsActive).ToList();
            }

            DateTime dateFrom = new DateTime(from.Year, from.Month, from.Day);
            DateTime dateTo = new DateTime(to.Year, to.Month, to.Day);

            var leaves = db.Leaves.ToList();
            if (type != null)
            {
                leaves = leaves.Where(x => x.LeaveType.ToString() == type).ToList();
            }

            db = new ExceedDbContext();

            if (leaves.Any())
            {
                foreach (var l in leaves)
                {

                    if ((l.From >= dateFrom && l.From <= dateTo))
                    {
                        db = new ExceedDbContext();

                        var emplist = employees.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);

                        if (emplist != null)
                        {
                            EmployeeInfo = new EmployeeLeave();
                            //  EmployeeInfo.Sex = emplist.Gender;
                            if (emplist.Gender == Gender.MALE)
                            {
                                EmployeeInfo.Sex = "ወንድ";
                            }
                            else
                            {
                                EmployeeInfo.Sex = "ሴት";
                            }
                            EmployeeInfo.FullName = emplist.FullNameAmharic;
                            EmployeeInfo.From = XAPI.EthiopicDateTime.GetEthiopicDate(l.From.Day, l.From.Month, l.From.Year);//l.From.ToShortDateString();
                            EmployeeInfo.To = XAPI.EthiopicDateTime.GetEthiopicDate(l.To.Day, l.To.Month, l.To.Year);//l.To.ToShortDateString();
                            EmployeeInfo.EmployeeId = emplist.EmployeeCode;
                            EmployeeInfo.LeaveType = l.LeaveType;

                            var fullhalf = l.Day;
                            if (fullhalf == LeaveDay.ሙሉ_ቀን)
                            {
                                EmployeeInfo.FullOrHalf = "ሙሉ ቀን ";
                            }
                            else if (fullhalf == LeaveDay.ግማሽ_ቀን)
                            {
                                if (l.MorningOrAfternoon == MorningOrAfternoon.ከሰዓት)
                                {
                                    EmployeeInfo.FullOrHalf = "ከሰአት ";
                                }
                                else if (l.MorningOrAfternoon == MorningOrAfternoon.ጥዋት)
                                {
                                    EmployeeInfo.FullOrHalf = "ጠዋት";
                                }
                            }

                            EmployeeInfo.AmountTaken = l.Amount;

                            var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == l.EmployeeId && !x.IsExpired).ToList();
                            if (entitle.Any())
                            {
                                EmployeeInfo.Balance = entitle.Sum(x => x.Balance);
                            }

                            bool IsCurrent = false;
                            int orgId = 0;

                            var placements = db.Placements.Where(x => x.EmployeeId == l.EmployeeId).ToList();

                            foreach (var place in placements)
                            {
                                if (place.isCurrent)
                                {
                                    IsCurrent = true;
                                    orgId = place.OrgStructureId;
                                    break;
                                }
                            }

                            if (!IsCurrent)
                            {
                                var place = placements.OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                                if (place != null)
                                {
                                    orgId = place.OrgStructureId;
                                }
                            }

                            var mainCompanyStructure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == orgId);

                            if (mainCompanyStructure != null)
                            {
                                EmployeeInfo.Departement = mainCompanyStructure.Name;
                            }

                            EmployeeInfoList.Add(EmployeeInfo);

                        }
                    }
                }
            }
            return EmployeeInfoList;
        }

    }
}