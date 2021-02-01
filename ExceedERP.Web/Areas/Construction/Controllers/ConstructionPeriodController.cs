using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class ConstructionPeriodController : Controller
    {
        // GET: Construction/ConstructionPeriod
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            var glperiods = _db.GLPeriods.ToList();
            foreach (var period in glperiods)
            {
                var exist = _db.ConstructionPeriod.FirstOrDefault(x => x.Name == period.Name);
                if (exist == null)
                {
                    var entity = new ConstructionPeriod
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
                    _db.ConstructionPeriod.Add(entity);
                    _db.SaveChanges();
                }

            }


            this.PrepareInitialSubPeriods();
            return View();
        }

        public ActionResult ConstructionPeriods_Read([DataSourceRequest]DataSourceRequest request)
        {


            IQueryable<ConstructionPeriod> constructionPeriods = _db.ConstructionPeriod;
            DataSourceResult result = constructionPeriods.ToDataSourceResult(request, gLPeriod => new
            {

                ConstructionPeriodId = gLPeriod.ConstructionPeriodId,
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConstructionPeriod_Create([DataSourceRequest]DataSourceRequest request, ConstructionPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionPeriod
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
                _db.ConstructionPeriod.Add(entity);
                _db.SaveChanges();
                period.ConstructionPeriodId = entity.ConstructionPeriodId;

            }
            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }


        public void ClosePeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.ConstructionPeriod.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status == FiscalYearStatus.Open)
                {
                    glPeriod.Status = FiscalYearStatus.Close;
                    _db.ConstructionPeriod.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region  log
                    //UserHelper.MiscLog(Request.UserHostAddress, "Inventory Period Close", "Success", User.Identity.Name, "Period " + glPeriod.Name);
                    #endregion
                }


            }

            //return RedirectToAction("Index");

        }

        public void OpenPeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.ConstructionPeriod.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status != FiscalYearStatus.PermanetlyClosed)
                {
                    glPeriod.Status = FiscalYearStatus.Open;
                    _db.ConstructionPeriod.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    //#region  log
                    //UserHelper.MiscLog(Request.UserHostAddress, "Inventory Period Open", "Success", User.Identity.Name, "Period " + glPeriod.Name);
                    //#endregion
                }


            }

            //return RedirectToAction("Index");

        }
        private void PrepareInitialSubPeriods()
        {
            var consPeriods = _db.ConstructionPeriod.ToList();
            if (consPeriods.Any())
            {
                foreach(ConstructionPeriod period in consPeriods)
                {
                    var subPeriods = _db.ConstructionSubPeriod.Where(s => s.ConstructionPeriodId == period.ConstructionPeriodId).ToList();
                    if (!subPeriods.Any())
                    {
                        try
                        {
                            var periodName = period.Name.Substring(0, 3);

                            TimeSpan difference = period.DateTo - period.DateFrom;
                            var numOfDays = difference.Days;
                            int firstNumOfDays = numOfDays / 2;
                            var firstSubPeriod = new ConstructionSubPeriod
                            {
                                DateFrom = period.DateFrom,
                                DateTo = period.DateFrom.AddDays(firstNumOfDays),
                                ConstructionPeriodId = period.ConstructionPeriodId,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                LastModified = null,
                                ModifiedBy = null,
                                OrgBranchName = null,
                                //Name = period.Name + 
                            };
                            //firstSubPeriod.Name = periodName + " " + firstSubPeriod.DateFrom.ToString("dd") + " to " + periodName + " " + firstSubPeriod.DateTo.ToString("dd") ;
                            firstSubPeriod.Name = firstSubPeriod.DateFrom.ToString("dd") + " to " + firstSubPeriod.DateTo.ToString("dd");

                            _db.ConstructionSubPeriod.Add(firstSubPeriod);
                            _db.SaveChanges();

                            var secondSubPeriod = new ConstructionSubPeriod
                            {
                                DateFrom = firstSubPeriod.DateTo.AddDays(1),
                                DateTo = period.DateTo,
                                ConstructionPeriodId = period.ConstructionPeriodId,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                LastModified = null,
                                ModifiedBy = null,
                                OrgBranchName = null,
                                //Name = period.Name + 
                            };
                            secondSubPeriod.Name =  secondSubPeriod.DateFrom.ToString("dd") + " to " + secondSubPeriod.DateTo.ToString("dd");
                            _db.ConstructionSubPeriod.Add(secondSubPeriod);
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}