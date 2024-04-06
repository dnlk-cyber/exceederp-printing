using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveRequestStatusObjectDataSource : List<LeaveRequestStatusVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveRequestStatusVM> GetLeaveRequestStatus(string empId, string leaveType, string periodId, string branchId, string orgStructureId, string status, DateTime? from, DateTime? to)
        {
            List<LeaveRequestStatusVM> list = new List<LeaveRequestStatusVM>();
           // var model = new LeaveRequestStatusVM();

            var leaveLists = db.LeaveRequest.ToList();
            if(empId != null)
            {
                int.TryParse(empId, out int employeeId);
                leaveLists = leaveLists.Where(x => x.EmployeeId== employeeId).ToList();

            }
            if (leaveType != null)
            {
                leaveLists = leaveLists.Where(x => x.Type.ToString() == leaveType).ToList();
            }
            if (periodId != null)
            {
                int.TryParse(periodId, out int glfisicalYearId);
                leaveLists = leaveLists.Where(x => x.GlFiscalYearId == glfisicalYearId).ToList();
            }

          //  leaveLists = db.Leaves.DistinctByField(x => x.EmployeeId).ToList();
            var placements = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
                var employeIds = placements.Select(x => x.EmployeeId).ToList();
                leaveLists = leaveLists.Where(x => employeIds.Contains(x.EmployeeId)).ToList();

            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
                var employeIds = placements.Select(x => x.EmployeeId).ToList();
                leaveLists = leaveLists.Where(x => employeIds.Contains(x.EmployeeId)).ToList();

            }
            if(status != null)
            {
                var requstIds = db.RequestesStatus.Where(x => x.Status.ToString() == status).Select(x => x.RequestId);
                leaveLists = leaveLists.Where(x => requstIds.Contains(x.LeaveRequestId)).ToList();

            }
            if (from.HasValue)
            {
                leaveLists = leaveLists.Where(x => x.From.Date >= from.Value.Date).ToList();
            }
            else if ( to.HasValue)
            {
                leaveLists = leaveLists.Where(x => x.To.Date <= to.Value.Date).ToList();
            }
          
            var listItems = leaveLists.DistinctByField(x => x.EmployeeId).ToList();
            foreach (var listItem in listItems)
            {
                var item = new LeaveRequestStatusVM();
                var person = db.Person_Employee.Find(listItem.EmployeeId);
                if(person != null)
                {
                    item.ApplicantName = person.FullNameaEnglish;
                }
                var place = db.Placements.FirstOrDefault(x => x.EmployeeId == listItem.EmployeeId);
                if (place != null)
                {

                    var department = db.MainCompanyStructures.Find(place.OrgStructureId);
                    if (department != null)
                    {
                        item.Department = department.Name;
                    }
                }
                item.LeaveRequestStatus = new List<RequestesStatusVM>();
                var leaves = leaveLists.Where(x => x.EmployeeId ==listItem.EmployeeId).ToList();
                foreach(var leaveList in leaves)
                {
                    var requestList = new RequestesStatusVM();
                    requestList.Period = leaveList.GlFiscalYear.Name;
                    requestList.Type = leaveList.Type.ToString();
                    requestList.ActionType = leaveList.LeaveActionTypes.ToString();
                    requestList.Day = leaveList.Day.ToString();
                    requestList.From = leaveList.FromAmharic;
                    requestList.To = leaveList.ToAmharic;
                    requestList.RequestAmount = leaveList.LeaveBalance;
                    requestList.LeaveDays = leaveList.LeaveDays;
                    requestList.RequestStatusDetail = new List<RequestesStatusDetailVM>();
                    
                    var requestesStatus = db.RequestesStatus.Where(x => x.RequestId == leaveList.LeaveRequestId).ToList();
                    if(status != null)
                    {
                        requestesStatus = requestesStatus.Where(x => x.Status.ToString() == status).ToList();
                    }

                    foreach(var statusDetail in requestesStatus)
                    {
                        var detail = new RequestesStatusDetailVM();
                        detail.Date = statusDetail.DateReqStatAm;
                        var approver = db.Person_Employee.Find(statusDetail.ApproverId);
                        if(approver != null)
                        {
                            detail.AproverName = approver.FullNameaEnglish;

                        }
                        var placem = db.Placements.FirstOrDefault(x => x.EmployeeId == statusDetail.ApproverId);
                        if (placem != null)
                        {

                            var department = db.MainCompanyStructures.Find(placem.OrgStructureId);
                            if (department != null)
                            {
                                detail.ApproverDepartmentName = department.Name;
                            }
                        }
                        detail.Status = statusDetail.Status.ToString();
                        requestList.RequestStatusDetail.Add(detail);
                    }

                    item.LeaveRequestStatus.Add(requestList);
                  
                }
                list.Add(item);

            }

           

            return list;
        }

    }

    [DataObject]
    public class LeaveTypeODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetLeaveType()
        {
            List<string> list = new List<string>();
            list.Add("ANNUAL");
            list.Add("OTHER");
            list.Add("MARRIAGE");
            list.Add("MATERNITY");
            list.Add("PATERNITY");
            list.Add("SICK_LEAVE");
            return list;
        }

    }
}