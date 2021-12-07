using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{

    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
    public class LeaveSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        string ErrorMessage = "";
        public ActionResult Index()
        {

            GenerateLeaveStartDateSetting();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["FisicalYear"] = db.GLFiscalYears.ToList();
            var LeaveSetting = Enum.GetValues(typeof(LeaveSetting))
                .Cast<LeaveSetting>()
                .Select(v => v.ToString())
                .ToList();
            var LeaveSettings = LeaveSetting.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LeaveSetting"] = LeaveSettings;
            ViewData["FiscalYeaer"] = db.GLFiscalYears.ToList();
        }
        public void GenerateLeaveStartDateSetting()
        {
            var leaveStartDate = db.LeaveEntitleStartingDates.FirstOrDefault();

            if (leaveStartDate == null)
            {
                var entity = new LeaveEntitleStartingDate
                {
                    Start = LeaveEntitleStarting.Fiscal_Year_End_Date,
                    Remark = "Default Setting"
                };

                db.LeaveEntitleStartingDates.Add(entity);
                db.SaveChanges();
            }
        }
        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Category == "Leave");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new
            {
                ConfigurationID = configuration.ConfigurationId,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark,
                DateCreated = configuration.DateCreated,
                LastModified = configuration.LastModified,

                CreatedBy = configuration.CreatedBy,

                ModifiedBy = configuration.ModifiedBy,
                //  GlFiscalYearId = configuration.GlFiscalYearId,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();

            var checkEntitleLimit = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());
            if (configuration.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString() &&
                (Convert.ToInt32(configuration.CurrentValue) > 12 || Convert.ToInt32(configuration.CurrentValue) < 0))
            {
                ModelState.AddModelError("", "you can't set value below 0 and above 12");
            }
            if (checkEntitleLimit != null && configuration.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString())
            {
                ModelState.AddModelError("", "you already set value for this setting");
            }

            if (configuration.Attribute == LeaveSetting.Old_Initial_Leave_End_Year.ToString())
            {
                var glfiscayear = db.GLFiscalYears.FirstOrDefault(x => x.Name.Trim() == configuration.CurrentValue.Trim());
                if (glfiscayear == null)
                {
                    ModelState.AddModelError("", "Check the year you write it must be from defined budget years");
                }
            }

            var checkexistance = db.Configurations.Where(x => x.Category == "Leave" && x.Reference == "Leave" && x.Attribute == configuration.Attribute).FirstOrDefault();
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "Leave Setting:- " + configuration.Attribute + " is already registered!!");
            }

            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    Category = "Leave",
                    Reference = "Leave",
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = configuration.LastModified,

                    CreatedBy = User.Identity.Name,

                    ModifiedBy = configuration.ModifiedBy,
                    // GlFiscalYearId = configuration.GlFiscalYearId
                };

                db.Configurations.Add(entity);
                db.SaveChanges();
                configuration.ConfigurationId = entity.ConfigurationId;

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Leave Setting", "Success", User.Identity.Name, "Leave Setting is Created By  " + User.Identity.Name, Modules.HRM);


                #endregion
            }


            return Json(new[] { configuration
    }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
            if (configuration.Attribute == LeaveSetting.Old_Initial_Leave_End_Year.ToString())
            {
                var glfiscayear = db.GLFiscalYears.FirstOrDefault(x => x.Name.Trim() == configuration.CurrentValue.Trim());
                if (glfiscayear == null)
                {
                    ModelState.AddModelError("", "Check the year you write it must be from defined budget years");
                }
            }
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Category = "Leave",
                    Reference = "Leave",
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark,
                    DateCreated = configuration.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = configuration.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    // GlFiscalYearId = configuration.GlFiscalYearId
                };

                db.Configurations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Leave Setting", "Success", User.Identity.Name, "Leave Setting is Updated By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
        private bool CheckLeaveValidation(int id)
        {
            ErrorMessage = "";


            #region leave
            var leave = db.Configurations.Where(x => x.ConfigurationId == id && x.Category == "Leave" && x.Reference == "Leave").FirstOrDefault();
            if (leave != null)
            {
                var entitle = db.LeavesEntitle;
                if (entitle.Any())
                {
                    ErrorMessage = "You cannot delete this Setting. It already processed by employee leave entitlment.";
                    return false;
                }

            }
            #endregion


            return true;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Destroy([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckLeaveValidation(configuration.ConfigurationId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new Configuration
                    {
                        ConfigurationId = configuration.ConfigurationId,
                        Category = "Leave",
                        Reference = "Leave",
                        Attribute = configuration.Attribute,
                        CurrentValue = configuration.CurrentValue,
                        PreviousValue = configuration.PreviousValue,
                        Remark = configuration.Remark,
                        DateCreated = configuration.DateCreated,
                        LastModified = configuration.LastModified,
                        CreatedBy = configuration.CreatedBy,
                        ModifiedBy = configuration.ModifiedBy,
                        // GlFiscalYearId = configuration.GlFiscalYearId,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted leave setting " + configuration.Attribute);
                    #endregion
                    db.Configurations.Attach(entity);
                    db.Configurations.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult CheckNewAwajDataIfExists()
        {
            var newAwaj = db.LeaveNewAwajStartYears.FirstOrDefault();
            if (newAwaj != null)
            {
                var mm = new
                {
                    Message = "OK"
                };

                return Json(mm);
            }

            var mm1 = new
            {
                Message = "FAIL"
            };

            return Json(mm1);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
