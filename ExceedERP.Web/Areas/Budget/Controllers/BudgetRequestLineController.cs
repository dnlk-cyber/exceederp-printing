using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Web.Areas.Budget.Models;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    public class BudgetRequestLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult BudgetRequestLines_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                Description = budgetRequestLine.Description,
                AccountCode = budgetRequestLine.AccountCode,
                Accepted = budgetRequestLine.Accepted,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult BudgetRequestLinesOther_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id && x.BudgetRequestType == BudgetRequestType.Other).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                Description = budgetRequestLine.Description,
                Accepted = budgetRequestLine.Accepted,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                AccountCode = budgetRequestLine.AccountCode,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult BudgetRequestLinesSubContract_Aggrement_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id && x.BudgetRequestType == BudgetRequestType.SubContract_Aggrement).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                Description = budgetRequestLine.Description,
                Accepted = budgetRequestLine.Accepted,
                AccountCode = budgetRequestLine.AccountCode,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult BudgetRequestLinesStaff_Monthly_Salary_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id && x.BudgetRequestType == BudgetRequestType.Staff_Monthly_Salary).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                Description = budgetRequestLine.Description,
                AccountCode = budgetRequestLine.AccountCode,
                Accepted = budgetRequestLine.Accepted,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult BudgetRequestLinesSkilled_Daily_Labour_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id && x.BudgetRequestType == BudgetRequestType.Skilled_Daily_Labour).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                Description = budgetRequestLine.Description,
                Accepted = budgetRequestLine.Accepted,
                AccountCode = budgetRequestLine.AccountCode,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult BudgetRequestLinesGas_Oil_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id && x.BudgetRequestType == BudgetRequestType.Gas_Oil).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                Description = budgetRequestLine.Description,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                Accepted = budgetRequestLine.Accepted,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                AccountCode = budgetRequestLine.AccountCode,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult BudgetRequestLinesEquipment_Purchase_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id && (x.BudgetRequestType == BudgetRequestType.Equipment_Purchase || x.BudgetRequestType == BudgetRequestType.Material_Purchase)).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                Description = budgetRequestLine.Description,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                Accepted = budgetRequestLine.Accepted,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                AccountCode = budgetRequestLine.AccountCode,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult BudgetRequestLinesCar_Rent_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestLine> budgetrequestlines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id && x.BudgetRequestType == BudgetRequestType.Equipment_Rent).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestLine => new
            {
                BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                BudgetRequestType = budgetRequestLine.BudgetRequestType,
                Accepted = budgetRequestLine.Accepted,
                BudgetRequestId = budgetRequestLine.BudgetRequestId,
                Description = budgetRequestLine.Description,
                PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                RentTotalHr = budgetRequestLine.RentTotalHr,
                RentPerHour = budgetRequestLine.RentPerHour,
                DailySalary = budgetRequestLine.DailySalary,
                NoWorkingDay = budgetRequestLine.NoWorkingDay,
                SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                OverTime = budgetRequestLine.OverTime,
                Unit = budgetRequestLine.Unit,
                FromMonth = budgetRequestLine.FromMonth,
                ToMonth = budgetRequestLine.ToMonth,
                AccountCode = budgetRequestLine.AccountCode,


                Amount = budgetRequestLine.Amount,
                Reason = budgetRequestLine.Reason,
                Status = budgetRequestLine.Status,
                Remark = budgetRequestLine.Remark,
                IsVoid = budgetRequestLine.IsVoid,
                VoidBy = budgetRequestLine.VoidBy,
                VoidTime = budgetRequestLine.VoidTime,
                IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                IsSendForApproval = budgetRequestLine.IsSendForApproval,
                OrgBranchName = budgetRequestLine.OrgBranchName,
                SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                DateCreated = budgetRequestLine.DateCreated,
                LastModified = budgetRequestLine.LastModified,
                CreatedBy = budgetRequestLine.CreatedBy,
                ModifiedBy = budgetRequestLine.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequest_BatchUpdate(BudgetRequestLineList model)
        {
            BudgetRequestHelper BudgetRequestHelper = new BudgetRequestHelper();
            foreach (var budgetRequestLine in model.BudgetRequestLine)
            {
                var RequestLine = db.BudgetRequestLines.Where(x => x.BudgetRequestLineId == budgetRequestLine.BudgetRequestLineId).FirstOrDefault();

                if (RequestLine != null)
                {
                    RequestLine.Accepted = true;
                    RequestLine.AccountCode = budgetRequestLine.AccountCode;

                    if (RequestLine.BudgetRequestType == BudgetRequestType.Equipment_Purchase || RequestLine.BudgetRequestType == BudgetRequestType.Material_Purchase)
                    {
                        RequestLine.PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit;
                        RequestLine.PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice;
                    }
                    if (RequestLine.BudgetRequestType == BudgetRequestType.Equipment_Rent )
                    {
                        RequestLine.RentPerHour = budgetRequestLine.RentPerHour;
                        RequestLine.RentTotalHr = budgetRequestLine.RentTotalHr;
                    }
                    if (RequestLine.BudgetRequestType == BudgetRequestType.Gas_Oil)
                    {
                        RequestLine.Unit = budgetRequestLine.Unit;
                        RequestLine.Amount = budgetRequestLine.Amount;
                    }
                    if (RequestLine.BudgetRequestType == BudgetRequestType.Material_Purchase)
                    {
                        RequestLine.PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit;
                        RequestLine.PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice;

                    }
                    if (RequestLine.BudgetRequestType == BudgetRequestType.Other_Expense)
                    {
                        RequestLine.PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit;
                        RequestLine.PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice;
                    }
                    db.BudgetRequestLines.Attach(RequestLine);
                    db.Entry(RequestLine).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(model.BudgetRequestLine.Count + " Request Accepted", JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestLines_Create([DataSourceRequest]DataSourceRequest request, BudgetRequestLine budgetRequestLine, int Id)
        {
            if (ModelState.IsValid)
            {
                BudgetRequestHelper BudgetRequestHelper = new BudgetRequestHelper();

                var entity = new BudgetRequestLine
                {
                    BudgetRequestId = Id,
                    BudgetRequestType = budgetRequestLine.BudgetRequestType,
                    Description = budgetRequestLine.Description,
                    AccountCode = budgetRequestLine.AccountCode,
                    PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                    PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                    RentTotalHr = budgetRequestLine.RentTotalHr,
                    RentPerHour = budgetRequestLine.RentPerHour,
                    DailySalary = budgetRequestLine.DailySalary,
                    NoWorkingDay = budgetRequestLine.NoWorkingDay,
                    SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                    ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                    DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                    ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                    OverTime = budgetRequestLine.OverTime,
                    Unit = budgetRequestLine.Unit,
                    FromMonth = budgetRequestLine.FromMonth,
                    ToMonth = budgetRequestLine.ToMonth,
                    Amount = budgetRequestLine.Amount,
                    Reason = budgetRequestLine.Reason,
                    Status = budgetRequestLine.Status,
                    Remark = budgetRequestLine.Remark,
                    IsVoid = budgetRequestLine.IsVoid,
                    VoidBy = budgetRequestLine.VoidBy,
                    VoidTime = budgetRequestLine.VoidTime,
                    IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                    OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                    OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                    IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                    IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                    OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                    IsSendForApproval = budgetRequestLine.IsSendForApproval,
                    OrgBranchName = budgetRequestLine.OrgBranchName,
                    SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                    SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = budgetRequestLine.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = budgetRequestLine.ModifiedBy
                };

                entity.Amount = BudgetRequestHelper.GetAmount(budgetRequestLine);
                db.BudgetRequestLines.Add(entity);
                db.SaveChanges();
                budgetRequestLine.BudgetRequestLineId = entity.BudgetRequestLineId;
                budgetRequestLine.Amount = entity.Amount;
            }

            return Json(new[] { budgetRequestLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestLines_Update([DataSourceRequest]DataSourceRequest request, BudgetRequestLine budgetRequestLine)
        {
            BudgetRequestHelper BudgetRequestHelper = new BudgetRequestHelper();
            if (ModelState.IsValid)
            {
                var entity = new BudgetRequestLine
                {
                    BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                    BudgetRequestId = budgetRequestLine.BudgetRequestId,
                    BudgetRequestType = budgetRequestLine.BudgetRequestType,
                    Description = budgetRequestLine.Description,
                    AccountCode = budgetRequestLine.AccountCode,
                    PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                    PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                    RentTotalHr = budgetRequestLine.RentTotalHr,
                    RentPerHour = budgetRequestLine.RentPerHour,
                    DailySalary = budgetRequestLine.DailySalary,
                    NoWorkingDay = budgetRequestLine.NoWorkingDay,
                    SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                    ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                    DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                    ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                    OverTime = budgetRequestLine.OverTime,
                    Unit = budgetRequestLine.Unit,
                    FromMonth = budgetRequestLine.FromMonth,
                    ToMonth = budgetRequestLine.ToMonth,
                    Amount = budgetRequestLine.Amount,
                    Reason = budgetRequestLine.Reason,
                    Status = budgetRequestLine.Status,
                    Remark = budgetRequestLine.Remark,
                    IsVoid = budgetRequestLine.IsVoid,
                    VoidBy = budgetRequestLine.VoidBy,
                    VoidTime = budgetRequestLine.VoidTime,
                    IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                    OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                    OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                    IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                    IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                    OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                    IsSendForApproval = budgetRequestLine.IsSendForApproval,
                    OrgBranchName = budgetRequestLine.OrgBranchName,
                    SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                    SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                    DateCreated = budgetRequestLine.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = budgetRequestLine.CreatedBy,
                    ModifiedBy =User.Identity.Name
                };
                entity.Amount = BudgetRequestHelper.GetAmount(budgetRequestLine);
                db.BudgetRequestLines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                budgetRequestLine.Amount = entity.Amount;
            }

            return Json(new[] { budgetRequestLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestLines_Destroy([DataSourceRequest]DataSourceRequest request, BudgetRequestLine budgetRequestLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetRequestLine
                {
                    BudgetRequestLineId = budgetRequestLine.BudgetRequestLineId,
                    BudgetRequestType = budgetRequestLine.BudgetRequestType,
                    Description = budgetRequestLine.Description,
                    AccountCode = budgetRequestLine.AccountCode,
                    PurchasedRequestUnit = budgetRequestLine.PurchasedRequestUnit,
                    PurchasedRequestUnitPrice = budgetRequestLine.PurchasedRequestUnitPrice,
                    RentTotalHr = budgetRequestLine.RentTotalHr,
                    RentPerHour = budgetRequestLine.RentPerHour,
                    DailySalary = budgetRequestLine.DailySalary,
                    NoWorkingDay = budgetRequestLine.NoWorkingDay,
                    SalaryPerMonth = budgetRequestLine.SalaryPerMonth,
                    ResponsibleAllowance = budgetRequestLine.ResponsibleAllowance,
                    DisturbanceAllowance = budgetRequestLine.DisturbanceAllowance,
                    ProffiessionalAllowance = budgetRequestLine.ProffiessionalAllowance,
                    OverTime = budgetRequestLine.OverTime,
                    Unit = budgetRequestLine.Unit,
                    FromMonth = budgetRequestLine.FromMonth,
                    ToMonth = budgetRequestLine.ToMonth,
                    Amount = budgetRequestLine.Amount,
                    Reason = budgetRequestLine.Reason,
                    Status = budgetRequestLine.Status,
                    Remark = budgetRequestLine.Remark,
                    IsVoid = budgetRequestLine.IsVoid,
                    VoidBy = budgetRequestLine.VoidBy,
                    VoidTime = budgetRequestLine.VoidTime,
                    IsOnlineApproved = budgetRequestLine.IsOnlineApproved,
                    OnlineApprovedBy = budgetRequestLine.OnlineApprovedBy,
                    OnlineApprovedTime = budgetRequestLine.OnlineApprovedTime,
                    IsOnlineTransferred = budgetRequestLine.IsOnlineTransferred,
                    IsOnlineTransferredBy = budgetRequestLine.IsOnlineTransferredBy,
                    OnlineTransferredTime = budgetRequestLine.OnlineTransferredTime,
                    IsSendForApproval = budgetRequestLine.IsSendForApproval,
                    OrgBranchName = budgetRequestLine.OrgBranchName,
                    SendForApprovalBy = budgetRequestLine.SendForApprovalBy,
                    SendForApprovalTime = budgetRequestLine.SendForApprovalTime,
                    DateCreated = budgetRequestLine.DateCreated,
                    LastModified = budgetRequestLine.LastModified,
                    CreatedBy = budgetRequestLine.CreatedBy,
                    ModifiedBy = budgetRequestLine.ModifiedBy
                };

                db.BudgetRequestLines.Attach(entity);
                db.BudgetRequestLines.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { budgetRequestLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
