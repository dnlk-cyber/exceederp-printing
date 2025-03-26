using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class OverTimeTransferMaintainController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        private const string HR_Employee_SelfService = "HR_Employee_SelfService";
        private const string HR_Admin = "HR_Admin";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {

            PopulateViewData();

            var periodList = db.GLPeriods.Where(x => x.Status == FiscalYearStatus.Open).ToList();

            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g.GLPeriodId,
                           g.Name
                       });

            ViewData["PeriodList"] = list.ToList().Select(x => new {
                GLPeriodId = x.GLPeriodId,
                Name = x.Name
            }).ToList(); ;

            //ViewData["EmployeeList"] = db.Person_Employee
            //                                .Where(x => x.IsActive)
            //                                .Select(x=>new { x.EmployeeId, FullNameEnglish = x.EmployeeCode+"-"+x.FirstNameEnglish+" "+x.MiddleNameEnglish+" "+x.LastNameEnglish});

            return View();
        }
        public JsonResult GetCurrentYearPeriodsForOvertime()
        {
            List<GLPeriod> gLPeriods = new List<GLPeriod>();

            var periodList = db.GLPeriods.Where(x => x.Status == FiscalYearStatus.Open).ToList();

            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });



            return Json(list.OrderByDescending(x => x.g.DateFrom).Select(p => new
            {
                GLPeriodId = p.g.GLPeriodId,
                Name = p.g.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public void PopulateViewData()
        {
            List<Person_Employee> empList = new List<Person_Employee>();
            var employees = db.OverTimeRequestEmployee.ToList();
            foreach (var employee in employees)
            {
                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                if (emp != null)
                {
                    empList.Add(emp);
                }
            }
            ViewData["EmployeeList"] = db.Person_Employee
                                            .Where(x => x.IsActive)
                                            .Select(x => new { x.EmployeeId, FullNameEnglish = x.EmployeeCode + "-" + x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OverTimeTransfer> products)
        {
            var results = new List<OverTimeTransfer>();


            OverTime overTime = new OverTime();
            ExceedDbContext dbs = new ExceedDbContext();
            var transfres = products.Select(x => x.OverTimeTransferId);
            var list = dbs.OverTimeTransfers.Where(x => transfres.Contains(x.OverTimeTransferId));
            if (list.Any())
            {
                dbs.OverTimeTransfers.RemoveRange(list);
                dbs.SaveChanges();
            }
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    dbs = new ExceedDbContext();
                    var check = dbs.OverTimes.Where(x => x.EmployeeId == product.EmployeeId && x.GLPeriodId == product.GLPeriodId).FirstOrDefault();
                    var checkProcess = db.PayrollProcessViewModel.Where(x => x.EmployeeId == product.EmployeeId && product.GLPeriodId == x.GLPeriodId).FirstOrDefault();
                    if (product.Check)
                    {
                        if (check != null)
                        {

                            if (checkProcess != null)
                            {
                                ModelState.AddModelError("", "You can not update employee " + checkProcess.FullName + "information.It already processed.");
                            }
                            else
                            {
                                overTime = new OverTime();
                                overTime.EmployeeId = product.EmployeeId;

                                overTime.OverTimeId = check.OverTimeId;
                                overTime.GLPeriodId = product.GLPeriodId;
                                overTime.NormalHours = product.NormalHours;
                                overTime.NightHours = product.NightHours;
                                overTime.HolidayHours = product.HolidayHours;
                                overTime.RestdayHours = product.RestdayHours;
                                overTime.Remark = overTime.Remark;
                                overTime.ModifiedBy = User.Identity.Name;
                                overTime.LastModified = DateTime.Now;
                                overTime.DateCreated = check.DateCreated;
                                overTime.CreatedBy = check.CreatedBy;
                                dbs.OverTimes.Attach(overTime);
                                dbs.Entry(overTime).State = EntityState.Modified;
                                dbs.SaveChanges();
                            }
                        }
                        else
                        {
                            overTime = new OverTime();
                            overTime.EmployeeId = product.EmployeeId;
                            overTime.GLPeriodId = product.GLPeriodId;
                            overTime.NormalHours = product.NormalHours;
                            overTime.NightHours = product.NightHours;
                            overTime.HolidayHours = product.HolidayHours;
                            overTime.RestdayHours = product.RestdayHours;
                            overTime.Remark = overTime.Remark;
                            overTime.LastModified = null;
                            overTime.DateCreated = DateTime.Now;
                            overTime.CreatedBy = User.Identity.Name;
                            overTime.ModifiedBy = null;
                            dbs.OverTimes.Add(overTime);
                            dbs.SaveChanges();
                            overTime.OverTimeId = overTime.OverTimeId;
                        }
                    }
                    else
                    {
                        if (checkProcess != null)
                        {
                            ModelState.AddModelError("", "You can not update employee " + checkProcess.FullName + "information.It already processed.");
                        }
                        else
                        {
                            if (check != null)
                            {
                                dbs.OverTimes.Remove(check);
                                dbs.SaveChanges();
                            }
                        }
                    }
                    var entity = new OverTimeTransfer
                    {
                        EmployeeId = product.EmployeeId,
                        Date = DateTime.Now,
                        GLPeriodId = product.GLPeriodId,
                        IsSendForApproval = product.IsSendForApproval,
                        NormalHours = product.NormalHours,
                        NightHours = product.NightHours,
                        HolidayHours = product.HolidayHours,
                        RestdayHours = product.RestdayHours,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = product.ModifiedBy,
                        LastModified = product.LastModified,
                        Check = product.Check,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };
                    db.OverTimeTransfers.Add(entity);
                    db.SaveChanges();
                    product.OverTimeTransferId = entity.OverTimeTransferId;
                }

            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        public ActionResult OverTimeTransfers_Read([DataSourceRequest] DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<int> employees = inputs.GetUserBranchAndBusinessUnitEmployees(User.Identity.Name); //inputs.GetUserBranchEmployees(User.Identity.Name);
            //IQueryable<OverTimeTransfer> overtimetransfers = db.OverTimeTransfers.Where(x => employees.Contains(x.EmployeeId.ToString()));
            IQueryable<OverTimeTransfer> overtimetransfers = db.OverTimeTransfers.Where(x => employees.Contains(x.EmployeeId));

            DataSourceResult result = overtimetransfers.ToDataSourceResult(request, overTimeTransfer => new
            {
                OverTimeTransferId = overTimeTransfer.OverTimeTransferId,
                EmployeeId = overTimeTransfer.EmployeeId,
                IsSendForApproval = overTimeTransfer.IsSendForApproval,
                Date = overTimeTransfer.Date,
                GLPeriodId = overTimeTransfer.GLPeriodId,
                NormalHours = overTimeTransfer.NormalHours,
                NightHours = overTimeTransfer.NightHours,
                HolidayHours = overTimeTransfer.HolidayHours,
                RestdayHours = overTimeTransfer.RestdayHours,
                CreatedBy = overTimeTransfer.CreatedBy,
                DateCreated = overTimeTransfer.DateCreated,
                ModifiedBy = overTimeTransfer.ModifiedBy,
                LastModified = overTimeTransfer.LastModified,
                Check = overTimeTransfer.Check,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeTransfers_Create([DataSourceRequest] DataSourceRequest request, OverTimeTransfer overTimeTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new OverTimeTransfer
                {
                    EmployeeId = overTimeTransfer.EmployeeId,
                    Date = DateTime.Now,
                    GLPeriodId = overTimeTransfer.GLPeriodId,
                    IsSendForApproval = overTimeTransfer.IsSendForApproval,
                    NormalHours = overTimeTransfer.NormalHours,
                    NightHours = overTimeTransfer.NightHours,
                    HolidayHours = overTimeTransfer.HolidayHours,
                    RestdayHours = overTimeTransfer.RestdayHours,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = overTimeTransfer.ModifiedBy,
                    LastModified = overTimeTransfer.LastModified,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    Check = overTimeTransfer.Check,
                };

                db.OverTimeTransfers.Add(entity);
                db.SaveChanges();
                overTimeTransfer.OverTimeTransferId = entity.OverTimeTransferId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime transfer maintain", "Success", User.Identity.Name, $"added overtime transfer for employee  {entity.EmployeeId} period {entity.GLPeriodId} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { overTimeTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeTransfers_Update([DataSourceRequest] DataSourceRequest request, OverTimeTransfer overTimeTransfer)
        {
            ModelState.Clear();
            var over = db.OverTimeTransfers.FirstOrDefault(x => x.OverTimeTransferId == overTimeTransfer.OverTimeTransferId && x.IsSendForApproval);
            if (over != null)
            {
                ModelState.AddModelError("", "Cannot update approved requests");
            }
            if (ModelState.IsValid)
            {
                var entity = new OverTimeTransfer
                {
                    OverTimeTransferId = overTimeTransfer.OverTimeTransferId,
                    EmployeeId = overTimeTransfer.EmployeeId,
                    Date = overTimeTransfer.Date,
                    IsSendForApproval = overTimeTransfer.IsSendForApproval,
                    GLPeriodId = overTimeTransfer.GLPeriodId,
                    NormalHours = overTimeTransfer.NormalHours,
                    NightHours = overTimeTransfer.NightHours,
                    HolidayHours = overTimeTransfer.HolidayHours,
                    RestdayHours = overTimeTransfer.RestdayHours,
                    CreatedBy = overTimeTransfer.CreatedBy,
                    DateCreated = overTimeTransfer.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName = overTimeTransfer.OrgBranchName,
                    Check = overTimeTransfer.Check,
                };

                db.OverTimeTransfers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime transfer maintain", "Success", User.Identity.Name, $"added overtime transfer for employee  {entity.EmployeeId} period {entity.GLPeriodId} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { overTimeTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeTransfers_Destroy([DataSourceRequest] DataSourceRequest request, OverTimeTransfer overTimeTransfer)
        {
            ModelState.Clear();
            var over = db.OverTimeTransfers.FirstOrDefault(x => x.OverTimeTransferId == overTimeTransfer.OverTimeTransferId && x.IsSendForApproval);
            if (over != null)
            {
                ModelState.AddModelError("", "Cannot delete approved requests");
            }
            if (ModelState.IsValid)
            {
                var checkProcess = db.PayrollProcessViewModel.Where(x => x.EmployeeId == overTimeTransfer.EmployeeId && overTimeTransfer.GLPeriodId == x.GLPeriodId).FirstOrDefault();
                if (checkProcess != null)
                {
                    ModelState.AddModelError("", "You can not delete this information.It already processed.");
                }
                else
                {
                    var entity = new OverTimeTransfer
                    {
                        OverTimeTransferId = overTimeTransfer.OverTimeTransferId,
                        EmployeeId = overTimeTransfer.EmployeeId,
                        IsSendForApproval = overTimeTransfer.IsSendForApproval,
                        Date = overTimeTransfer.Date,
                        GLPeriodId = overTimeTransfer.GLPeriodId,
                        NormalHours = overTimeTransfer.NormalHours,
                        NightHours = overTimeTransfer.NightHours,
                        HolidayHours = overTimeTransfer.HolidayHours,
                        RestdayHours = overTimeTransfer.RestdayHours,
                        CreatedBy = overTimeTransfer.CreatedBy,
                        DateCreated = overTimeTransfer.DateCreated,
                        ModifiedBy = overTimeTransfer.ModifiedBy,
                        LastModified = overTimeTransfer.LastModified,
                        OrgBranchName = overTimeTransfer.OrgBranchName,
                    };
                    #region delete log
                    UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee over time transfer information of employee " + overTimeTransfer.EmployeeId + " and period " + overTimeTransfer.GLPeriodId, Modules.Payroll);
                    #endregion
                    db.OverTimeTransfers.Attach(entity);
                    db.OverTimeTransfers.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { overTimeTransfer }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult SendForApproval(int id)
        {
            var transfer = db.OverTimeTransfers.FirstOrDefault(x => x.OverTimeTransferId == id);
            if (transfer == null)
            {
                return Json(new
                {
                    Message = "record not Found.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }
            transfer.IsSendForApproval = true;
            db.Entry(transfer).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Message = "successfully sent for approval.",
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
