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

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class GlFiscalYearController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
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
                        DateFrom = gLFiscalYear.DateFrom.Date,
                        DateTo = gLFiscalYear.DateTo.Date,
                        Status = FiscalYearStatus.Open,

                        OrgBranchName = gLFiscalYear.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = gLFiscalYear.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = gLFiscalYear.ModifiedBy
                    };

                    _db.GLFiscalYears.Add(entity);
                    _db.SaveChanges();
                    gLFiscalYear.GlFiscalYearId = entity.GlFiscalYearId;
                    //#region generate periods in the fiscal year with 30 days..ethiopian calendar

                    //List<GLPeriod> periods = new List<GLPeriod>();
                    //GLPeriod period;
                    //DateTime d1 = gLFiscalYear.DateFrom;
                    //DateTime d2 = gLFiscalYear.DateTo;
                    //DateTime d12 = d1;
                    //DateTime d22 = d12.AddDays(30);

                    //while (d22 <= d2)
                    //{
                    //    period = new GLPeriod
                    //    {
                    //        Name = d12.ToString("MMMM", CultureInfo.InvariantCulture) + "-" + d12.Year.ToString(),
                    //        DateFrom = d12,
                    //        DateTo = d22,
                    //        Status = FiscalYearStatus.Open,
                    //        GlFiscalYearId = entity.GlFiscalYearId
                    //    };
                    //    periods.Add(period);
                    //    //get next period
                    //    d12 = d22.AddDays(1);
                    //    d22 = d12.AddDays(30);
                    //}
                    //if (d22 > d2 && periods.Count < 12)
                    //{
                    //    period = new GLPeriod
                    //    {
                    //        Name = d12.ToString("MMMM", CultureInfo.InvariantCulture) + "-" + d12.Year.ToString(),
                    //        DateFrom = d12,
                    //        DateTo = d2,
                    //        Status = FiscalYearStatus.Open,
                    //        GlFiscalYearId = entity.GlFiscalYearId
                    //    };
                    //    periods.Add(period);
                    //}

                    //foreach (var glPeriod in periods)
                    //{
                    //    db.GLPeriods.Add(glPeriod);
                    //    db.SaveChanges();
                    //    var branchs = db.Branch.ToList();
                    //    foreach (var b in branchs)
                    //    {
                    //        var entityy = new PeriodStatus
                    //        {
                    //            GLPeriodId = glPeriod.GLPeriodId,
                    //            GLPeriod = glPeriod,
                    //            Status = PeriodStatusList.NEW,
                    //            BranchId = b.BranchId
                    //        };
                    //        db.PeriodStatuses.Add(entityy);
                    //        db.SaveChanges();
                    //    }


                    //}
                    //#endregion
                    #region generate periods in the fiscal year European July 1

                    int leapyear = 0;
                    if (DateTime.IsLeapYear(gLFiscalYear.DateTo.Year))
                        leapyear = 1;

                    List<GLPeriod> periods = new List<GLPeriod>();
                    var july = new GLPeriod
                    {
                        Name = "July-" + gLFiscalYear.DateFrom.Year,
                        Index=1,
                        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 7, 1),
                        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 7, 31),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(july);

                    var august = new GLPeriod
                    {
                        Name = "August-" + gLFiscalYear.DateFrom.Year,
                        Index = 2,
                        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 8, 1),
                        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 8, 31),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(august);

                    var september = new GLPeriod
                    {
                        Name = "September-" + gLFiscalYear.DateFrom.Year,
                        Index = 3,
                        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 9, 1),
                        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 9, 30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(september);

                    var october = new GLPeriod
                    {
                        Name = "October-" + gLFiscalYear.DateFrom.Year,
                        Index = 4,
                        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 10, 1),
                        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 10, 31),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(october);

                    var november = new GLPeriod
                    {
                        Name = "November-" + gLFiscalYear.DateFrom.Year,
                        Index = 5,
                        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 11, 1),
                        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 11, 30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(november);

                    var december = new GLPeriod
                    {
                        Name = "December-" + gLFiscalYear.DateFrom.Year,
                        Index = 6,
                        DateFrom = new DateTime(gLFiscalYear.DateFrom.Year, 12, 1),
                        DateTo = new DateTime(gLFiscalYear.DateFrom.Year, 12, 31),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(december);

                    var january = new GLPeriod
                    {
                        Name = "January-" + gLFiscalYear.DateTo.Year,
                        Index = 7,
                        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 1, 1),
                        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 1, 31),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(january);

                    var february = new GLPeriod
                    {
                        Name = "February-" + gLFiscalYear.DateTo.Year,
                        Index = 8,
                        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 2, 1),
                        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 2, 28 + leapyear),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(february);

                    var march = new GLPeriod
                    {
                        Name = "March-" + gLFiscalYear.DateTo.Year,
                        Index = 9,
                        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 3, 1),
                        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 3, 31),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(march);

                    var april = new GLPeriod
                    {
                        Name = "April-" + gLFiscalYear.DateTo.Year,
                        Index = 10,
                        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 4, 1),
                        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 4, 30),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(april);

                    var may = new GLPeriod
                    {
                        Name = "May-" + gLFiscalYear.DateTo.Year,
                        Index = 11,
                        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 5, 1),
                        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 5, 31),
                        Status = FiscalYearStatus.FuturePeriod,
                        GlFiscalYearId = entity.GlFiscalYearId
                    };
                    periods.Add(may);

                    var june = new GLPeriod
                    {
                        Name = "June-" + gLFiscalYear.DateTo.Year,
                        Index = 12,
                        DateFrom = new DateTime(gLFiscalYear.DateTo.Year, 6, 1),
                        DateTo = new DateTime(gLFiscalYear.DateTo.Year, 6, 30),
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
                    financeHelper.TransferFiscalYearEnding(currentyearLastPeriod, nextyearfirstPeriod, User.Identity.Name);
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
