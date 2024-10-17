using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{
    [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin)]
    public class FixedAssetPeriodController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            var glperiods = _db.GLPeriods.ToList();
            foreach (var period in glperiods)
            {
                var exist = _db.FixedAssetPeriods.FirstOrDefault(x => x.Name == period.Name);
                if (exist == null)
                {
                    var entity = new FixedAssetPeriod
                    {
                        Name = period.Name,
                        DateFrom = period.DateFrom,
                        DateTo = period.DateTo,
                        Status = period.Status,
                        OrgBranchName = period.OrgBranchName,
                        DateCreated = period.DateCreated,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy
                    };
                    _db.FixedAssetPeriods.Add(entity);
                    _db.SaveChanges();
                }

            }
            return View();
        }

        public ActionResult FixedAssetPeriods_Read([DataSourceRequest]DataSourceRequest request)
        {


            IQueryable<FixedAssetPeriod> fixedAssetPeriods = _db.FixedAssetPeriods;
            DataSourceResult result = fixedAssetPeriods.ToDataSourceResult(request, gLPeriod => new
            {
               
                FixedAssetPeriodId = gLPeriod.FixedAssetPeriodId,
                Name = gLPeriod.Name,
                DateFrom = gLPeriod.DateFrom,
                DateTo = gLPeriod.DateTo,
                Status = gLPeriod.Status,
                OrgBranchName = gLPeriod.OrgBranchName,
                DateCreated = gLPeriod.DateCreated,
                LastModified = gLPeriod.LastModified,
                CreatedBy = gLPeriod.CreatedBy,
                ModifiedBy = gLPeriod.ModifiedBy
            });

            return Json(result);
        }


        public ActionResult ClosePeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.FixedAssetPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status == FiscalYearStatus.Open)
                {
                    glPeriod.Status = FiscalYearStatus.Close;
                    _db.FixedAssetPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fixed Asset Period Close", "Success", User.Identity.Name, "Period " + glPeriod.Name,Modules.FixedAsset);
                    #endregion
                }


            }

            return RedirectToAction("Index");

        }

        public ActionResult OpenPeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.FixedAssetPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status != FiscalYearStatus.PermanetlyClosed)
                {
                    glPeriod.Status = FiscalYearStatus.Open;
                    _db.FixedAssetPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fixed Asset Period Open", "Success", User.Identity.Name, "Period " + glPeriod.Name, Modules.FixedAsset);
                    #endregion
                }


            }

            return RedirectToAction("Index");

        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
