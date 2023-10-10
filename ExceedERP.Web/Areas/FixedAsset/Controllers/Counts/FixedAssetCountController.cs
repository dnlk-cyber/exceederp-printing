using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Counts;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
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
            var businessUnits = _db.BusinessUnits;
            var stores = _db.InventoryStores;

            var categories = _db.FixedAssetCategories; 
            var subCategories = _db.FixedAssetSubCategories;
            var items = _db.AssetRegistrations;

            var fiscalYears = _db.GLFiscalYears;
            var periods = _db.GLPeriods;

            ViewData["Years"] = fiscalYears;
            ViewData["Periods"] = periods;

            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["SubCategory"] = subCategories;
           
            ViewData["Stores"] = stores;
            ViewData["branches"] = branches;
            ViewData["businessUnits"] = businessUnits;
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount,  FixedAssetConstants.FixedAssetCountApprover)]
        public ActionResult FixedAssetCounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<FixedAssetCount> fixedAssetCounts = _db.FixedAssetCounts;

            DataSourceResult result = fixedAssetCounts.ToDataSourceResult(request, fixedAssetCount => new
            {
                FixedAssetCountId = fixedAssetCount.FixedAssetCountId,
                CountDate = fixedAssetCount.CountDate,
                BranchId = fixedAssetCount.BranchId,
                BusinessUnitId = fixedAssetCount.BusinessUnitId,
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
            #region validation
            var exist = _db.FixedAssetCounts
                .Where(x => x.BusinessUnitId == fixedAssetCount.BusinessUnitId)
                .Where(x => x.BranchId == fixedAssetCount.BranchId)                
                .Where(x => x.GlFiscalYearId ==fixedAssetCount.GlFiscalYearId).AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Count already exists");
            }
            if (fixedAssetCount.GLPeriodId != 0)
            {
                var glPeriod = _db.GLPeriods.Find(fixedAssetCount.GLPeriodId);
                if (glPeriod != null)
                {
                    if (
                        !((fixedAssetCount.CountDate.Date >= glPeriod.DateFrom) &&
                          (fixedAssetCount.CountDate.Date <= glPeriod.DateTo)))
                        ModelState.AddModelError("",
                            $"Count Date should be within the period: " + glPeriod.Name + "(From " +
                            glPeriod.DateFrom.ToShortDateString() + " To " +
                            glPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }

            #endregion 

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCount
                {
                    CountDate =fixedAssetCount.CountDate.Date,
                    BranchId = fixedAssetCount.BranchId,
                    BusinessUnitId = fixedAssetCount.BusinessUnitId,
                    GlFiscalYearId = fixedAssetCount.GlFiscalYearId,
                    GLPeriodId = fixedAssetCount.GLPeriodId,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = fixedAssetCount.BranchId.ToString()
                };

                _db.FixedAssetCounts.Add(entity);
                _db.SaveChanges();
                fixedAssetCount.FixedAssetCountId = entity.FixedAssetCountId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Asset Count", "Success", User.Identity.Name, $"created asset Count {entity.FixedAssetCountId} Tag {entity.BusinessUnitId}", Modules.FixedAsset);
                #endregion
            }
                

            return Json(new[] { fixedAssetCount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCounts_Update([DataSourceRequest]DataSourceRequest request, FixedAssetCount fixedAssetCount)
        {
            #region
            var exist = _db.FixedAssetCounts.Where(x => x.FixedAssetCountId != fixedAssetCount.FixedAssetCountId)
                                .Where(x => x.BusinessUnitId == fixedAssetCount.BusinessUnitId)
                                .Where(x => x.BranchId == fixedAssetCount.BranchId)
                                .Where(x => x.GlFiscalYearId == fixedAssetCount.GlFiscalYearId).AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Count already exists");
            }
            if (fixedAssetCount.GLPeriodId != 0)
            {
                var glPeriod = _db.GLPeriods.Find(fixedAssetCount.GLPeriodId);
                if (glPeriod != null)
                {
                    if (
                        !((fixedAssetCount.CountDate.Date >= glPeriod.DateFrom) &&
                          (fixedAssetCount.CountDate.Date <= glPeriod.DateTo)))
                        ModelState.AddModelError("",
                            $"Count Date should be within the period: " + glPeriod.Name + "(From " +
                            glPeriod.DateFrom.ToShortDateString() + " To " +
                            glPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }

            #endregion 

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCount
                {
                    FixedAssetCountId = fixedAssetCount.FixedAssetCountId,
                    CountDate = fixedAssetCount.CountDate.Date,
                    BranchId = fixedAssetCount.BranchId,
                    BusinessUnitId = fixedAssetCount.BusinessUnitId,
                    GlFiscalYearId = fixedAssetCount.GlFiscalYearId,
                    GLPeriodId = fixedAssetCount.GLPeriodId,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                _db.FixedAssetCounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

              
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Asset Count", "Success", User.Identity.Name, $"updated asset Count {entity.FixedAssetCountId} Tag {entity.BusinessUnitId}", Modules.FixedAsset);
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
                    BusinessUnitId = fixedAssetCount.BusinessUnitId,

                };

                _db.FixedAssetCounts.Attach(entity);
                _db.FixedAssetCounts.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted property count number " + fixedAssetCount.FixedAssetCountId + " " + fixedAssetCount.Remark);
                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted asset Count {fixedAssetCount.FixedAssetCountId } and Tag {fixedAssetCount.BusinessUnitId}");
                #endregion
            }

            return Json(new[] { fixedAssetCount }.ToDataSourceResult(request, ModelState));
        }


      
        private bool GetStatus(int id)
        {
            var data = _db.FixedAssetCountItems
                .Where(a => a.FixedAssetCountId
                .Equals(id)).Count(x => x.Status == false);

            if (data > 0)
            {
                return false;
            }
            return true;

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        public JsonResult LoadAssets(int id)
        {            
            var record = _db.FixedAssetCounts.Find(id);
            
            if (record != null)
            {
                var assets = _db.AssetRegistrations
                    .Where(x => x.BranchId == record.BranchId).AsQueryable();
                if (GlobalHelper.IsMultipleBusinessUnit())
                {
                    assets= assets
                        .Where(x => x.BusinessUnitId == record.BusinessUnitId ||x.BusinessUnitId==0 || x.BusinessUnitId == null).AsQueryable();
                }

                var assetList= assets.ToList();
                foreach (var item in assetList)
                {
                    var exist=_db.FixedAssetCountItems
                        .Where(x=>x.FixedAssetCountId==id)
                        .Where(x => x.AssetRegistrationId == item.AssetRegistrationId).FirstOrDefault();
                    if (exist == null)
                    {
                        #region create count item
                        int empId = 0;
                        int.TryParse(item.EmployeeId,out empId);

                        var fixedAssetCountItem = new FixedAssetCountItem
                        {
                            FixedAssetCountId = id,
                            AssetRegistrationId = item.AssetRegistrationId,
                            FixedAssetCategoryId = item.FixedAssetCategoryID,
                            FixedAssetSubCategoryId = item.FixedAssetSubCategoryID,
                            Value = item.BookValue,
                            PreviousBalance = item.Quanity,
                            Count = item.Quanity,
                            Updated =false,
                            Status=false,
                            AssetStatus=item.Status,
                            EmployeeId= empId,
                            StoreCode=item.StoreCode,

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now
                        };
                      
                        _db.FixedAssetCountItems.Add(fixedAssetCountItem);
                        _db.SaveChanges();

                        #endregion
                    }

                }
                var formattedData = new
                {
                    Message = "Loading items completed.",
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
            var fixedAssetCountItems = _db.FixedAssetCountItems
                .Where(x => x.FixedAssetCountId == id)
                .ToList();

            if (record != null)
            {
                foreach (var item in fixedAssetCountItems)
                {
                    if (!item.Updated)
                    {
                        var assetRegistration = _db.AssetRegistrations
                            .FirstOrDefault(x => x.AssetRegistrationId == item.AssetRegistrationId);

                        if (assetRegistration != null)
                        {
                            assetRegistration.Quanity = (int)item.Count;
                            assetRegistration.Status = item.AssetStatus;
                          
                            item.Updated = true;
                            _db.Entry(item).State = EntityState.Modified;

                            _db.Entry(assetRegistration).State = EntityState.Modified;
                            _db.SaveChanges();
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
