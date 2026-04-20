using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin,
        HrmConstants.HRLeaveUser)]
    public class LeavePayablesController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/SickLeaveSetting
        HRMInputs inputs = new HRMInputs();

        public ActionResult Index()
        {
            GetPeriod();
            PopulateViewData();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
            ViewData["businessUnits"] = db.BusinessUnits;

            return View();
        }
        private void PopulateViewData()
        {
            var lists = db.Person_Employee.Select(x=>new 
            {
                            EmployeeId = x.EmployeeId,
                            FullName =x.EmployeeCode+"-" +x.FirstName + " " + x.MiddleName + " " + x.LastName,
                            FullNameEnglish = x.EmployeeCode + "-"+x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish,
                        }).ToList();
            ViewData["EmployeeList"] = lists;
            

        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeavePayables_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LeavePayable> leavePayables = db.LeavePayables;
            DataSourceResult result = leavePayables.ToDataSourceResult(request, leavePayable => new
            {
                LeavePayableId = leavePayable.LeavePayableId,
                GlFiscalYearId = leavePayable.GlFiscalYearId,
                Description = leavePayable.Description,
                DateCreated = leavePayable.DateCreated,
                CreatedBy = leavePayable.CreatedBy,
                LastModified = leavePayable.LastModified,
                ModifiedBy = leavePayable.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayables_Create([DataSourceRequest]DataSourceRequest request, LeavePayable leave)
        {
            bool exist = db.LeavePayables.Where(x => x.GlFiscalYearId == leave.GlFiscalYearId).Any();
            if (exist)
            {
                ModelState.AddModelError("", "Reacord Exists, repulicaton is Not allowed!!");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {


                var entity = GetMappedObject(leave);
                db.LeavePayables.Add(entity);
                db.SaveChanges();

                #region operation log

                        UserHelper.OperationLog(Request.UserHostAddress, "Leave payable ", "Success", User.Identity.Name, "Leave payable  is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayables_Update([DataSourceRequest]DataSourceRequest request, LeavePayable leave)
        {
            bool exist = db.LeavePayables.Where(x => x.GlFiscalYearId == leave.GlFiscalYearId
                                                    &&x.LeavePayableId!=leave.LeavePayableId).Any();
            if (exist)
            {
                ModelState.AddModelError("", "Reacord Exists, repulicaton is Not allowed!!");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(leave);
                db.LeavePayables.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Leave payable Setting", "Success", User.Identity.Name, "Leave payable Setting is Updated By  " + User.Identity.Name,Modules.HRM);                
                #endregion
            }


            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayables_Destroy([DataSourceRequest]DataSourceRequest request, LeavePayable leave)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(leave);
                #region delete log
                var leaves = db.GLFiscalYears.Find(leave.GlFiscalYearId);
                if(leaves != null)
                {
                    var delete = leaves.Name;
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave Plan of employee" + leave.GlFiscalYear.Name + " and fiscal year " + delete);
                }
               
                #endregion
                db.LeavePayables.Attach(entity);
                db.LeavePayables.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }
        private LeavePayable GetMappedObject(LeavePayable leavePayable)
        {
            var entity = new LeavePayable
            {
                LeavePayableId = leavePayable.LeavePayableId,
                GlFiscalYearId = leavePayable.GlFiscalYearId,
                Description = leavePayable.Description,
                DateCreated = leavePayable.DateCreated,
                CreatedBy = leavePayable.CreatedBy,
                LastModified = leavePayable.LastModified,
                ModifiedBy = leavePayable.ModifiedBy
            };
            return entity;
        }
        public ActionResult GenerateLeavePayable(int id,string employee=null)
        {
            var leavePayable = db.LeavePayables.Find(id);
            if (leavePayable == null)
            {
                return Json(new { Message = "There Is no Such record.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            var fiscalYear = db.GLFiscalYears.Find(leavePayable.GlFiscalYearId);
            if (fiscalYear == null)
            {
                return Json(new { Message = "There Is no Fiscal Year Record, check it first.", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);
            }
            var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault();
            if (leavePayableSetting == null || leavePayableSetting.CurrentValue == 0)
            {
                return Json(new { Message = "There Is no setted Leave PayableS ettings, set it first.", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);
            }
            if (leavePayableSetting.CurrentValue == 0)
            {
                return Json(new { Message = "There Is no Fiscal Year Record, check it first.", Status = "NOT-OK" }, JsonRequestBehavior.AllowGet);
            }
            var glFiscalYears = db.GLFiscalYears
                .Where(x => x.DateTo < fiscalYear.DateTo)
                .OrderByDescending(x => x.DateTo)
                .Take(2)
                .Select(x => x.GlFiscalYearId)
                .ToArray();

            var leaveEntitlements = db.LeavesEntitle
                .Where(x => !x.IsExpired && glFiscalYears.Contains(x.GlFiscalYearId))
                .ToList();
            if(!string.IsNullOrEmpty(employee)&&int.TryParse(employee,out int employeeId))
            {
                 leaveEntitlements = db.LeavesEntitle
                       .Where(x => !x.IsExpired &&x.EmployeeId==employeeId
                       && glFiscalYears.Contains(x.GlFiscalYearId))
                       .ToList();
            }

            foreach (var leaveEntitlement in leaveEntitlements.DistinctByField(x => x.EmployeeId))
            {
                var fiscalYear1 = glFiscalYears[0];
                var fiscalYear2 = glFiscalYears[1];
                var leavesEntitle1 = db.LeavesEntitle
                    .Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalYear1);
                var balance1 = leavesEntitle1.Any()?
                    leavesEntitle1.Sum(x => x.Balance):0;
                var payableDays1 = leavesEntitle1.Any() ?
                    leavesEntitle1.Sum(x => x.Amount):0;
                var leavesEntitle2 = db.LeavesEntitle
                    .Where(x => x.EmployeeId == leaveEntitlement.EmployeeId
                    && x.GlFiscalYearId == fiscalYear2);

                var balance2 = leavesEntitle2.Any()? leavesEntitle2
                                  .Sum(x => x.Balance):0;

                var payableDays2 = leavesEntitle2.Any() ? leavesEntitle2
                    .Sum(x => x.Amount):0;

                var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == leaveEntitlement.EmployeeId);
                if (placement == null)
                {
                    continue;
                }
                decimal payable1 = (placement.Salary * balance1) / leavePayableSetting.CurrentValue;
                decimal payable2 = (placement.Salary * balance2) / leavePayableSetting.CurrentValue;
               // decimal total = payable1 + payable2;
                decimal total = (placement.Salary*(balance1+balance2))/ leavePayableSetting.CurrentValue;
                decimal annualLeavePerMonth = total / 12;
                decimal aggregateTaxableAmount = annualLeavePerMonth + placement.Salary;
                var leavePayableDetail = db.LeavePayableDetails
                    .FirstOrDefault(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.LeavePayableId == id);

                if (leavePayableDetail == null)
                {
                    leavePayableDetail = new LeavePayableDetail
                    {
                        LeavePayableId = leavePayable.LeavePayableId,
                        EmployeeCode = leaveEntitlement.EmployeeCode,
                        EmployeeId = leaveEntitlement.EmployeeId,
                        BranchId = leaveEntitlement.BranchId,
                        BusinessUnitId = placement.BusinessUnitId,
                        OrgStructureId = placement.OrgStructureId,
                        BasicSalary = placement.Salary,
                        PayableDays = balance1 + balance2,
                        Year1 = balance1,
                        Year2 = balance2,
                        //Year1 = payableDays1,
                        //Year2 = payableDays2,
                        //Balance = total,
                        Balance = total,
                        AnnualLeavePerMonth = annualLeavePerMonth,
                        AgregateTaxableAmount = aggregateTaxableAmount,
                        AgregateIncomeTax = CalculateTax(aggregateTaxableAmount),
                        BasicSalaryIncomeTax = CalculateBasicSalaryIncomeTax(placement.Salary),
                        Remark = "Generated Leave Payable",
                        DateCreated = leavePayable.DateCreated,
                        CreatedBy = leavePayable.CreatedBy,
                        LastModified = leavePayable.LastModified,
                        ModifiedBy = leavePayable.ModifiedBy
                    };
                    leavePayableDetail.TaxDifference = (leavePayableDetail.AgregateIncomeTax - leavePayableDetail.BasicSalaryIncomeTax) * 12;
                    leavePayableDetail.NetPayment = leavePayableDetail.Balance - leavePayableDetail.TaxDifference;
                    db.LeavePayableDetails.Add(leavePayableDetail);
                }
                else
                {
                    leavePayableDetail.PayableDays = balance1 + balance2;
                    leavePayableDetail.Year1 = balance1;
                    leavePayableDetail.Year2 = balance2;
                    leavePayableDetail.Balance = total;
                    leavePayableDetail.BasicSalary = placement.Salary;
                    leavePayableDetail.AnnualLeavePerMonth = annualLeavePerMonth;
                    leavePayableDetail.AgregateTaxableAmount = aggregateTaxableAmount;
                    leavePayableDetail.AgregateIncomeTax = CalculateTax(aggregateTaxableAmount);
                    leavePayableDetail.BasicSalaryIncomeTax = CalculateBasicSalaryIncomeTax(placement.Salary);
                    leavePayableDetail.Remark = "Generated Leave Payable";
                    leavePayableDetail.LastModified = DateTime.Now;
                    leavePayableDetail.ModifiedBy = User.Identity.Name;
                    leavePayableDetail.TaxDifference = (leavePayableDetail.AgregateIncomeTax - leavePayableDetail.BasicSalaryIncomeTax) * 12;
                    leavePayableDetail.NetPayment = leavePayableDetail.Balance - leavePayableDetail.TaxDifference;
                    db.Entry(leavePayableDetail).State = EntityState.Modified;
                }
            }

            db.SaveChanges();

            return Json(new { Message = "Successfully Generated.", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        #region old codes
        //public ActionResult GenerateLeavePayable1(int id)
        //{
            
        //    var leavePayable = db.LeavePayables.Find(id);
        //    if (leavePayable != null)
        //    {
        //        var fiscalyear = db.GLFiscalYears.Find(leavePayable.GlFiscalYearId);
        //        if (fiscalyear != null)
        //        {
        //            var glfiscal = db.GLFiscalYears.Where(x => x.DateTo < fiscalyear.DateTo).OrderByDescending(x => x.DateTo).Select(x => x.GlFiscalYearId).Take(2).ToArray();
        //            var glfiscalyearIds = db.GLFiscalYears.Where(x => x.DateTo < fiscalyear.DateTo).OrderByDescending(x => x.DateTo).Select(x => x.GlFiscalYearId).Take(2);
        //            var leaveEntitlements = db.LeavesEntitle.Where(x => !x.IsExpired).Where(x => glfiscalyearIds.Contains(x.GlFiscalYearId)).ToList();
        //            var entitlements = leaveEntitlements.DistinctByField(x => x.EmployeeId).ToList();
        //            foreach (var leaveEntitlement in entitlements)
        //            {
        //                int fiscalyear1 = glfiscal[0];
        //                int fiscalyear2 = glfiscal[1];
        //                var balance1 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear1).Sum(x => x.Balance);
        //                var payableDays1 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear1).Sum(x => x.Amount);
        //                var payableDays2 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear2).Sum(x => x.Amount);
        //                var balance2 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear2).Sum(x => x.Balance);
        //                var placement = db.Placements.Where(x => x.isCurrent && x.EmployeeId == leaveEntitlement.EmployeeId).FirstOrDefault();
        //                var PayableDetail = db.LeavePayableDetails.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId&&x.LeavePayableId==id).FirstOrDefault();
        //                if (PayableDetail == null)
        //                {
                            
        //                    if (placement != null)
        //                    {
        //                        decimal payable1 = 0;
        //                        decimal payable2 = 0;
        //                        decimal total = 0;
        //                        decimal agregateTaxableAmount = 0;
        //                        var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault();

        //                        if (leavePayableSetting != null && leavePayableSetting.CurrentValue != 0)
        //                        {
        //                            payable1 += ((placement.Salary * balance1) / leavePayableSetting.CurrentValue);
        //                            payable2 += ((placement.Salary * balance2) / leavePayableSetting.CurrentValue);
        //                            total += (payable1 + payable2);
        //                            agregateTaxableAmount = total + placement.Salary;
        //                        }
        //                        var entity = new LeavePayableDetail
        //                        {
        //                            LeavePayableId = leavePayable.LeavePayableId,
        //                            EmployeeCode = leaveEntitlement.EmployeeCode,
        //                            EmployeeId = leaveEntitlement.EmployeeId,
        //                            BranchId = leaveEntitlement.BranchId,
        //                            BusinessUnitId = placement.BusinessUnitId,
        //                            OrgStructureId = placement.OrgStructureId,
        //                            BasicSalary = placement.Salary,
        //                            PayableDays = payableDays1 + payableDays2,
        //                            Year1 = payableDays1,
        //                            Year2 = payableDays2,
        //                            Balance = total,
        //                            AnnualLeavePerMonth = total/12,
        //                            AgregateTaxableAmount = agregateTaxableAmount,
        //                            AgregateIncomeTax = CalculateTax(agregateTaxableAmount),
        //                            BasicSalaryIncomeTax = CalculateBasicSalaryIncomeTax(placement.Salary),
        //                            Remark = "Generated Leave Payable",
        //                            DateCreated = leavePayable.DateCreated,
        //                            CreatedBy = leavePayable.CreatedBy,
        //                            LastModified = leavePayable.LastModified,
        //                            ModifiedBy = leavePayable.ModifiedBy
        //                        };
        //                        entity.TaxDifference = (entity.AgregateTaxableAmount - entity.AgregateIncomeTax) * 12;
        //                        entity.NetPayment = entity.Balance - entity.TaxDifference;
        //                        db.LeavePayableDetails.Add(entity);
                               

        //                    }
                           

        //                }
        //                {
        //                    if (placement != null)
        //                    {
        //                        decimal payable1 = 0;
        //                        decimal payable2 = 0;
        //                        decimal total = 0;
        //                        decimal agregateTaxableAmount = 0;
        //                        var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault();

        //                        if (leavePayableSetting != null && leavePayableSetting.CurrentValue != 0)
        //                        {
        //                            payable1 += ((placement.Salary * balance1) / leavePayableSetting.CurrentValue);
        //                            payable2 += ((placement.Salary * balance2) / leavePayableSetting.CurrentValue);
        //                            total += (payable1 + payable2);
        //                            agregateTaxableAmount = total + placement.Salary;
        //                        }

        //                        PayableDetail.LeavePayableId = leavePayable.LeavePayableId;
        //                        PayableDetail.EmployeeCode = leaveEntitlement.EmployeeCode;
        //                        PayableDetail.EmployeeId = leaveEntitlement.EmployeeId;
        //                        PayableDetail.BranchId = leaveEntitlement.BranchId;
        //                        PayableDetail.BusinessUnitId = placement.BusinessUnitId;
        //                        PayableDetail.OrgStructureId = placement.OrgStructureId;
        //                        PayableDetail.BasicSalary = placement.Salary;
        //                        PayableDetail.PayableDays = payableDays1 + payableDays2;
        //                        PayableDetail.Year1 = payableDays1;
        //                        PayableDetail.Year2 = payableDays2;
        //                        PayableDetail.Balance = total;
        //                        PayableDetail.AnnualLeavePerMonth = total / 12;
        //                        PayableDetail.AgregateTaxableAmount = agregateTaxableAmount;
        //                        PayableDetail.AgregateIncomeTax = CalculateTax(agregateTaxableAmount);
        //                        PayableDetail.BasicSalaryIncomeTax = CalculateBasicSalaryIncomeTax(placement.Salary);
        //                        PayableDetail.Remark = "Generated Leave Payable";
        //                        PayableDetail.LastModified = DateTime.Now;
        //                        PayableDetail.ModifiedBy = User.Identity.Name;
        //                        PayableDetail.TaxDifference = (PayableDetail.AgregateTaxableAmount - PayableDetail.AgregateIncomeTax) * 12;
        //                        PayableDetail.NetPayment = PayableDetail.Balance - PayableDetail.TaxDifference;
        //                        db.Entry(PayableDetail).State =EntityState.Modified;

        //                    }
        //                }
                           
        //            }
        //            db.SaveChanges();
        //            var formattedData2 = new
        //            {
        //                Message = "Successfully Generated.",
        //                Status = "OK"
        //            };
        //            return Json(formattedData2, JsonRequestBehavior.AllowGet);

        //        }
        //        var formattedData1 = new
        //        {
        //            Message = "There Is no Fiscal Year Record check it first .",
        //            Status = "OK"
        //        };
        //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
        //    }
        //    var formattedData3 = new
        //    {
        //        Message = "There Is no Such record.",
        //        Status = "OK"
        //    };
        //    return Json(formattedData3, JsonRequestBehavior.AllowGet);
           
        //}
        //public ActionResult GenerateLeavePayable2(int id)
        //{
        //    var leavePayable = db.LeavePayables.Find(id);
        //    if (leavePayable == null)
        //        return Json(new { Message = "No such record.", Status = "Error" }, JsonRequestBehavior.AllowGet);

        //    var fiscalYear = db.GLFiscalYears.Find(leavePayable.GlFiscalYearId);
        //    if (fiscalYear == null)
        //        return Json(new { Message = "No Fiscal Year Record found.", Status = "Error" }, JsonRequestBehavior.AllowGet);

        //    var previousFiscalYears = db.GLFiscalYears
        //        .Where(x => x.DateTo < fiscalYear.DateTo)
        //        .OrderByDescending(x => x.DateTo)
        //        .Select(x => x.GlFiscalYearId)
        //        .Take(2)
        //        .ToArray();

        //    if (previousFiscalYears.Length < 2)
        //        return Json(new { Message = "Not enough Fiscal Year data.", Status = "Error" }, JsonRequestBehavior.AllowGet);

        //    var leaveEntitlements = db.LeavesEntitle
        //        .Where(x => !x.IsExpired && previousFiscalYears.Contains(x.GlFiscalYearId))
        //        .ToList()
        //        .GroupBy(x => x.EmployeeId)
        //        .Select(g => g.First())
        //        .ToList();

        //    var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault();
        //    if (leavePayableSetting == null || leavePayableSetting.CurrentValue == 0)
        //        return Json(new { Message = "Leave Payable Settings are invalid.", Status = "Error" }, JsonRequestBehavior.AllowGet);

        //    foreach (var entitlement in leaveEntitlements)
        //    {
        //        ProcessLeavePayable(entitlement, previousFiscalYears, leavePayable, leavePayableSetting);
        //    }

        //    db.SaveChanges();

        //    return Json(new { Message = "Successfully Generated.", Status = "OK" }, JsonRequestBehavior.AllowGet);
        //}

        private void ProcessLeavePayable(LeaveEntitle entitlement, int[] fiscalYears, LeavePayable leavePayable, LeavePayableSetting setting)
        {
            var fiscalYear1 = fiscalYears[0];
            var fiscalYear2 = fiscalYears[1];

            var balance1 = db.LeavesEntitle.Where(x => x.EmployeeId == entitlement.EmployeeId && x.GlFiscalYearId == fiscalYear1).Sum(x => x.Balance);
            var balance2 = db.LeavesEntitle.Where(x => x.EmployeeId == entitlement.EmployeeId && x.GlFiscalYearId == fiscalYear2).Sum(x => x.Balance);
            var payableDays1 = db.LeavesEntitle.Where(x => x.EmployeeId == entitlement.EmployeeId && x.GlFiscalYearId == fiscalYear1).Sum(x => x.Amount);
            var payableDays2 = db.LeavesEntitle.Where(x => x.EmployeeId == entitlement.EmployeeId && x.GlFiscalYearId == fiscalYear2).Sum(x => x.Amount);

            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == entitlement.EmployeeId);
            if (placement == null) return;

            decimal payable1 = (placement.Salary * balance1) / setting.CurrentValue;
            decimal payable2 = (placement.Salary * balance2) / setting.CurrentValue;
            decimal total = payable1 + payable2;
            decimal agregateTaxableAmount = total + placement.Salary;

            var existingDetail = db.LeavePayableDetails.FirstOrDefault(x => x.EmployeeId == entitlement.EmployeeId && x.LeavePayableId == leavePayable.LeavePayableId);
            if (existingDetail == null)
            {
                var newDetail = CreateLeavePayableDetail(entitlement, leavePayable, placement, payableDays1, payableDays2, total, agregateTaxableAmount);
                db.LeavePayableDetails.Add(newDetail);
            }
            else
            {
                UpdateLeavePayableDetail(existingDetail, leavePayable, placement, payableDays1, payableDays2, total, agregateTaxableAmount);
            }
        }

        private LeavePayableDetail CreateLeavePayableDetail(LeaveEntitle entitlement, LeavePayable leavePayable, Placement placement, decimal payableDays1, decimal payableDays2, decimal total, decimal agregateTaxableAmount)
        {
            return new LeavePayableDetail
            {
                LeavePayableId = leavePayable.LeavePayableId,
                EmployeeCode = entitlement.EmployeeCode,
                EmployeeId = entitlement.EmployeeId,
                BranchId = entitlement.BranchId,
                BusinessUnitId = placement.BusinessUnitId,
                OrgStructureId = placement.OrgStructureId,
                BasicSalary = placement.Salary,
                PayableDays = payableDays1 + payableDays2,
                Year1 = payableDays1,
                Year2 = payableDays2,
                Balance = total,
                AnnualLeavePerMonth = total / 12,
                AgregateTaxableAmount = agregateTaxableAmount,
                AgregateIncomeTax = CalculateTax(agregateTaxableAmount),
                BasicSalaryIncomeTax = CalculateBasicSalaryIncomeTax(placement.Salary),
                Remark = "Generated Leave Payable",
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name,
                LastModified = DateTime.Now,
                ModifiedBy = User.Identity.Name,
                TaxDifference = (agregateTaxableAmount - CalculateTax(agregateTaxableAmount)) * 12,
                NetPayment = total - ((agregateTaxableAmount - CalculateTax(agregateTaxableAmount)) * 12)
            };
        }

        private void UpdateLeavePayableDetail(LeavePayableDetail detail, LeavePayable leavePayable, Placement placement, decimal payableDays1, decimal payableDays2, decimal total, decimal agregateTaxableAmount)
        {
            detail.LeavePayableId = leavePayable.LeavePayableId;
            detail.BasicSalary = placement.Salary;
            detail.PayableDays = payableDays1 + payableDays2;
            detail.Balance = total;
            detail.AgregateTaxableAmount = agregateTaxableAmount;
            detail.AgregateIncomeTax = CalculateTax(agregateTaxableAmount);
            detail.NetPayment = total - ((agregateTaxableAmount - detail.AgregateIncomeTax) * 12);
            detail.LastModified = DateTime.Now;
            detail.ModifiedBy = User.Identity.Name;
            db.Entry(detail).State = EntityState.Modified;
        }
        #endregion
        public decimal CalculateTax(decimal income)
        {
            if (income <= 600)
                return income * 0;
            else if (income <= 1650)
                return income * 0.1m - 60;
            else if (income <= 3200)
                return income * 0.15m - 142.5m;
            else if (income <= 5250)
                return income * 0.2m - 302.5m;
            else if (income <= 7800)
                return income * 0.25m - 565;
            else if (income <= 10900)
                return income * 0.3m - 955;
            else // if income > 10900
                return income * 0.35m - 1500; // Adjust as needed for income > 10900
        }
        public decimal CalculateBasicSalaryIncomeTax(decimal income)
        {
            if (income <= 600)
                return income * 0;
            else if (income <= 1650)
                return income * 0.1m - 60;
            else if (income <= 3200)
                return income * 0.15m - 142.5m;
            else if (income <= 5250)
                return income * 0.2m - 302.5m;
            else if (income <= 7800)
                return income * 0.25m - 565;
            else if (income <= 10900)
                return income * 0.3m - 955;
            else // income > 10900
                return income * 0.35m - 1500;
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
