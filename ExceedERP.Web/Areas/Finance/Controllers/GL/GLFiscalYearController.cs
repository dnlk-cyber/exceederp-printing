using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using static ExceedERP.Core.Domain.Global.GlobalEnum;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class GlFiscalYearController : Controller
    {
        private  ExceedDbContext _db = new ExceedDbContext();
        readonly FinanceHelper _financeHelper=new FinanceHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GLFiscalYears_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlFiscalYear> glfiscalyears = _db.GLFiscalYears;
            DataSourceResult result = glfiscalyears.ToDataSourceResult(request, gLFiscalYear => new
            {

                GlFiscalYearId = gLFiscalYear.GlFiscalYearId,
                Name = gLFiscalYear.Name,
                DateFrom = gLFiscalYear.DateFrom,
                DateTo = gLFiscalYear.DateTo,
                Status = gLFiscalYear.Status,

                OrgBranchName = gLFiscalYear.OrgBranchName,
                DateCreated = gLFiscalYear.DateCreated,
                LastModified = gLFiscalYear.LastModified,
                CreatedBy = gLFiscalYear.CreatedBy,
                ModifiedBy = gLFiscalYear.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLFiscalYears_Create([DataSourceRequest]DataSourceRequest request, GlFiscalYear gLFiscalYear)
        {
            #region validation

          
            var exists = _db.GLFiscalYears.Where(x => x.GlFiscalYearId != gLFiscalYear.GlFiscalYearId).Where(x => x.Name.ToUpper().Equals(gLFiscalYear.Name.ToUpper())).ToList();

            if (exists.Any())
                ModelState.AddModelError("", gLFiscalYear.Name + " already exists in the system.");

            //if (gLFiscalYear.DateFrom > gLFiscalYear.DateTo)
            //    ModelState.AddModelError("", "Start Date should be before End Date.");

            var overlaps = _db.GLFiscalYears.Where(x => x.Name != gLFiscalYear.Name)
                                                .Where(x => x.DateFrom <= gLFiscalYear.DateTo)
                                                .Where(x => x.DateTo >= gLFiscalYear.DateFrom).ToList();
            if (overlaps.Any())
                ModelState.AddModelError("", "The Fiscal Year's range overlaps with one or more previous Fiscal Years.");

            if (GlobalHelper.GetFiscalYearType()== FiscalYearTypeAttribute.Unknown)
                ModelState.AddModelError("superuser", "Type of fiscal year is not defined in master configuration.");

            if (GlobalHelper.GetFiscalYearType() == FiscalYearTypeAttribute.Ethiopian)
            {
                int day = XAPI.EthiopicDateTime.GetEthiopicDay(gLFiscalYear.DateFrom.Day, gLFiscalYear.DateFrom.Month, gLFiscalYear.DateFrom.Year);
                int month = XAPI.EthiopicDateTime.GetEthiopicMonth(gLFiscalYear.DateFrom.Day, gLFiscalYear.DateFrom.Month, gLFiscalYear.DateFrom.Year);
                if (day!=1 && month!=11)
                    ModelState.AddModelError("start date", "Ethiopian fiscal year should start at 1st of the 11th month of Ethiopian calendar.");
            }

            if (GlobalHelper.GetFiscalYearType() == FiscalYearTypeAttribute.Standard)
            {
                if (gLFiscalYear.DateFrom.Day!=1)
                    ModelState.AddModelError("start date", "Fiscal year should start at 1st a month of Gregorian calendar.");
            }

            #endregion

            gLFiscalYear.DateTo=gLFiscalYear.DateFrom.AddYears(1).AddDays(-1);
            if (ModelState.IsValid)
            {
                    var entity = new GlFiscalYear
                    {
                        Name = gLFiscalYear.Name,
                        DateFrom = gLFiscalYear.DateFrom.Date,
                        DateTo = gLFiscalYear.DateTo.Date,
                        Status = FiscalYearStatus.FuturePeriod,

                        OrgBranchName = gLFiscalYear.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = gLFiscalYear.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = gLFiscalYear.ModifiedBy
                    };

                    _db.GLFiscalYears.Add(entity);
                    _db.SaveChanges();
                    gLFiscalYear.GlFiscalYearId = entity.GlFiscalYearId;

                if (GlobalHelper.GetFiscalYearType() == FiscalYearTypeAttribute.Ethiopian)
                {
                    #region generate periods in the fiscal year European July 1

                    bool leapyear = DateTime.IsLeapYear(gLFiscalYear.DateTo.Year);
                  

                    List<GLPeriod> periods = new List<GLPeriod>();
                    var july = new GLPeriod
                    {
                        Name = $" {gLFiscalYear.DateFrom.ToString("MMMM")} - { gLFiscalYear.DateFrom.Year}",
                        Index = 1,
                        DateFrom = gLFiscalYear.DateFrom.Date,
                        DateTo = gLFiscalYear.DateFrom.Date.AddDays(29),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(july);

                    var august = new GLPeriod
                    {
                        Name = $" {july.DateTo.AddDays(1).ToString("MMMM")} - { july.DateTo.AddDays(1).Year}",
                        Index = 2,
                        DateFrom = july.DateTo.AddDays(1),
                        DateTo = leapyear? july.DateTo.AddDays(36) : july.DateTo.AddDays(35),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(august);

                    var september = new GLPeriod
                    {
                        Name = $" {august.DateTo.AddDays(1).ToString("MMMM")} - { august.DateTo.AddDays(1).Year}",
                        Index = 3,
                        DateFrom = august.DateTo.AddDays(1),
                        DateTo = august.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(september);

                    var october = new GLPeriod
                    {
                        Name = $" {september.DateTo.AddDays(1).ToString("MMMM")} - { september.DateTo.AddDays(1).Year}",
                        Index = 4,
                        DateFrom = september.DateTo.AddDays(1),
                        DateTo = september.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(october);

                    var november = new GLPeriod
                    {
                        Name = $" {october.DateTo.AddDays(1).ToString("MMMM")} - {october.DateTo.AddDays(1).Year}",
                        Index = 5,
                        DateFrom = october.DateTo.AddDays(1),
                        DateTo = october.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(november);

                    var december = new GLPeriod
                    {
                        Name = $" {november.DateTo.AddDays(1).ToString("MMMM")} - { november.DateTo.AddDays(1).Year}",
                        Index = 6,
                        DateFrom = november.DateTo.AddDays(1),
                        DateTo = november.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(december);

                    var january = new GLPeriod
                    {
                        Name = $" {december.DateTo.AddDays(1).ToString("MMMM")} - { december.DateTo.AddDays(1).Year}",
                        Index = 7,
                        DateFrom = december.DateTo.AddDays(1),
                        DateTo = december.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(january);

                    var february = new GLPeriod
                    {
                        Name = $" {january.DateTo.AddDays(1).ToString("MMMM")} - { january.DateTo.AddDays(1).Year}",
                        Index = 8,
                        DateFrom = january.DateTo.AddDays(1),
                        DateTo = january.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(february);

                    var march = new GLPeriod
                    {
                        Name = $" {february.DateTo.AddDays(1).ToString("MMMM")} - { february.DateTo.AddDays(1).Year}",
                        Index = 9,
                        DateFrom = february.DateTo.AddDays(1),
                        DateTo = february.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(march);

                    var april = new GLPeriod
                    {
                        Name = $" {march.DateTo.AddDays(1).ToString("MMMM")} - { march.DateTo.AddDays(1).Year}",
                        Index = 10,
                        DateFrom = march.DateTo.AddDays(1),
                        DateTo = march.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(april);

                    var may = new GLPeriod
                    {
                        Name = $" {april.DateTo.AddDays(1).ToString("MMMM")} - { april.DateTo.AddDays(1).Year}",
                        Index = 11,
                        DateFrom = april.DateTo.AddDays(1),
                        DateTo = april.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(may);

                    var june = new GLPeriod
                    {
                        Name = $" {may.DateTo.AddDays(1).ToString("MMMM")} - {may.DateTo.AddDays(1).Year}",
                        Index = 12,
                        DateFrom = may.DateTo.AddDays(1),
                        DateTo = may.DateTo.AddDays(30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(june);



                    foreach (var glPeriod in periods)
                    {
                        _db.GLPeriods.Add(glPeriod);
                        _db.SaveChanges();
                    }
                    #endregion
                }
                else if (GlobalHelper.GetFiscalYearType() == FiscalYearTypeAttribute.Standard)
                {
                    #region generate periods in the fiscal year Gregorian

                    DateTime startDate = gLFiscalYear.DateFrom; // Specify your desired start date

                    List<GLPeriod> periods = new List<GLPeriod>();

                    for (int i = 0; i < 12; i++)
                    {
                        DateTime currentDate = startDate.AddMonths(i);

                        int year = currentDate.Year;
                        int month = currentDate.Month;
                        int daysInMonth = DateTime.DaysInMonth(year, month);

                        DateTime periodStartDate = new DateTime(year, month, 1);
                        DateTime periodEndDate = new DateTime(year, month, daysInMonth);

                        var period = new GLPeriod
                        {
                            Name = $"{periodStartDate.ToString("MMMM")} - {year}",
                            Index = i + 1,
                            DateFrom = periodStartDate,
                            DateTo = periodEndDate,
                            Status = FiscalYearStatus.FuturePeriod,
                            GlFiscalYearId = entity.GlFiscalYearId
                        };

                        periods.Add(period);
                    }
                    




                    foreach (var glPeriod in periods)
                    {
                        _db.GLPeriods.Add(glPeriod);
                        _db.SaveChanges();
                    }
                    #endregion
                }
                //    #region generate periods in the fiscal year European July 1

                //int leapyear = 0;
                //    if (DateTime.IsLeapYear(gLFiscalYear.DateTo.Year))
                //        leapyear = 1;

                //    List<GLPeriod> periods = new List<GLPeriod>();
                //    var july = new GLPeriod
                //    {
                //        Name = "July-" + gLFiscalYear.DateFrom.Year,
                //        Index=1,
                //        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 7, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 7, 31),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(july);

                //    var august = new GLPeriod
                //    {
                //        Name = "August-" + gLFiscalYear.DateFrom.Year,
                //        Index = 2,
                //        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 8, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 8, 31),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(august);

                //    var september = new GLPeriod
                //    {
                //        Name = "September-" + gLFiscalYear.DateFrom.Year,
                //        Index = 3,
                //        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 9, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 9, 30),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(september);

                //    var october = new GLPeriod
                //    {
                //        Name = "October-" + gLFiscalYear.DateFrom.Year,
                //        Index = 4,
                //        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 10, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 10, 31),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(october);

                //    var november = new GLPeriod
                //    {
                //        Name = "November-" + gLFiscalYear.DateFrom.Year,
                //        Index = 5,
                //        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 11, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 11, 30),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(november);

                //    var december = new GLPeriod
                //    {
                //        Name = "December-" + gLFiscalYear.DateFrom.Year,
                //        Index = 6,
                //        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 12, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 12, 31),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(december);

                //    var january = new GLPeriod
                //    {
                //        Name = "January-" + gLFiscalYear.DateTo.Year,
                //        Index = 7,
                //        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 1, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 1, 31),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(january);

                //    var february = new GLPeriod
                //    {
                //        Name = "February-" + gLFiscalYear.DateTo.Year,
                //        Index = 8,
                //        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 2, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 2, 28 + leapyear),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(february);

                //    var march = new GLPeriod
                //    {
                //        Name = "March-" + gLFiscalYear.DateTo.Year,
                //        Index = 9,
                //        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 3, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 3, 31),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(march);

                //    var april = new GLPeriod
                //    {
                //        Name = "April-" + gLFiscalYear.DateTo.Year,
                //        Index = 10,
                //        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 4, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 4, 30),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(april);

                //    var may = new GLPeriod
                //    {
                //        Name = "May-" + gLFiscalYear.DateTo.Year,
                //        Index = 11,
                //        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 5, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 5, 31),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(may);

                //    var june = new GLPeriod
                //    {
                //        Name = "June-" + gLFiscalYear.DateTo.Year,
                //        Index = 12,
                //        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 6, 1),
                //        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 6, 30),
                //        Status = FiscalYearStatus.FuturePeriod,
                //        GlFiscalYearId = entity.GlFiscalYearId
                //    };
                //    periods.Add(june);



                //    foreach (var glPeriod in periods)
                //    {
                //        _db.GLPeriods.Add(glPeriod);
                //        _db.SaveChanges();
                //    }
                //    #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL fiscal year setup", "Success", User.Identity.Name, $"created fiscal year {entity.Name}", Modules.Finance);
                    #endregion
                
            }

            return Json(new[] { gLFiscalYear }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLFiscalYears_Update([DataSourceRequest]DataSourceRequest request, GlFiscalYear gLFiscalYear)
        {
            #region validation

            bool valid = true;
            var exists = _db.GLFiscalYears.Where(x => x.GlFiscalYearId != gLFiscalYear.GlFiscalYearId).Where(x => x.Name.ToUpper().Equals(gLFiscalYear.Name.ToUpper())).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("", gLFiscalYear.Name + " already exists in the system.");
                valid = false;
            }

            if (gLFiscalYear.DateFrom > gLFiscalYear.DateTo)
            {
                ModelState.AddModelError("", "Start Date should be before End Date.");
                valid = false;
            }

            var overlaps = _db.GLFiscalYears.Where(x => x.Name != gLFiscalYear.Name)
                                                .Where(x => x.DateFrom <= gLFiscalYear.DateTo)
                                                .Where(x => x.DateTo >= gLFiscalYear.DateFrom).ToList();
            if (overlaps.Any())
            {
                ModelState.AddModelError("", "The Fiscal Year's range overlaps with one or more previous Fiscal Years.");
                valid = false;
            }
            #endregion
            if (ModelState.IsValid)
            {
                
                var entity = new GlFiscalYear
                {
                    Name = gLFiscalYear.Name,
                    DateFrom = gLFiscalYear.DateFrom,
                    DateTo = gLFiscalYear.DateTo,
                    Status = gLFiscalYear.Status,

                    OrgBranchName = gLFiscalYear.OrgBranchName,
                    DateCreated = gLFiscalYear.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLFiscalYear.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                _db.GLFiscalYears.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL fiscal year setup", "Success", User.Identity.Name, $"updated fiscal year {entity.Name}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLFiscalYear }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLFiscalYears_Destroy([DataSourceRequest]DataSourceRequest request, GlFiscalYear gLFiscalYear)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlFiscalYear
                {
                    Name = gLFiscalYear.Name,
                    DateFrom = gLFiscalYear.DateFrom,
                    DateTo = gLFiscalYear.DateTo,
                    Status = gLFiscalYear.Status,

                    OrgBranchName = gLFiscalYear.OrgBranchName,
                    DateCreated = gLFiscalYear.DateCreated,
                    LastModified = gLFiscalYear.LastModified,
                    CreatedBy = gLFiscalYear.CreatedBy,
                    ModifiedBy = gLFiscalYear.ModifiedBy
                };

                _db.GLFiscalYears.Attach(entity);
                _db.GLFiscalYears.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLFiscalYear }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public JsonResult CloseFiscalYear(string id)
        {
           
            if (id != null)
            {
                int did = int.Parse(id);
                var periods = _db.GLPeriods.Where(a => a.GlFiscalYearId == did)
                    .Where(x => x.Status == FiscalYearStatus.Open || x.Status == FiscalYearStatus.FuturePeriod).ToList();
                if (periods.Any())
                {
                    var formattedData1 = new
                    {
                        Message = "There are periods not closed,please close them first.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }

                var year = _db.GLFiscalYears.Find(int.Parse(id));
                if (year != null && year.Status == FiscalYearStatus.Open)
                {
                    year.Status = FiscalYearStatus.Close;
                    _db.GLFiscalYears.Attach(year);
                    _db.Entry(year).State = EntityState.Modified;
                    _db.SaveChanges();
                       
                    #region  log

                    UserHelper.OperationLog(Request.UserHostAddress, "Fiscal Year close", "Success", User.Identity.Name,
                        "Fiscal year " + year.Name,Modules.Finance);

                    #endregion

                    var formattedData1 = new
                    {
                        Message = "Fiscal year closed successfully.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
            }

            var formattedData = new
            {
                Message = "Please select fiscal year to close.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult OpenFiscalYear(string id)
        {
            if (id != null)
            {
                var year = _db.GLFiscalYears.Find(int.Parse(id));
                if (year != null && year.Status != FiscalYearStatus.PermanetlyClosed)
                {
                    year.Status = FiscalYearStatus.Open;
                    _db.GLFiscalYears.Attach(year);
                    _db.Entry(year).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fiscal Year open", "Success", User.Identity.Name, "Fiscal year " + year.Name, Modules.Finance);
                    #endregion

                    var formattedData1 = new
                    {
                        Message = "Fiscal year open successfully.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }


            }

            var formattedData = new
            {
                Message = "Please select fiscal year to open.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ClosePeriod(string id)
        {
            ExceedDbContext _db = new ExceedDbContext();
            if (id != null)
            {
                var glPeriod = _db.GLPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status == FiscalYearStatus.Open)
                {
                    glPeriod.Status = FiscalYearStatus.Close;
                    _db.GLPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL Period Close", "Success", User.Identity.Name, "Period " + glPeriod.Name, Modules.Finance);
                    #endregion

                    var formattedData1 = new
                    {
                        Message = "Period closed successfully.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }


            }

            var formattedData = new
            {
                Message = "Please select period to xlose.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult OpenPeriod(string id)
        {
            ExceedDbContext _db = new ExceedDbContext();
            if (id != null)
            {
                var glPeriod = _db.GLPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status != FiscalYearStatus.PermanetlyClosed)
                {
                    glPeriod.Status = FiscalYearStatus.Open;
                    _db.GLPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL Period Open", "Success", User.Identity.Name, "Period " + glPeriod.Name, Modules.Finance);
                    #endregion

                    #region update ledger balances

                    //var jobId = BackgroundJob.Enqueue(() => _financeHelper.UpdateAllBegEndBalancesFast());

                    #endregion

                    var formattedData1 = new
                    {
                        Message = "Period open successfully.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }


            }

            var formattedData = new
            {
                Message = "Please select period to open.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult TransferEndingToNextYear(string id)
        {
            if (id != null)
            {
                int currentYearId = int.Parse(id);
                var year = _db.GLFiscalYears.Find(currentYearId);
                if (year != null && (year.Status == FiscalYearStatus.Close || year.Status == FiscalYearStatus.PermanetlyClosed))
                {
                    #region validations

                    var profittaxRate = _db.FinanceSettings.FirstOrDefault();
                    if (profittaxRate == null)
                    {
                        var formattedData = new
                        {
                            Message = "Profit Tax rate is not defined.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    var retainedAccount = _db.GlRetainedEarningAccounts.ToList();
                    if (!retainedAccount.Any())
                    {
                        var formattedData = new
                        {
                            Message = "Retained earning account setting is not defined.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    var retained = retainedAccount.Where(x => x.Type == EarningTypes.RetainedEarning).FirstOrDefault();

                    if (retained==null)
                    {
                        var formattedData = new
                        {
                            Message = "Retained earning account is not defined.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    var profitTax = retainedAccount.Where(x => x.Type == EarningTypes.ProfitTax).FirstOrDefault();

                    if (profitTax == null)
                    {
                        var formattedData = new
                        {
                            Message = "Profit tax account is not defined.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }


                    var beginningPeriod = _db.GlBeginningPeriods.FirstOrDefault();
                    if (beginningPeriod == null)
                    {
                        var formattedData = new
                        {
                            Message = "Beginning Period is not defined, define first.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    var begPeriodObj = _db.GLPeriods.Find(beginningPeriod.GLPeriodId);
                    if (begPeriodObj == null)
                    {
                        var formattedData = new
                        {
                            Message = "Beginning Period definition not found.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
                    int nextyearId = fiscalYearHelper.GetNextFYearId(currentYearId);
                    if (nextyearId == 0)
                    {
                        var formattedData = new
                        {
                            Message = "Next fiscal year is not created, create it first.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    int currentyearLastPeriod = fiscalYearHelper.GetFiscalYearLastPeriod(currentYearId);
                    if (currentyearLastPeriod == 0)
                    {
                        var formattedData = new
                        {
                            Message = "Selected fiscal year doest not have a period defined, Define periods first.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    var currentPeriodObj = _db.GLPeriods.Find(currentyearLastPeriod);
                    if (currentPeriodObj == null)
                    {
                        var formattedData = new
                        {
                            Message = "Selected fiscal year's Ending Period definition not found.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (currentPeriodObj.DateFrom<begPeriodObj.DateFrom)
                    {
                        var formattedData = new
                        {
                            Message = "The selected fiscal year is before the beginning period.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    int nextyearfirstPeriod = fiscalYearHelper.GetFiscalYearFirstPeriod(nextyearId);
                    if (nextyearfirstPeriod == 0)
                    {
                        var formattedData = new
                        {
                            Message = "Next fiscal year doest not have a period defined, Define periods first.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    //var financeSetting  = _db.FinanceSettings.FirstOrDefault();
                    //if (financeSetting == null || financeSetting?.RetainedEarningAccount==null || financeSetting?.RetainedEarningAccount == "")
                    //{
                    //    var formattedData = new
                    //    {
                    //        Message = "Retained Earning Account is not configured, Please configure it first.",
                    //        Status = "NOT_OK"
                    //    };
                    //    return Json(formattedData, JsonRequestBehavior.AllowGet);
                    //}
                    #endregion
                    FinanceHelper financeHelper = new FinanceHelper();
                    financeHelper.TransferFiscalYearEndingFast(currentyearLastPeriod, nextyearfirstPeriod, User.Identity.Name);
                    var transferjobId = BackgroundJob.Enqueue(() => financeHelper.UpdateAllBegEndBalancesFastP(nextyearfirstPeriod, "Automatic update after transfer ending"));
                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fiscal Year Ending transfer", "Success", User.Identity.Name, "Fiscal year " + year.Name, Modules.Finance);
                    #endregion

                    var formattedData1 = new
                    {
                        Message = "Fiscal year Ending Transfer Completed.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    
                    
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "The fiscal year is in "+year.Status.ToString()+" state, it should be closed or permanently closed to allow transfer.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                var formattedData = new
                {
                    Message = "Please select fiscal year to transfer from",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
