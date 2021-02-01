using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
      [Authorize(Roles = Payroll_Admin)]
    public class PayrollSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
          
            return View();
        }
       
        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Category == "Payroll");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new {
                ConfigurationID = configuration.ConfigurationId,
                Category = configuration.Category,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark,
               
                DateCreated = configuration.DateCreated,
                LastModified = configuration.LastModified,

                CreatedBy = configuration.CreatedBy,

                ModifiedBy = configuration.ModifiedBy 
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration,string Attribute, string CurrentValue)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var exist = db.Configurations
                    .Where(x => x.Category == "Payroll" && x.Reference==configuration.Reference && x.Attribute == Attribute).FirstOrDefault();
                if (exist != null)
                {
                    ModelState.AddModelError("", "This setting is already registered!!");
                }
                else
                {
                    var entity = new Configuration
                    {
                        Category = "Payroll",
                        Reference = configuration.Reference,
                        Attribute = Attribute,
                        CurrentValue = CurrentValue,
                        PreviousValue = configuration.PreviousValue,
                        Remark = configuration.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = configuration.LastModified,

                        CreatedBy = User.Identity.Name,

                        ModifiedBy = configuration.ModifiedBy
                    };

                    db.Configurations.Add(entity);
                    db.SaveChanges();
                    configuration.ConfigurationId = entity.ConfigurationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll settings", "Success", User.Identity.Name, $"added configuration reference {entity.Reference} attribute {entity.Attribute} current value {entity.CurrentValue} previous value {entity.PreviousValue}", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Category = configuration.Category,
                    Reference = configuration.Reference,
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark,
                    DateCreated = configuration.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = configuration.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Configurations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll settings", "Success", User.Identity.Name, $"updated configuration reference {entity.Reference} attribute {entity.Attribute} current value {entity.CurrentValue} previous value {entity.PreviousValue}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Destroy([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "You cannot delete this setting because it already processed!!");
                }
                else
                {
                    var entity = new Configuration
                    {
                        ConfigurationId = configuration.ConfigurationId,
                        Category = configuration.Category,
                        Reference = configuration.Reference,
                        Attribute = configuration.Attribute,
                        CurrentValue = configuration.CurrentValue,
                        PreviousValue = configuration.PreviousValue,
                        Remark = configuration.Remark,
                        DateCreated = configuration.DateCreated,
                        LastModified = configuration.LastModified,

                        CreatedBy = configuration.CreatedBy,

                        ModifiedBy = configuration.ModifiedBy
                    };
                    #region delete log
                    UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payroll setting information of setting " + configuration.Attribute,Modules.Payroll);
                    #endregion
                    db.Configurations.Attach(entity);
                    db.Configurations.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCascadeAttributes(string Reference)
        {
            List<string> attributes = new List<string>();
            switch (Reference)
            {


                case "OVERTIME":

                    attributes =
                          Enum.GetValues(typeof(OverTimeAttribute))
                              .Cast<OverTimeAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "PENSION":
                    attributes =
                          Enum.GetValues(typeof(PensionAttribute))
                              .Cast<PensionAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "PROVIDENT_FUND":
                    attributes =
                          Enum.GetValues(typeof(ProvidentAttribute))
                              .Cast<ProvidentAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "BONUS":
                    attributes =
                          Enum.GetValues(typeof(BonusAttribute))
                              .Cast<BonusAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "BENEFIT_ZERO_THRESHOLD":
                    attributes =
                          Enum.GetValues(typeof(BenefittAtribute))
                              .Cast<BenefittAtribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "APPLY_PROBATION_DAY_LENGTH":
                    attributes =
                        Enum.GetValues(typeof(BooleanList))
                            .Cast<BooleanList>()
                            .Select(v => v.ToString())
                            .ToList();
                    break;
                case "DISABLE_PENSION_FOR_CONTRACT":
                    attributes =
                        Enum.GetValues(typeof(BooleanList))
                            .Cast<BooleanList>()
                            .Select(v => v.ToString())
                            .ToList();
                    break;
                case "DEDUCT_LEAVE_WITHOUT_PAYMENT":
                    attributes =
                        Enum.GetValues(typeof(BooleanList))
                            .Cast<BooleanList>()
                            .Select(v => v.ToString())
                            .ToList();
                    break;

            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();

            attributes =
                  Enum.GetValues(typeof(PayrollSettingCategories))
                      .Cast<PayrollSettingCategories>()
                      .Select(v => v.ToString())
                      .ToList();


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
