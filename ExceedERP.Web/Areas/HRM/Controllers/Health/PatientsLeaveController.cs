using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using XAPI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{

    [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
    public class PatientsLeaveController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        public static DateTime From { get; set; }
        public static string FromAmharic { get; set; }
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public ActionResult Index()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            GetPeriod();
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            else if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        private JsonResult GetPeriod()
        {
            var periodList = db.GLPeriods.ToList();
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GLPeriod>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PatientsLeave_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PatientsLeave> patientsleave = db.PatientsLeave.Where(x => x.PatientId == id);
            DataSourceResult result = patientsleave.ToDataSourceResult(request, patientsLeave => new
            {
                PatientsLeaveId = patientsLeave.PatientsLeaveId,
                PatientId = patientsLeave.PatientId,
                GlFiscalYearId = patientsLeave.GlFiscalYearId,
                Date = patientsLeave.Date,

                DateAmharic = patientsLeave.DateAmharic,
                FromAmharic = patientsLeave.FromAmharic,
                ToAmharic = patientsLeave.ToAmharic,
                TentativeDiagnosis = patientsLeave.TentativeDiagnosis,
                RecommendedRest = patientsLeave.RecommendedRest,
                From = patientsLeave.From,
                To = patientsLeave.To,
                Days = patientsLeave.Days,
                TreatedBy = patientsLeave.TreatedBy,
                Remark = patientsLeave.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthUser)]
        public ActionResult PatientsLeave_Create([DataSourceRequest]DataSourceRequest request, PatientsLeave patientsLeave, int id)
        {
            ModelState.Clear();

            if (patientsLeave.FromAmharic != null)
            {
                var splitDate = patientsLeave.FromAmharic.Split('/');
                patientsLeave.From = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "From Date is required!!");
            }
            if (patientsLeave.ToAmharic != null)
            {
                var splitDate = patientsLeave.ToAmharic.Split('/');
                patientsLeave.To = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "To Date is required!!");
            }
            if (patientsLeave.ToAmharic != null)
            {
                var splitDate = patientsLeave.DateAmharic.Split('/');
                patientsLeave.Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Date is required!!");
            }
            if (patientsLeave.From.Date > patientsLeave.To.Date)
            {
                ModelState.AddModelError("", "Please Check the date range!!");
            }


            if (ModelState.IsValid)
            {

                var entity = new PatientsLeave
                {
                    PatientId = id,
                    Date = patientsLeave.Date,
                    DateAmharic = DateAmharic,
                    ToAmharic = ToAmharic,
                    FromAmharic = FromAmharic,
                    TentativeDiagnosis = patientsLeave.TentativeDiagnosis,
                    RecommendedRest = patientsLeave.RecommendedRest,
                    GlFiscalYearId = patientsLeave.GlFiscalYearId,
                    From = patientsLeave.From,
                    To = patientsLeave.To,
                    Days = Convert.ToDecimal((To - From).TotalDays),
                    TreatedBy = patientsLeave.TreatedBy,
                    Remark = patientsLeave.Remark
                };

                db.PatientsLeave.Add(entity);
                db.SaveChanges();
                patientsLeave.PatientsLeaveId = entity.PatientsLeaveId;

                // Core.Domain.HRM.LeaveManagment.Leave leave = new Core.Domain.HRM.LeaveManagment.Leave();

                var card = db.PatientCard.FirstOrDefault(x => x.PatientId == id);
                if (card != null)
                {
                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == card.EmployeeId);
                    if (employee != null)
                    {
                        var entityy = new Core.Domain.HRM.LeaveManagment.Leave
                        {
                            EmployeeId = employee.EmployeeId,
                            LeaveType = LeaveType.SICK_LEAVE,
                            Amount = Convert.ToDecimal(patientsLeave.To.Subtract(patientsLeave.From).Days),
                            GlFiscalYearId = patientsLeave.GlFiscalYearId,
                            From = patientsLeave.From,
                            To = patientsLeave.To,
                            FromAmharic = patientsLeave.FromAmharic,
                            ToAmharic = patientsLeave.ToAmharic,
                            OrgName = "",//leave.OrgName,
                            Before = "",//leave.Before,
                            After = "",//leave.After,
                            Remark = patientsLeave.PatientsLeaveId.ToString(),
                            DateCreated = DateTime.Now,
                            LastModified = null,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            EmployeeCode = employee.EmployeeCode,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        };

                        db.Leaves.Add(entityy);
                        db.SaveChanges();
                    }

                }
            }

            return Json(new[] { patientsLeave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthUser)]
        public ActionResult PatientsLeave_Update([DataSourceRequest]DataSourceRequest request, PatientsLeave patientsLeave)
        {
            ModelState.Clear();
            if (patientsLeave.FromAmharic != null)
            {
                var splitDate = patientsLeave.FromAmharic.Split('/');
                patientsLeave.From = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "From Date is required!!");
            }
            if (patientsLeave.ToAmharic != null)
            {
                var splitDate = patientsLeave.ToAmharic.Split('/');
                patientsLeave.To = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "To Date is required!!");
            }
            if (patientsLeave.ToAmharic != null)
            {
                var splitDate = patientsLeave.DateAmharic.Split('/');
                patientsLeave.Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Date is required!!");
            }
            if (patientsLeave.From.Date > patientsLeave.To.Date)
            {
                ModelState.AddModelError("", "Please Check the date range!!");
            }

            if (ModelState.IsValid)
            {
                var entity = new PatientsLeave
                {
                    PatientsLeaveId = patientsLeave.PatientsLeaveId,
                    PatientId = patientsLeave.PatientId,
                    Date = patientsLeave.Date,
                    DateAmharic = patientsLeave.DateAmharic,
                    ToAmharic = patientsLeave.ToAmharic,
                    FromAmharic = patientsLeave.FromAmharic,
                    TentativeDiagnosis = patientsLeave.TentativeDiagnosis,
                    RecommendedRest = patientsLeave.RecommendedRest,
                    GlFiscalYearId = patientsLeave.GlFiscalYearId,
                    From = patientsLeave.From,
                    To = patientsLeave.To,
                    Days = Convert.ToDecimal((To - From).TotalDays),
                    TreatedBy = patientsLeave.TreatedBy,
                    Remark = patientsLeave.Remark
                };

                db.PatientsLeave.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //Core.Domain.HRM.LeaveManagment.Leave leave = new Core.Domain.HRM.LeaveManagment.Leave();
                var card = db.PatientCard.FirstOrDefault(x => x.PatientId == patientsLeave.PatientId);
                if (card != null)
                {
                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == card.PatientCardId);
                    if (employee != null)
                    {
                        var leave = db.Leaves.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.GlFiscalYearId == patientsLeave.GlFiscalYearId && x.Remark == patientsLeave.PatientsLeaveId.ToString());
                        if (leave != null)
                        {
                            ExceedDbContext db2 = new ExceedDbContext();
                            var entityy = new Core.Domain.HRM.LeaveManagment.Leave
                            {
                                TakenLeaveId = leave.TakenLeaveId,
                                EmployeeId = leave.EmployeeId,
                                LeaveType = leave.LeaveType,
                                Amount = Convert.ToDecimal(patientsLeave.To.Subtract(patientsLeave.From).Days),
                                GlFiscalYearId = patientsLeave.GlFiscalYearId,
                                From = patientsLeave.From,
                                To = patientsLeave.To,
                                ToAmharic = patientsLeave.ToAmharic,
                                FromAmharic = patientsLeave.FromAmharic,
                                OrgName = leave.OrgName,
                                Before = leave.Before,
                                After = leave.After,
                                Remark = patientsLeave.PatientsLeaveId.ToString()
                            };
                            db2.Leaves.Attach(entityy);
                            db2.Entry(entityy).State = EntityState.Modified;
                            db2.SaveChanges();
                        }
                    }
                }

            }

            return Json(new[] { patientsLeave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthAdmin)]
        public ActionResult PatientsLeave_Destroy([DataSourceRequest]DataSourceRequest request, PatientsLeave patientsLeave)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PatientsLeave
                {
                    PatientsLeaveId = patientsLeave.PatientsLeaveId,
                    PatientId = patientsLeave.PatientId,

                    Date = patientsLeave.Date,
                    DateAmharic = patientsLeave.DateAmharic,
                    FromAmharic = patientsLeave.FromAmharic,
                    ToAmharic = patientsLeave.ToAmharic,
                    TentativeDiagnosis = patientsLeave.TentativeDiagnosis,
                    RecommendedRest = patientsLeave.RecommendedRest,
                    GlFiscalYearId = patientsLeave.GlFiscalYearId,
                    From = patientsLeave.From,
                    To = patientsLeave.To,
                    Days = patientsLeave.Days,
                    TreatedBy = patientsLeave.TreatedBy,
                    Remark = patientsLeave.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted sick leave information of patient card id " + patientsLeave.PatientId);
                #endregion
                db.PatientsLeave.Attach(entity);
                db.PatientsLeave.Remove(entity);
                db.SaveChanges();


                Core.Domain.HRM.LeaveManagment.Leave leave = new Core.Domain.HRM.LeaveManagment.Leave();
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == patientsLeave.PatientCard.PatientCardId);
                if (employee != null)
                {
                    db = new ExceedDbContext();
                    leave = db.Leaves.Where(x => x.EmployeeId == employee.EmployeeId && x.GlFiscalYearId == patientsLeave.GlFiscalYearId && x.Remark == patientsLeave.PatientsLeaveId.ToString()).FirstOrDefault();
                    if (leave != null)
                    {
                        db = new ExceedDbContext();
                        var entityy = new Core.Domain.HRM.LeaveManagment.Leave
                        {
                            TakenLeaveId = leave.TakenLeaveId,
                            EmployeeId = leave.EmployeeId,
                            LeaveType = leave.LeaveType,
                            Amount = leave.Amount,
                            GlFiscalYearId = leave.GlFiscalYearId,
                            From = leave.From,
                            To = leave.To,
                            OrgName = leave.OrgName,
                            Before = leave.Before,
                            After = leave.After,
                            Remark = leave.Remark
                        };
                        db = new ExceedDbContext();
                        db.Leaves.Attach(entityy);
                        db.Leaves.Remove(entityy);
                        db.SaveChanges();
                    }
                }
            }

            return Json(new[] { patientsLeave }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
