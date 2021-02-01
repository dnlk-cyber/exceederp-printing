using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Controllers
{
      [Authorize(Roles = Admin)]
    public class GlobalSettingController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private const string Admin = "Admin";
        public ActionResult Index()
        {
          
            return View();
        }
       
        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = _db.Configurations.Where(c => c.Category == "Global");
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
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration,string attribute, string currentValue)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = _db.Configurations.FirstOrDefault(x => x.Category == "Global" && x.Attribute == attribute);
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "This setting is already registered!!");
                }
                else
                {
                    var entity = new Configuration
                    {
                        Category = "Global",
                        Reference = configuration.Reference,
                        Attribute = attribute,
                        CurrentValue = currentValue,
                        PreviousValue = configuration.PreviousValue,
                        Remark = configuration.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = configuration.LastModified,

                        CreatedBy = User.Identity.Name,

                        ModifiedBy = configuration.ModifiedBy
                    };

                    _db.Configurations.Add(entity);
                    _db.SaveChanges();
                    configuration.ConfigurationId = entity.ConfigurationId;
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

                _db.Configurations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Destroy([DataSourceRequest]DataSourceRequest request, Configuration configuration)
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
                    LastModified = configuration.LastModified,

                    CreatedBy = configuration.CreatedBy,

                    ModifiedBy = configuration.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payroll setting information of setting " + configuration.Attribute);
                #endregion
                _db.Configurations.Attach(entity);
                _db.Configurations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCascadeAttributes(string reference)
        {
            List<string> attributes = new List<string>();
            switch (reference)
            {


                case "BACKUP_AND_RESTORE":

                    attributes =
                          Enum.GetValues(typeof(GlobalEnum.GlobalSettingBackup))
                              .Cast<GlobalEnum.GlobalSettingBackup>()
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

            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();

            attributes =
                  Enum.GetValues(typeof(GlobalEnum.GlobalSettingCategories))
                      .Cast<GlobalEnum.GlobalSettingCategories>()
                      .Select(v => v.ToString())
                      .ToList();


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
