using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using static ExceedERP.Core.Domain.Global.GlobalEnum;
using GlobalEnum = ExceedERP.Core.Domain.Global.GlobalEnum;

namespace ExceedERP.Web.Controllers.Cmd
{
    public partial class CmdController : Controller
    {
        public static bool unlocked = false;
        
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

      

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SuperLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string cat = SecurityHelper.Encrypt("S", XAPI.Common.KEY);
            string reft = SecurityHelper.Encrypt("SP", XAPI.Common.KEY);
            string att = SecurityHelper.Encrypt("SPA", XAPI.Common.KEY);

            var secValue = db.Configurations
                .Where(c => c.Category ==cat )
                .Where(c => c.Reference == reft)
                .FirstOrDefault(c => c.Attribute ==att);

            if (secValue == null)
            {
                ModelState.AddModelError("Security","Security Token Not Found");
                return View(model);
            }

            if (SecurityHelper.Encrypt(model.Password, XAPI.Common.KEY) == secValue.CurrentValue)
                unlocked = true;

            if (unlocked)
                return RedirectToAction("Configurations");
            else
                return View(model);
        }

        public ActionResult Logout()
        {
            unlocked = false;
            return View("Success");
        }

        public ActionResult Configurations()
        {

            return View();
        }

        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Category == "S");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new {
                ConfigurationID = configuration.ConfigurationId,
                Category = configuration.Category,
                Reference = SecurityHelper.Decrypt(configuration.Reference,XAPI.Common.KEY),
                Attribute = SecurityHelper.Decrypt(configuration.Attribute,XAPI.Common.KEY),
                CurrentValue = SecurityHelper.Decrypt(configuration.CurrentValue,XAPI.Common.KEY),
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
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration, string attribute, string currentValue)
        {
            // ModelState.Clear();
            if (configuration.Reference == "AdminMode")
            {
                string reff = SecurityHelper.Encrypt("AdminMode",XAPI.Common.KEY);

                var adminMode = db.Configurations
                       .FirstOrDefault(x => x.Category == "S" && x.Reference ==reff );
                if (adminMode != null)
                {
                    ModelState.AddModelError("", "You can't have duplicate for Admin Mode!!");
                }
            }

            if (configuration.Reference == "ModuleAdmin")
            {
                
                var userExist = userDb.Users.FirstOrDefault(x => x.UserName == configuration.CurrentValue);

                if (userExist == null)
                {
                    ModelState.AddModelError("", "The user does not exist in the system");
                }
            }

            //string att = SecurityHelper.Encrypt(attribute, XAPI.Common.KEY);

            //var checkexistance = db.Configurations
            //         .FirstOrDefault(x => x.Category == "S" && x.Attribute ==att );
            //if (checkexistance != null)
            //{
            //    ModelState.AddModelError("", "This setting is already registered!!");
            //}

            if (ModelState.IsValid)
            {

                var entity = new Configuration
                {
                    Category = "S",
                    Reference =SecurityHelper.Encrypt(configuration.Reference,XAPI.Common.KEY),
                    Attribute = SecurityHelper.Encrypt(attribute,XAPI.Common.KEY),
                    CurrentValue = SecurityHelper.Encrypt(currentValue,XAPI.Common.KEY),
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
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            // ModelState.Clear();
            string reff = SecurityHelper.Encrypt("AdminMode", XAPI.Common.KEY);

            if (configuration.Reference == "AdminMode")
            {
                var adminMode = db.Configurations
                     .Where(x => x.ConfigurationId != configuration.ConfigurationId)
                       .FirstOrDefault(x => x.Category == "S" && x.Reference == reff );
                if (adminMode != null)
                {
                    ModelState.AddModelError("", "You can't have duplicate for AdminMode!!");
                }
            }

            if (configuration.Reference == "ModuleAdmin")
            {

                var userExist = userDb.Users.FirstOrDefault(x => x.UserName == configuration.CurrentValue);

                if (userExist == null)
                {
                    ModelState.AddModelError("", "The user does not exist in the system");
                }
            }

            //string att = SecurityHelper.Encrypt(configuration.Attribute, XAPI.Common.KEY);

            //var checkexistance = db.Configurations
            //    .Where(x=>x.ConfigurationId!= configuration.ConfigurationId)
            //         .FirstOrDefault(x => x.Category == "S" && x.Attribute ==att );
            //if (checkexistance != null)
            //{
            //    ModelState.AddModelError("", "This setting is already registered!!");
            //}

            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Category = configuration.Category,
                    Reference = SecurityHelper.Encrypt(configuration.Reference,XAPI.Common.KEY),
                    Attribute = SecurityHelper.Encrypt(configuration.Attribute,XAPI.Common.KEY),
                    CurrentValue = SecurityHelper.Encrypt(configuration.CurrentValue,XAPI.Common.KEY),
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted security setting information of setting " + configuration.Attribute);
                #endregion
                db.Configurations.Attach(entity);
                db.Configurations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCascadeAttributes(string reference)
        {
            List<string> attributes = new List<string>();
            switch (reference){


                case "AdminMode":

                    attributes =
                          Enum.GetValues(typeof(GlobalEnum.SuperSettingAdminMode))
                              .Cast<GlobalEnum.SuperSettingAdminMode>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;


                case "ModuleAdmin":
                    attributes =
                        Enum.GetValues(typeof(Modules))
                            .Cast<Modules>()
                            .Select(v => v.ToString())
                            .ToList();
                    break;

                case "BusinessUnit":
                    attributes =
                        Enum.GetValues(typeof(SuperSettingBusinessUnit))
                            .Cast<SuperSettingBusinessUnit>()
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
                  Enum.GetValues(typeof(GlobalEnum.SuperSettingCategories))
                      .Cast<GlobalEnum.SuperSettingCategories>()
                      .Select(v => v.ToString())
                      .ToList();


            return Json(attributes.Select(s => new
                { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class SuperLoginViewModel
    {       
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
       
    }
}