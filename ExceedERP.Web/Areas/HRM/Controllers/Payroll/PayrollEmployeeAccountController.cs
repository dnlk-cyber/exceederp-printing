using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class PayrollEmployeeAccountController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        public static bool CheckStatus { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {

            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name).Where(x => x.IsActive).ToList();
            ViewData["EmployeeList"] = list;

            var costCenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == Core.Domain.Finance.GL.Dynamic.DynamicSegmentTypes.CostCenter);
            var costcenters = db.GlChartOfAccountSegments.Where(x=>x.GLSegmentSetupId== costCenterSeg.GLSegmentSetupId);
            ViewData["costCenter"] = costcenters;

            var subAccSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == Core.Domain.Finance.GL.Dynamic.DynamicSegmentTypes.SubAccount);
            var subAccCodes = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccSeg.GLSegmentSetupId);
            ViewData["SubAccCode"] = subAccCodes;

            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == Core.Domain.Finance.GL.Dynamic.DynamicSegmentTypes.Location);
            var loactionCodes = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            ViewData["LocationCode"] = loactionCodes;
            // PrepareEmployeeAccount();
            return View();
        }

        public int FileterEmployee(int id)
        {

            var employeeCostCenter = db.PayrollEmployeeAccounts.Where(x => x.EmployeeId == id).FirstOrDefault();
            CheckStatus = false;
            if (employeeCostCenter == null)
            {
                CheckStatus = true;
                return 1;

            }

            return 0;
        }
        public ActionResult PayrollEmployeeAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<PayrollEmployeeAccount> payrollemployeeaccounts = db.PayrollEmployeeAccounts.Where(x => employees.Contains(x.EmployeeId)); ;
            DataSourceResult result = payrollemployeeaccounts.ToDataSourceResult(request, payrollEmployeeAccount => new
            {
                PayrollEmployeeAccountID = payrollEmployeeAccount.PayrollEmployeeAccountId,
                DateCreated = payrollEmployeeAccount.DateCreated,
                CreatedBy = payrollEmployeeAccount.CreatedBy,
                EmployeeId = payrollEmployeeAccount.EmployeeId,
                CostCenterAccountCode = payrollEmployeeAccount.CostCenterAccountCode,
                LocationAccCode = payrollEmployeeAccount.LocationAccCode,
                SubAccCode = payrollEmployeeAccount.SubAccCode,
                //StaffDebtorAccountCode = payrollEmployeeAccount.StaffDebtorAccountCode,
                //StaffAdvanceAccountCode = payrollEmployeeAccount.StaffAdvanceAccountCode,
                //StaffLoanAccountCode = payrollEmployeeAccount.StaffLoanAccountCode,
                Remark = payrollEmployeeAccount.Remark,
                ModifiedBy = payrollEmployeeAccount.ModifiedBy,
                LastModified = payrollEmployeeAccount.LastModified,
                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                //isSendForApproval = payrollEmployeeAccount.isSendForApproval,
                //isOnlineApproved = payrollEmployeeAccount.isOnlineApproved,
                //OnlineApprovedBy = payrollEmployeeAccount.OnlineApprovedBy,
                //OnlineApprovedTime = payrollEmployeeAccount.OnlineApprovedTime,
                //isVoid = payrollEmployeeAccount.isVoid,
                //VoidBy = payrollEmployeeAccount.VoidBy,
                //VoidTime = payrollEmployeeAccount.VoidTime
            });

            return Json(result);
        }

        private string GetCostCenterDescription(string code)
        {
            var costCenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var costCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costCenterSeg.GLSegmentSetupId).AsQueryable();
            var costCenterDesc = costCenters.Where(x => x.Values == code).FirstOrDefault();
            if (costCenterDesc != null)
            {

                return costCenterDesc.Description;
            }
            return "";
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollEmployeeAccounts_Create([DataSourceRequest]DataSourceRequest request, PayrollEmployeeAccount payrollEmployeeAccount)
        {
            var checkexistance = db.PayrollEmployeeAccounts.FirstOrDefault(x => x.EmployeeId == payrollEmployeeAccount.EmployeeId);
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "The Employee Cost center is already registered!!");
            }
            else if (BranchCode(payrollEmployeeAccount.EmployeeId) == null)
            {
                ModelState.AddModelError("", "There is no assigned cost center value for this employee placement!!");
            }


            if (ModelState.IsValid)
            {

                var entity = new PayrollEmployeeAccount
                {
                    DateCreated = DateTime.Now,
                    CreatedBy = payrollEmployeeAccount.CreatedBy,
                    EmployeeId = payrollEmployeeAccount.EmployeeId,
                    CostCenterAccountCode = BranchCode(payrollEmployeeAccount.EmployeeId),
                    LocationAccCode = payrollEmployeeAccount.LocationAccCode,
                    //StaffDebtorAccountCode = payrollEmployeeAccount.StaffDebtorAccountCode,
                    //StaffAdvanceAccountCode = payrollEmployeeAccount.StaffAdvanceAccountCode,
                    //StaffLoanAccountCode = payrollEmployeeAccount.StaffLoanAccountCode,
                    //OrgBranchName = payrollEmployeeAccount.OrgBranchName,
                    //Status = payrollEmployeeAccount.Status,
                    //isSendForApproval = payrollEmployeeAccount.isSendForApproval,
                    //isOnlineApproved = payrollEmployeeAccount.isOnlineApproved,
                    //OnlineApprovedBy = payrollEmployeeAccount.OnlineApprovedBy,
                    //OnlineApprovedTime = payrollEmployeeAccount.OnlineApprovedTime,
                    //isVoid = payrollEmployeeAccount.isVoid,
                    SubAccCode = payrollEmployeeAccount.SubAccCode,
                    Remark = payrollEmployeeAccount.Remark,
                    ModifiedBy = payrollEmployeeAccount.ModifiedBy,
                    LastModified = payrollEmployeeAccount.LastModified,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.PayrollEmployeeAccounts.Add(entity);
                db.SaveChanges();
                payrollEmployeeAccount.PayrollEmployeeAccountId = entity.PayrollEmployeeAccountId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll employee account", "Success", User.Identity.Name, $"added employee account for employee {entity.EmployeeId} location {entity.LocationAccCode} sub account {entity.SubAccCode}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { payrollEmployeeAccount }.ToDataSourceResult(request, ModelState));
        }
        private string BranchCode(int id)
        {
            var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placment != null)
            {
                var costcenter = db.BranchCostCenter.FirstOrDefault(x => x.BranchId == placment.BranchId && x.OrgStructureId == placment.OrgStructureId);
                if (costcenter != null)
                {
                    return costcenter.Values;
                }
            }
            return null;
        }
        private void PrepareEmployeeAccount()
        {

            var list = inputs.GetEmployees(User.Identity.Name).Where(x => x.IsActive).ToList();
            ExceedDbContext dbAc = new ExceedDbContext();
            foreach (var e in list)
            {
                string costCenter = BranchCode(e.PersonId);
                if (costCenter != null)
                {
                    var payrollEmployeeAccount = dbAc.PayrollEmployeeAccounts.FirstOrDefault(x => x.EmployeeId == e.PersonId);
                    if (payrollEmployeeAccount != null)
                    {
                        if (payrollEmployeeAccount.CostCenterAccountCode != costCenter)
                        {
                            payrollEmployeeAccount.EmployeeId = e.PersonId;
                            payrollEmployeeAccount.CostCenterAccountCode = costCenter;
                            payrollEmployeeAccount.LocationAccCode = payrollEmployeeAccount.LocationAccCode;
                            payrollEmployeeAccount.ModifiedBy = User.Identity.Name;
                            payrollEmployeeAccount.LastModified = DateTime.Now;
                            payrollEmployeeAccount.OrgBranchName =UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            dbAc.Entry(payrollEmployeeAccount).State = EntityState.Modified;
                            dbAc.SaveChanges();
                        }
                    }

                    else
                    {
                        var entityy = new PayrollEmployeeAccount
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            EmployeeId = e.PersonId,
                            CostCenterAccountCode = costCenter,
                            LocationAccCode = null,
                            SubAccCode = null,
                            OrgBranchName =UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            Remark = null
                        };

                        db.PayrollEmployeeAccounts.Add(entityy);
                        db.SaveChanges();
                        //payrollEmployeeAccount.PayrollEmployeeAccountId = entityy.PayrollEmployeeAccountId;
                    }
                }
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollEmployeeAccounts_Update([DataSourceRequest]DataSourceRequest request, PayrollEmployeeAccount payrollEmployeeAccount)
        {
          
            if (ModelState.IsValid)
            {

                var entity = new PayrollEmployeeAccount
                {
                    PayrollEmployeeAccountId = payrollEmployeeAccount.PayrollEmployeeAccountId,
                    DateCreated = payrollEmployeeAccount.DateCreated,
                    CreatedBy = payrollEmployeeAccount.CreatedBy,
                    EmployeeId = payrollEmployeeAccount.EmployeeId,
                    CostCenterAccountCode = payrollEmployeeAccount.CostCenterAccountCode,
                    LocationAccCode = payrollEmployeeAccount.LocationAccCode,
                    SubAccCode = payrollEmployeeAccount.SubAccCode,
                    //StaffDebtorAccountCode = payrollEmployeeAccount.StaffDebtorAccountCode,
                    //StaffAdvanceAccountCode = payrollEmployeeAccount.StaffAdvanceAccountCode,
                    //StaffLoanAccountCode = payrollEmployeeAccount.StaffLoanAccountCode,
                    //OrgBranchName = payrollEmployeeAccount.OrgBranchName,
                    //Status = payrollEmployeeAccount.Status,
                    //isSendForApproval = payrollEmployeeAccount.isSendForApproval,
                    //isOnlineApproved = payrollEmployeeAccount.isOnlineApproved,
                    //OnlineApprovedBy = payrollEmployeeAccount.OnlineApprovedBy,
                    //OnlineApprovedTime = payrollEmployeeAccount.OnlineApprovedTime,
                    //isVoid = payrollEmployeeAccount.isVoid,
                    //VoidBy = payrollEmployeeAccount.VoidBy,
                    Remark = payrollEmployeeAccount.Remark,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.PayrollEmployeeAccounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll employee account", "Success", User.Identity.Name, $"updated employee account for employee {entity.EmployeeId} location {entity.LocationAccCode} sub account {entity.SubAccCode}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { payrollEmployeeAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollEmployeeAccounts_Destroy([DataSourceRequest]DataSourceRequest request, PayrollEmployeeAccount payrollEmployeeAccount)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PayrollEmployeeAccount
                {
                    PayrollEmployeeAccountId = payrollEmployeeAccount.PayrollEmployeeAccountId,
                    DateCreated = payrollEmployeeAccount.DateCreated,
                    CreatedBy = payrollEmployeeAccount.CreatedBy,
                    EmployeeId = payrollEmployeeAccount.EmployeeId,
                    CostCenterAccountCode = payrollEmployeeAccount.CostCenterAccountCode,
                    LocationAccCode = payrollEmployeeAccount.LocationAccCode,
                    SubAccCode = payrollEmployeeAccount.SubAccCode,
                    //StaffDebtorAccountCode = payrollEmployeeAccount.StaffDebtorAccountCode,
                    //StaffAdvanceAccountCode = payrollEmployeeAccount.StaffAdvanceAccountCode,
                    //StaffLoanAccountCode = payrollEmployeeAccount.StaffLoanAccountCode,
                    //OrgBranchName = payrollEmployeeAccount.OrgBranchName,
                    //Status = payrollEmployeeAccount.Status,
                    //isSendForApproval = payrollEmployeeAccount.isSendForApproval,
                    //isOnlineApproved = payrollEmployeeAccount.isOnlineApproved,
                    //OnlineApprovedBy = payrollEmployeeAccount.OnlineApprovedBy,
                    //OnlineApprovedTime = payrollEmployeeAccount.OnlineApprovedTime,
                    //isVoid = payrollEmployeeAccount.isVoid,
                    //VoidBy = payrollEmployeeAccount.VoidBy,
                    Remark = payrollEmployeeAccount.Remark,
                    ModifiedBy = payrollEmployeeAccount.ModifiedBy,
                    LastModified = payrollEmployeeAccount.LastModified,
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee payroll accounts information of account " + payrollEmployeeAccount.EmployeeId,Modules.Payroll );
                #endregion
                db.PayrollEmployeeAccounts.Attach(entity);
                db.PayrollEmployeeAccounts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { payrollEmployeeAccount }.ToDataSourceResult(request, ModelState));
        }

       
        public JsonResult Refresh()
        {
            var placedEmployees = db.Placements.Where(x => x.isCurrent).ToList();
            foreach (var placement in placedEmployees)
            {
                var costcenter = db.BranchCostCenter
                    .Where(x => x.BranchId == placement.BranchId)
                    .FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);

                var location = db.Branch
                    .Where(x => x.BranchId == placement.BranchId).FirstOrDefault();

                var payrollEmployeeAccount = db.PayrollEmployeeAccounts.AsNoTracking()
                    .FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);

                if (payrollEmployeeAccount != null)
                {
                    #region update
                    var entity = new PayrollEmployeeAccount
                    {
                        PayrollEmployeeAccountId = payrollEmployeeAccount.PayrollEmployeeAccountId,
                        EmployeeId = payrollEmployeeAccount.EmployeeId,
                        CostCenterAccountCode = costcenter.Values ?? "NA",
                        LocationAccCode = location.LocationAccount ?? "NA",
                        SubAccCode = payrollEmployeeAccount.SubAccCode,

                        Remark = payrollEmployeeAccount.Remark,
                        DateCreated = payrollEmployeeAccount.DateCreated,
                        CreatedBy = payrollEmployeeAccount.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,
                        OrgBranchName = placement.BranchId.ToString()
                    };

                    db.PayrollEmployeeAccounts.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #endregion
                }
                else
                {
                    #region create empty

                    var entity = new PayrollEmployeeAccount
                    {

                        EmployeeId = placement.EmployeeId,
                        CostCenterAccountCode = "NA",
                        LocationAccCode = "NA",
                        SubAccCode = "NA",
                        Remark = "",
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        OrgBranchName = placement.BranchId.ToString()
                    };

                    db.PayrollEmployeeAccounts.Add(entity);
                    db.SaveChanges();

                    #endregion

                }
            }
            var formattedData = new
            {
                Message = "Update completed.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
