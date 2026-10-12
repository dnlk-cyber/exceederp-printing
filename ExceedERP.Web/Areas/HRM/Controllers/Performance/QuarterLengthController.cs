using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin)]
    public class QuarterLengthController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var fisaclYear = db.GLFiscalYears.ToList();
            ViewData["PeriodList"] = fisaclYear;
        }
        public JsonResult GetPeriod()
        {
            var periods = db.GLPeriods.ToList();

            return Json(periods.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult QuarterLength_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<QuarterLength> quarterLengths = db.QuarterLengths;
            DataSourceResult result = quarterLengths.ToDataSourceResult(request, quarterLength => new
            {
                QuarterLengthId = quarterLength.QuarterLengthId,
                Length = quarterLength.Length,
                Remark = quarterLength.Remark,
                GlFiscalYearId = quarterLength.GlFiscalYearId,
                IsClosed = quarterLength.IsClosed,
                CreatedBy = quarterLength.CreatedBy,
                DateCreated = quarterLength.DateCreated,
                ModifiedBy = quarterLength.ModifiedBy,
                LastModified = quarterLength.LastModified,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuarterLength_Create([DataSourceRequest]DataSourceRequest request, QuarterLength quarterLength)
        {

            var checkExistance = db.QuarterLengths
                            .FirstOrDefault(x => x.GlFiscalYearId == quarterLength.GlFiscalYearId);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This Fiscal Year is already registred!!");
                return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));

            }
            if (quarterLength.Length != 1 
                && quarterLength.Length != 2 
                && quarterLength.Length != 3
                && quarterLength.Length != 4 
                && quarterLength.Length != 12)
            {
                ModelState.AddModelError("","Quarter length should eqluas one of the folowing 1/Yearly, 2/Half Year, 3/ 1/3 Year, 4/Quarterly ,12 Monthly");
                return Json(new[] {     quarterLength }.ToDataSourceResult(request, ModelState));

            }
            var fiscalYear = db.GLFiscalYears.Find(quarterLength.GlFiscalYearId);
            if(fiscalYear == null)
            {
                ModelState.AddModelError("", "This Fiscal Year record not found");
                return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));

            }
            quarterLength.CreatedBy = User.Identity.Name;
            quarterLength.DateCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(quarterLength);

                db.QuarterLengths.Add(entity);
                db.SaveChanges();
                quarterLength.QuarterLengthId = entity.QuarterLengthId;

                GenerateQuarter(quarterLength.Length, quarterLength.QuarterLengthId);

                #region operation log
                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == entity.GlFiscalYearId);
                if (year != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "QuarterLength", "Success", User.Identity.Name, User.Identity.Name + " add new Quarter length for fiscal year " + year.Name + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);
                }
                #endregion
            }

            return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuarterLength_Update([DataSourceRequest] DataSourceRequest request, QuarterLength quarterLength)
        {
            quarterLength.ModifiedBy = User.Identity.Name;
            quarterLength.LastModified = DateTime.Now;

            var quarterLengthExisted = db.QuarterLengths.Find(quarterLength.QuarterLengthId);
            if (quarterLengthExisted == null)
            {
                ModelState.AddModelError("", "Record not found.");
                return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));
            }

            // Validation: Prevent update if both Fiscal Year and Length are changed (force new record)
            if (quarterLength.GlFiscalYearId != quarterLengthExisted.GlFiscalYearId
                && quarterLength.Length == quarterLengthExisted.Length)
            {
                ModelState.AddModelError("", "You cannot update this record. Please create a new entry instead.");
                return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));
            }
            var quarterIds = db.Quarters
                                     .Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId)
                                     .Select(x => x.QuarterId)
                                     .ToList(); // Materialize the list for reuse

            if (quarterIds.Any())
            {
                var dependentRecordsExist = db.EmployeePerformances
                                          .Where(x => quarterIds.Contains(x.GlPeriodId) 
                                                   && x.IsApproved).Any();

                if (dependentRecordsExist)
                {
                    ModelState.AddModelError("", "You cannot update this record because the length change will affect existing quarters that are already in use.");
                    return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));
                }
            }

            if (ModelState.IsValid)
            {
                // Update existing entity instead of mapping a new object
                if (quarterLength.Length != quarterLengthExisted.Length)
                {
                    DeleteQuarters(quarterLength.QuarterLengthId);
                    quarterLengthExisted.GlFiscalYearId = quarterLength.GlFiscalYearId;
                    quarterLengthExisted.Length = quarterLength.Length;
                    quarterLengthExisted.ModifiedBy = quarterLength.ModifiedBy;
                    quarterLengthExisted.LastModified = quarterLength.LastModified;
                    GenerateQuarter(quarterLength.Length, quarterLength.QuarterLengthId);
                }
                else
                {
                    quarterLengthExisted.GlFiscalYearId = quarterLength.GlFiscalYearId;
                    quarterLengthExisted.Length = quarterLength.Length;
                    quarterLengthExisted.ModifiedBy = quarterLength.ModifiedBy;
                    quarterLengthExisted.LastModified = quarterLength.LastModified;
                }
                db.Entry(quarterLengthExisted).State = EntityState.Modified; 
                db.SaveChanges(); // Save changes before deleting quarters

                // Operation Log
                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == quarterLengthExisted.GlFiscalYearId);
                if (year != null)
                {
                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "QuarterLength",
                        "Success",
                        User.Identity.Name,
                        $"{User.Identity.Name} updated Quarter length of fiscal year {year.Name} on {DateTime.Today.ToShortDateString()}",
                        Modules.HRM
                    );
                }
            }

            return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuarterLength_Destroy([DataSourceRequest]DataSourceRequest request, QuarterLength quarterLength)
        {
            var usedQurterList = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId)
                .Select(x => x.QuarterId);
            var checkExistance = db.EmployeePerformances.Where(x => usedQurterList.Contains(x.GlPeriodId));
            if (checkExistance.Any())
            {
                ModelState.AddModelError("",
                    "You can not delete this quarter length. \nUnder this qurter there is evaluated employees!!");
                return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(quarterLength);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Quarter length Id " + quarterLength.QuarterLengthId);
                #endregion
                db.QuarterLengths.Attach(entity);
                db.QuarterLengths.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { quarterLength }.ToDataSourceResult(request, ModelState));
        }
        private QuarterLength GetMappedObject(QuarterLength quarterLength)
        {
            var entity = new QuarterLength
            {
                QuarterLengthId = quarterLength.QuarterLengthId,
                Length = quarterLength.Length,
                Remark = quarterLength.Remark,
                GlFiscalYearId = quarterLength.GlFiscalYearId,
                IsClosed = quarterLength.IsClosed,
                CreatedBy = quarterLength.CreatedBy,
                DateCreated = quarterLength.DateCreated,
                ModifiedBy = quarterLength.ModifiedBy,
                LastModified = quarterLength.LastModified,

            };
            return entity;
        }

        private void DeleteQuarters(int id)
        {
            try
            {
                var quarters = db.Quarters.Where(q => q.QuarterLengthId == id).ToList(); // Load quarters first

                if (!quarters.Any())
                {
                    return; // No quarters found, exit method
                }

                foreach (var quarter in quarters)
                {
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Quarter Id " + quarter.QuarterId);
                    #endregion
                    db.Entry(quarter).State = EntityState.Deleted; // Mark for deletion
                }

                db.SaveChanges(); // Commit all deletions in one transaction
            }
            catch (Exception ex)
            {
                // Log the error or handle it accordingly
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Failed", User.Identity.Name, "Error deleting quarters: " + ex.Message);
            }
        }

        private void GenerateQuarter(int length, int quarterLId)
        {
            string amname = "ኛ ሩብ ዓመት";
            string name = "Quarter ";

            var yearId = db.QuarterLengths.FirstOrDefault(x => x.QuarterLengthId == quarterLId);
            if (yearId == null) return;

            var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == yearId.GlFiscalYearId);
            if (year == null) return;

            TimeSpan totalDuration = year.DateTo - year.DateFrom;
            TimeSpan interval = TimeSpan.FromTicks(totalDuration.Ticks / length);
            DateTime start = year.DateFrom;

            for (int i = 1; i <= length; i++)
            {
                DateTime end = (i == length) ? year.DateTo : start + interval;

                var entity = new Quarter { QuarterLengthId = quarterLId };

                // Assign proper names based on length
                switch (length)
                {
                    case 1:
                        name = "Yearly";
                        amname = "ዓመታዊ";
                        break;
                    case 2:
                        name = $"Half Year {i}";
                        amname = $"{i}ኛ ግማሽ ዓመት";
                        break;
                    case 3:
                        name = $"1/3 Year {i}";
                        amname = $"{i}ኛ ሲሶ ዓመት";
                        break;
                    case 4:
                        name = $"Quarter {i}";
                        amname = $"{i}ኛ ሩብ አመት";
                        break;
                    case 12:
                        name = $"Month {i}";
                        amname = $"{i}ኛ ወር";
                        break;
                }

                entity.DateFrom = start;
                entity.DateTo = end;
                entity.Name = name;
                entity.AmharicName = amname;
                entity.Sequence = i;

                db.Quarters.Add(entity);
                db.SaveChanges();
                start = end.AddDays(1); // Move to the next period
            }
        }

        //private void GenerateQuarter(int length, int quarterLId)
        //{
        //    string amname = "ኛ ሩብ ዓመት";
        //    string name = "Qurter ";

        //    var yearId = db.QuarterLengths.FirstOrDefault(x => x.QuarterLengthId == quarterLId);
        //    if (yearId != null)
        //    {
        //        var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == yearId.GlFiscalYearId);
        //        if (year != null)
        //        {
        //            TimeSpan totalDuration = year.DateTo - year.DateFrom;
        //            TimeSpan interval = TimeSpan.FromTicks(totalDuration.Ticks / length);
        //            DateTime start = year.DateFrom;
        //            for (int i = 1; i <= length; i++)
        //            {
        //                DateTime end = (i == length) ? year.DateTo : start + interval;

        //                var entity = new Quarter();

        //                amname = "ኛ ሩብ ዓመት ";
        //                name = "Qurter ";

        //                entity.QuarterLengthId = quarterLId;

        //                if (length == 1)
        //                {
        //                    name = "Yearly ";
        //                    amname = "ዓመታዊ ";
        //                }
        //                else if (length == 2)
        //                {
        //                    name = "Half Year " + i;
        //                    amname = i + "ኛ ግማሽ ዓመት ";
        //                }
        //                else if (length == 3)
        //                {
        //                    name = "1/3 Year " + i;
        //                    amname = i + " ኛ ሲሶ ዓመት ";
        //                }
        //                else if (length == 4)
        //                {
        //                    name = "Quarter " + i;
        //                    amname = i + "ኛ ሩብ አመት ";
        //                }
        //                else if (length == 12)
        //                {
        //                    name = "Month " + i;
        //                    amname = i + "ኛ ወር ";
        //                }

        //                entity.DateFrom = start;
        //                entity.DateTo = end;
        //                entity.Name = name;
        //                entity.AmharicName = amname;
        //                entity.Sequence = i;
        //                db.Quarters.Add(entity);
        //                db.SaveChanges();
        //                start = end.AddDays(1); // Move to the next period


        //            }
        //        }

        //    }
        //}
        public JsonResult CloseQuarterLength(int id, string reason)
        {
            try
            {
                var quarterLength = db.QuarterLengths.Find(id);
                if (quarterLength == null)
                {
                    return Json(new { Message = "Record not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
                }

                if (quarterLength.GlFiscalYear.DateTo.Date > DateTime.Now.Date)
                {
                    return Json(new
                    {
                        Message = $"You can't close this quarter length (fiscal year) before {quarterLength.GlFiscalYear.DateTo:yyyy-MM-dd}.",
                        Status = "Not-Ok"
                    }, JsonRequestBehavior.AllowGet);
                }

                // Toggle quarter length status
                bool isClosing = !quarterLength.IsClosed;
                quarterLength.IsClosed = isClosing;
                quarterLength.Remark = (quarterLength.Remark ?? "") + $"{reason} - {(isClosing ? "Closed" : "Reopened")}\n";
                quarterLength.ClosedDate = DateTime.Now;
                quarterLength.ClosedBy = User.Identity.Name;

                db.Entry(quarterLength).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Message = isClosing
                        ? $"Quarter Length closed successfully on {quarterLength.ClosedDate:yyyy-MM-dd}."
                        : "Quarter Length reopened successfully.",
                    Status = "Ok"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "An error occurred while processing the request.",
                    Status = "Not-Ok",
                    Error = ex.Message // Useful for debugging; remove in production.
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CloseQuarter(int id, string reason)
        {
            try
            {
                var quarter = db.Quarters.Find(id);
                if (quarter == null)
                {
                    return Json(new { Message = "Record not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
                }

                if (quarter.DateTo.HasValue &&quarter.DateTo.Value.Date > DateTime.Now.Date)
                {
                    return Json(new
                    {
                        Message = $"You can't close this quarter before {quarter.DateTo.Value:yyyy-MM-dd}.",
                        Status = "Not-Ok"
                    }, JsonRequestBehavior.AllowGet);
                }

                // Update quarter length properties
                bool isClosing = !quarter.IsClosed;
                quarter.IsClosed = isClosing;
                quarter.Remark = (quarter.Remark ?? "") + $"{reason} - {(isClosing ? "Closed" : "Reopened")}\n";
                quarter.ClosedDate = DateTime.Now;
                quarter.ClosedBy = User.Identity.Name;

                db.Entry(quarter).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Message = $"Quarter closed successfully on {quarter.ClosedDate:yyyy-MM-dd}.",
                    Status = "Ok"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "An error occurred while closing the quarter.",
                    Status = "Ok",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
