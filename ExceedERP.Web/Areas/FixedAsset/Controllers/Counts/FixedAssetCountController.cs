using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Counts;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Counts
{

    public class FixedAssetCountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();


        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount,  FixedAssetConstants.FixedAssetCountApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var branches = _db.Branch;
            var stores = _db.InventoryStores;

            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var fiscalYears = _db.GLFiscalYears;
            var periods = _db.GLPeriods;

            ViewData["Years"] = fiscalYears;
            ViewData["periods"] = periods;

            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
            ViewData["Stores"] = stores;
            ViewData["branches"] = branches;
            ViewData["defaultCategory"] = branches.FirstOrDefault();
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount,  FixedAssetConstants.FixedAssetCountApprover)]
        public ActionResult FixedAssetCounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<FixedAssetCount> fixedAssetCounts = _db.FixedAssetCounts.Where(x => userStores.Contains(x.StoreCode));

            DataSourceResult result = fixedAssetCounts.ToDataSourceResult(request, fixedAssetCount => new
            {
                FixedAssetCountId = fixedAssetCount.FixedAssetCountId,
                CountDate = fixedAssetCount.CountDate,
                BranchId = fixedAssetCount.BranchId,
                StoreCode = fixedAssetCount.StoreCode,
                IsMidOfPeriod = fixedAssetCount.IsMidOfPeriod,
                GlFiscalYearId = fixedAssetCount.GlFiscalYearId,
                GLPeriodId = fixedAssetCount.GLPeriodId,
                IsClosed = fixedAssetCount.IsClosed,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCounts_Create([DataSourceRequest]DataSourceRequest request, FixedAssetCount fixedAssetCount)
        {
            var exist = _db.FixedAssetCounts.Where(x => x.BranchId == fixedAssetCount.BranchId)
                .Where(x => x.StoreCode == fixedAssetCount.StoreCode)
                .Where(x => x.CountDate ==fixedAssetCount.CountDate.Date).AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Count for date " + fixedAssetCount.CountDate + " and store " + fixedAssetCount.StoreCode + " already exists");
            }
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCount
                {
                    CountDate =fixedAssetCount.CountDate.Date,
                    BranchId = fixedAssetCount.BranchId,
                    StoreCode = fixedAssetCount.StoreCode,
                    GlFiscalYearId = fixedAssetCount.GlFiscalYearId,
                    GLPeriodId = fixedAssetCount.GLPeriodId,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = fixedAssetCount.BranchId.ToString()
                };

                _db.FixedAssetCounts.Add(entity);
                _db.SaveChanges();
                fixedAssetCount.FixedAssetCountId = entity.FixedAssetCountId;
            }

            #region disable store

            var store = _db.InventoryStores.Find(fixedAssetCount.StoreCode);
            if (store != null)
            {
                store.Enable = false;
                _db.Entry(store).State = EntityState.Modified;
                _db.SaveChanges();
            }

            #endregion
            return Json(new[] { fixedAssetCount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCounts_Update([DataSourceRequest]DataSourceRequest request, FixedAssetCount fixedAssetCount)
        {
            var exist = _db.FixedAssetCounts.Where(x => x.FixedAssetCountId != fixedAssetCount.FixedAssetCountId)
                            .Where(x => x.BranchId == fixedAssetCount.BranchId)
                            .Where(x => x.StoreCode == fixedAssetCount.StoreCode)
                            .Where(x => x.CountDate == fixedAssetCount.CountDate.Date).AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Count for date " + fixedAssetCount.CountDate + " and store " + fixedAssetCount.StoreCode + " already exists");
            }
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCount
                {
                    FixedAssetCountId = fixedAssetCount.FixedAssetCountId,
                    CountDate = fixedAssetCount.CountDate.Date,
                    BranchId = fixedAssetCount.BranchId,
                    StoreCode = fixedAssetCount.StoreCode,
                    GlFiscalYearId = fixedAssetCount.GlFiscalYearId,
                    GLPeriodId = fixedAssetCount.GLPeriodId,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                _db.FixedAssetCounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region disable store

                var store = _db.InventoryStores.Find(fixedAssetCount.StoreCode);
                if (store != null)
                {
                    store.Enable = false;
                    _db.Entry(store).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion

            }

            return Json(new[] { fixedAssetCount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCounts_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetCount fixedAssetCount)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCount
                {
                    FixedAssetCountId = fixedAssetCount.FixedAssetCountId,
                    CountDate = fixedAssetCount.CountDate,
                    BranchId = fixedAssetCount.BranchId,
                    StoreCode = fixedAssetCount.StoreCode,

                };

                _db.FixedAssetCounts.Attach(entity);
                _db.FixedAssetCounts.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted property count number " + fixedAssetCount.FixedAssetCountId + " " + fixedAssetCount.Remark);
                #endregion
            }

            return Json(new[] { fixedAssetCount }.ToDataSourceResult(request, ModelState));
        }

        private bool GetStatus(int id)
        {
            var data = _db.FixedAssetCountItems.Where(a => a.FixedAssetCountId.Equals(id)).Count(x => x.Status == false);
            if (data > 0)
            {
                return false;
            }
            return true;

        }

        public ActionResult FixedAssetCountPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCountApprover)]
        public JsonResult UpdateBalance(int id)
        {
            if (!GetStatus(id))
            {
                var formattedData0 = new
                {
                    Message = "There are items with status not updated.Please update them first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData0, JsonRequestBehavior.AllowGet);
            }
            var record = _db.FixedAssetCounts.Find(id);
            var fixedAssetCountItems = _db.FixedAssetCountItems.Where(x => x.FixedAssetCountId == id).ToList();
            if (record != null)
            {
                foreach (var item in fixedAssetCountItems)
                {
                    if (!item.Updated)
                    {
                        var assetRegistration = _db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == item.AssetRegistrationId);
                        if (assetRegistration != null)
                        {
                            assetRegistration.Quanity = (int)item.Count;
                            assetRegistration.Location = !String.IsNullOrWhiteSpace(item.Location)? item.Location:assetRegistration.Location;
                            item.Updated = true;
                            _db.Entry(item).State = EntityState.Modified;
                            _db.Entry(assetRegistration).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            #region create asset

                            assetRegistration = new AssetRegistration
                            {
                                StoreCode = record.StoreCode,
                                FixedAssetCategoryID = item.FixedAssetCategoryId,
                                FixedAssetSubCategoryID = item.FixedAssetSubCategoryId,
                                Quanity = (int)item.Count,
                                Location = item.Location,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            item.Updated = true;
                            _db.AssetRegistrations.Add(assetRegistration);
                            _db.SaveChanges();

                            #endregion
                           

                        }

                    }

                }
                var formattedData = new
                {
                    Message = "Updating items completed.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData2 = new
            {
                Message = "The count is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData2, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCountApprover)]
        public JsonResult ReverseUpdate(int id)
        {
            var record = _db.FixedAssetCounts.Find(id);
            var fixedAssetCountItems = _db.FixedAssetCountItems.Where(x => x.FixedAssetCountId == id).ToList();

            #region validate if all are updated

            foreach (var item in fixedAssetCountItems)
            {
                if (!item.Updated)
                {
                    var formattedData = new
                    {
                        Message = "There are items not update, please update before reversing.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }

            #endregion

            foreach (var item in fixedAssetCountItems)
            {
                var storeItem = _db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == item.AssetRegistrationId);
                if (storeItem != null)
                {
                    #region reverse store balance
                    storeItem.Quanity = (int)item.PreviousBalance;
                    item.Updated = false;
                    _db.Entry(item).State = EntityState.Modified;
                    _db.Entry(storeItem).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                }

            }
            if (record != null)
            {
                var referenceDoc = "Count on date " + record.CountDate.ToShortDateString();
                var reference = record.FixedAssetCountId.ToString();
                #region remove bin card
                var bincardLines = _db.BinCardLines
                    .Where(x => x.ReferenceDoc == referenceDoc && x.Reference == reference).ToList();
                _db.BinCardLines.RemoveRange(bincardLines);
                _db.SaveChanges();

                #endregion
            }
            var formattedData1 = new
            {
                Message = "Reversed Successfully.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeRoles(FixedAssetConstants.FixedAssetCountApprover)]
        public JsonResult CloseCount(int id)
        {
            
            var record = _db.FixedAssetCounts.Find(id);
            
            if (record != null)
            {
                var fixedAssetCountItems = _db.FixedAssetCountItems.Where(x => x.FixedAssetCountId == id).ToList();
                #region validate if all are updated

                foreach (var item in fixedAssetCountItems)
                {
                    if (!item.Updated)
                    {
                        var asset = _db.AssetRegistrations.Find(item.AssetRegistrationId);
                        var formattedData = new
                        {
                            Message = "There are items not update, please update before closing.\n " + asset?.Tag+ "-" + asset?.Name,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                #endregion

                record.IsClosed = true;
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
            }
           
           
            var formattedData1 = new
            {
                Message = "Count is closed Sucessfully.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
