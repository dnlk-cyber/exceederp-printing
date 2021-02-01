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
            List<Person_Employee> employees;
            if (branchId != null && dept != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).ToList();
            }
            else if (branchId != null && dept == null)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && dept != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByWorkUnit(Convert.ToInt32(dept)).ToList();
            }
            else
            {
                employees = db.Person_Employee.ToList();
            }
            if (status == "Active")
            {
                employees = employees.Where(x => x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                employees = employees.Where(x => !x.IsActive).ToList();
            }

            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            if (type != null)
            {
                db = new ExceedDbContext();
                var leaves = db.Leaves.Where(x => x.LeaveType.ToString() == type).ToList();
                if (leaves.Any())
                {
                    foreach (var l in leaves)
                    {
                        DateTime leaveFrom = new DateTime(l.From.Year, l.From.Month, l.From.Day);
                        DateTime leaveTow = new DateTime(l.To.Year, l.To.Month, l.To.Day);
                        //if ((leaveFrom >= dateOne && leaveFrom <= dateTow) || (leaveTow >= dateOne && leaveTow <= dateTow))
                        //{
                        //if ((leaveFrom >= dateOne && leaveFrom <= dateTow) && (leaveTow >= dateOne && leaveTow <= dateTow) && (dateOne <= dateTow))
                        //{
                        if ((leaveFrom >= dateOne && leaveFrom <= dateTow))
                        {
                            db = new ExceedDbContext();


                            var placement =
                                db.Placements.FirstOrDefault(
                                        x => x.EmployeeId == l.EmployeeId);

                            if (placement != null)
                            {


                                //foreach (var e in placement)
                                //{
                                var emplist =
                                    employees.Where(x => x.EmployeeId == placement.EmployeeId)
                                        .FirstOrDefault();
                                if (emplist != null)
                                {

                                    if (status == null && emplist.IsActive)
                                    {
                                        placement = db.Placements.FirstOrDefault(x => x.EmployeeId == l.EmployeeId && x.isCurrent);
                                    }
                                    if (status == null && !emplist.IsActive)
                                    {
                                        var placements = db.Placements.Where(x => x.EmployeeId == l.EmployeeId).OrderByDescending(x => x.PlacementId).ToList();
                                        if (placements.Any())
                                        {
                                            placement = placements.FirstOrDefault();
                                        }

                                    }
                                    if (status == "Active")
                                    {
                                        placement = db.Placements.FirstOrDefault(x => x.EmployeeId == l.EmployeeId && x.isCurrent);
                                    }
                                    if (status == "InActive")
                                    {
                                        var placements = db.Placements.Where(x => x.EmployeeId == l.EmployeeId && x.isCurrent).ToList().OrderByDescending(P => P.PlacementId);
                                        if(placements.Any())
                                        {
                                            placement = placements.FirstOrDefault();
                                        }
                                    }


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
                                    EmployeeInfo.FullName = emplist.FirstName + " " + emplist.MiddleName + " " +
                                                            emplist.LastName;
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
                                    var entitle = db.LeavesEntitle.Where(x=>x.EmployeeId == l.EmployeeId && !x.IsExpired).ToList();
                                    if(entitle.Any())
                                    {
                                        EmployeeInfo.Balance = entitle.Sum(x=>x.Balance);
                                    }
                                    var deptt =
                                        db.MainCompanyStructures.Where(x => x.OrgStructureId == placement.OrgStructureId)
                                            .FirstOrDefault();
                                    EmployeeInfo.Departement = deptt.Name;
                                    EmployeeInfoList.Add(EmployeeInfo);
                                }
                                //}


                            }
                        }
                    }
                }
            }
            else
            {
                db = new ExceedDbContext();
                var leaves = db.Leaves.ToList();
                if (leaves.Any())
                {
                    foreach (var l in leaves)
                    {
                        DateTime leaveFrom = new DateTime(l.From.Year, l.From.Month, l.From.Day);
                        DateTime leaveTow = new DateTime(l.To.Year, l.To.Month, l.To.Day);
                        //if ((leaveFrom >= dateOne && leaveFrom <= dateTow) || (leaveTow >= dateOne && leaveTow <= dateTow))
                        //{
                        //if ((leaveFrom >= dateOne && leaveFrom <= dateTow) && (leaveTow >= dateOne && leaveTow <= dateTow) && (dateOne <= dateTow))
                        //{
                        if ((leaveFrom >= dateOne && leaveFrom <= dateTow))
                        {
                            db = new ExceedDbContext();


                            var placement =
                               db.Placements.FirstOrDefault(
                                       x => x.EmployeeId == l.EmployeeId);

                            if (placement != null)
                            {
                                //foreach (var e in placement)
                                //{
                                var emplist =
                                    employees.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
                                if (emplist != null)
                                {

                                    if (status == null && emplist.IsActive)
                                    {
                                        placement = db.Placements.FirstOrDefault(x => x.EmployeeId == l.EmployeeId && x.isCurrent);
                                    }
                                    if (status == null && !emplist.IsActive)
                                    {
                                        var placements = db.Placements.Where(x => x.EmployeeId == l.EmployeeId).OrderByDescending(x => x.PlacementId).ToList();
                                        if (placements.Any())
                                        {
                                            placement = placements.FirstOrDefault();
                                        }

                                    }
                                    if (status == "Active")
                                    {
                                        placement = db.Placements.FirstOrDefault(x => x.EmployeeId == l.EmployeeId && x.isCurrent);
                                    }
                                    if (status == "InActive")
                                    {
                                        var placements = db.Placements.Where(x => x.EmployeeId == l.EmployeeId && x.isCurrent).ToList().OrderByDescending(P => P.PlacementId);
                                        if (placements.Any())
                                        {
                                            placement = placements.FirstOrDefault();
                                        }
                                    }

                                    EmployeeInfo = new EmployeeLeave();
                                    // EmployeeInfo.Sex = emplist.Gender;
                                    if (emplist.Gender == Gender.MALE)
                                    {
                                        EmployeeInfo.Sex = "ወንድ";
                                    }
                                    else
                                    {
                                        EmployeeInfo.Sex = "ሴት";
                                    }
                                    EmployeeInfo.FullName = emplist.FirstName + " " + emplist.MiddleName + " " +
                                                            emplist.LastName;
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
                                    var deptt =
                                        db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                                    if (deptt != null) EmployeeInfo.Departement = deptt.Name;
                                    EmployeeInfoList.Add(EmployeeInfo);
                                }
                                //}


                            }
                        }
                    }
                }
            }
            return EmployeeInfoList;
        }
    }
}