using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
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
                            FullName = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish,
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
        public ActionResult GenerateLeavePayable(int id)
        {
            //decimal balance = 0;
            //var leavePayableDetail = db.LeavePayableDetails.FirstOrDefault(x=>x.LeavePayableId==id);
            //if (leavePayableDetail == null)
            //{


                var leavePayable = db.LeavePayables.Find(id);
                if (leavePayable != null)
                {
                    var fiscalyear = db.GLFiscalYears.Find(leavePayable.GlFiscalYearId);
                    if (fiscalyear != null)
                    {
                        var glfiscal = db.GLFiscalYears.Where(x => x.DateTo < fiscalyear.DateTo).OrderByDescending(x => x.DateTo).Select(x => x.GlFiscalYearId).Take(2).ToArray();
                        var glfiscalyearIds = db.GLFiscalYears.Where(x => x.DateTo < fiscalyear.DateTo).OrderByDescending(x => x.DateTo).Select(x => x.GlFiscalYearId).Take(2);
                        var leaveEntitlements = db.LeavesEntitle.Where(x => !x.IsExpired).Where(x => glfiscalyearIds.Contains(x.GlFiscalYearId)).ToList();
                        var entitlements = leaveEntitlements.DistinctByField(x => x.EmployeeId).ToList();
                        foreach (var leaveEntitlement in entitlements)
                        {
                            int fiscalyear1 = glfiscal[0];
                            int fiscalyear2 = glfiscal[1];
                            var PayableDetail = db.LeavePayableDetails.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId&&x.LeavePayableId==id).FirstOrDefault();
                            if (PayableDetail == null)
                            {

                           
                                var balance1 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear1).Sum(x => x.Balance);
                                var payableDays1 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear1).Sum(x => x.Amount);
                                var payableDays2 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear2).Sum(x => x.Amount);
                                var balance2 = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitlement.EmployeeId && x.GlFiscalYearId == fiscalyear2).Sum(x => x.Balance);
                                var salary = db.Placements.Where(x => x.isCurrent && x.EmployeeId == leaveEntitlement.EmployeeId).FirstOrDefault();
                                if (salary != null)
                                {
                                    decimal payable1 = 0;
                                    decimal payable2 = 0;
                                    decimal total = 0;
                                    var leavePayableSetting = db.LeavePayableSettings.FirstOrDefault();

                                    if (leavePayableSetting != null && leavePayableSetting.CurrentValue != 0)
                                    {
                                        payable1 += ((salary.Salary * balance1) / leavePayableSetting.CurrentValue);
                                        payable2 += ((salary.Salary * balance2) / leavePayableSetting.CurrentValue);
                                        total += (payable1 + payable2);
                                    }

                                    var entity = new LeavePayableDetail
                                    {
                                        LeavePayableId = leavePayable.LeavePayableId,
                                        EmployeeCode = leaveEntitlement.EmployeeCode,
                                        EmployeeId = leaveEntitlement.EmployeeId,
                                        BranchId = leaveEntitlement.BranchId,
                                        BusinessUnitId = salary.BusinessUnitId,
                                        OrgStructureId = salary.OrgStructureId,
                                        PayableDays = payableDays1 + payableDays2,
                                        Balance = total,
                                        Year1 = payableDays1,
                                        Year2 = payableDays2,
                                        DateCreated = leavePayable.DateCreated,
                                        CreatedBy = leavePayable.CreatedBy,
                                        LastModified = leavePayable.LastModified,
                                        ModifiedBy = leavePayable.ModifiedBy
                                    };
                                    db.LeavePayableDetails.Add(entity);
                                    db.SaveChanges();

                                }
                                db.SaveChanges();

                            }
                           
                        }
                        var formattedData2 = new
                        {
                            Message = "Successfully Generated.",
                            Status = "OK"
                        };
                        return Json(formattedData2, JsonRequestBehavior.AllowGet);

                    }
                    var formattedData1 = new
                    {
                        Message = "There Is no Fiscal Year Record check it first .",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                var formattedData3 = new
                {
                    Message = "There Is no Such record.",
                    Status = "OK"
                };
                return Json(formattedData3, JsonRequestBehavior.AllowGet);
            //}
            //var formattedData = new
            //{
            //    Message = "It has Generated Once .you can't generate more than once.",
            //    Status = "OK"
            //};
           // return Json(formattedData, JsonRequestBehavior.AllowGet);
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
