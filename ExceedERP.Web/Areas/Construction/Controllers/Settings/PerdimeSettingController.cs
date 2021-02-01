using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using System.Collections.Generic;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
      [Authorize(Roles = Payroll_Admin)]
    public class PerdimeSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            return View();
        }
        private bool CheckExistance(decimal Minmum, decimal Maximum)
        {
            var lookup = db.PerdimeSetting.Where(x => x.Minmum == Minmum && x.Maximum == Maximum).FirstOrDefault();
            if (lookup != null)
            {
                return true;
            }
            return false;
        }
        private bool InvalidMaxAndMin(decimal min, decimal max)
        {
            bool isInvalid = false;
            if(min >= max)
            {
                isInvalid = true;
            }
            return isInvalid;
        }
        private bool InvalidPerdiemAmount(decimal min, decimal max, decimal perdiem)
        {
            bool isInvalidAmount = false;
            if(perdiem > min)
            {
                isInvalidAmount = true;
            }
            return isInvalidAmount;
        }
        public ActionResult PerdimeSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PerdimeSetting> perdimesettings = db.PerdimeSetting;
            DataSourceResult result = perdimesettings.ToDataSourceResult(request, perdimeSetting => new {
                PerdimeSettingId = perdimeSetting.PerdimeSettingId,
                Minmum = perdimeSetting.Minmum,
                Maximum = perdimeSetting.Maximum,
                Amount = perdimeSetting.Amount,
                Remark = perdimeSetting.Remark,
                 DateCreated = perdimeSetting.DateCreated,
                LastModified = perdimeSetting.LastModified,
                CreatedBy = perdimeSetting.CreatedBy,
                ModifiedBy = perdimeSetting.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerdimeSetting_Create([DataSourceRequest]DataSourceRequest request, PerdimeSetting perdimeSetting)
        {
            //ModelState.Clear();
            if (ModelState.IsValid)
            {
                var modelMinimum = perdimeSetting.Minmum;
                var modelMaximum = perdimeSetting.Maximum;
                var min = GetMinimumOfAll();
                var max = GetMaxOfAll();

                if (CheckExistance(perdimeSetting.Minmum, perdimeSetting.Maximum))
                {
                    ModelState.AddModelError("", "This per diem setting range is already registered");
                }
                else if (InvalidMaxAndMin(perdimeSetting.Minmum, perdimeSetting.Maximum))
                {
                    ModelState.AddModelError("", "Invalid Minimum and Maximum Combination");
                }
                else if (modelMinimum < min && modelMaximum >= min) {
                    ModelState.AddModelError("", "Invalid Range");
                }
                else if(modelMaximum > max && modelMinimum <= max)
                {
                    ModelState.AddModelError("", "Invalid Range");
                }
                else if (InvalidPerdiemAmount(perdimeSetting.Minmum, perdimeSetting.Maximum, perdimeSetting.Amount))
                {
                    ModelState.AddModelError("", "Per diem amount is too large");
                }
                else
                {
                    var entity = new PerdimeSetting
                    {
                        Minmum = perdimeSetting.Minmum,
                        Maximum = perdimeSetting.Maximum,
                        Amount = perdimeSetting.Amount,
                        Remark = perdimeSetting.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = perdimeSetting.LastModified,

                        CreatedBy = User.Identity.Name,

                        ModifiedBy = perdimeSetting.ModifiedBy
                    };

                    db.PerdimeSetting.Add(entity);
                    db.SaveChanges();
                    perdimeSetting.PerdimeSettingId = entity.PerdimeSettingId;
                }
            }

            return Json(new[] { perdimeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerdimeSetting_Update([DataSourceRequest]DataSourceRequest request, PerdimeSetting perdimeSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (InvalidMaxAndMin(perdimeSetting.Minmum, perdimeSetting.Maximum))
                {
                    ModelState.AddModelError("", "Invalid Minimum and Maximum Combination");
                }

                if (CheckExistance(perdimeSetting.Minmum, perdimeSetting.Maximum))
                {
                    ModelState.AddModelError("", "This per diem setting range is already registered");
                }
                else
                {
                    var entity = new PerdimeSetting
                    {
                        PerdimeSettingId = perdimeSetting.PerdimeSettingId,
                        Minmum = perdimeSetting.Minmum,
                        Maximum = perdimeSetting.Maximum,
                        Amount = perdimeSetting.Amount,
                        Remark = perdimeSetting.Remark,
                        DateCreated = perdimeSetting.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = perdimeSetting.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.PerdimeSetting.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
            }

            return Json(new[] { perdimeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerdimeSetting_Destroy([DataSourceRequest]DataSourceRequest request, PerdimeSetting perdimeSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PerdimeSetting
                {
                    PerdimeSettingId = perdimeSetting.PerdimeSettingId,
                    Minmum = perdimeSetting.Minmum,
                    Maximum = perdimeSetting.Maximum,
                    Amount = perdimeSetting.Amount,
                    Remark = perdimeSetting.Remark,
                    DateCreated = perdimeSetting.DateCreated,
                    LastModified = perdimeSetting.LastModified,
                    CreatedBy = perdimeSetting.CreatedBy,
                    ModifiedBy = perdimeSetting.ModifiedBy 
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  perdime setting information of setting " + perdimeSetting.Amount);
                #endregion
                db.PerdimeSetting.Attach(entity);
                db.PerdimeSetting.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { perdimeSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private decimal GetMinimumOfAll()
        {
            var allValues = db.PerdimeSetting.ToList();
            var minimums = new HashSet<decimal>(allValues.Select(p => p.Minmum));
            var minimum = minimums.Min();
            return minimum;
        }
        private decimal GetMaxOfAll()
        {
            var allValues = db.PerdimeSetting.ToList();
            var maximums = new HashSet<decimal>(allValues.Select(p => p.Maximum));
            var max = maximums.Max();
            return max;
        }
    }
}
