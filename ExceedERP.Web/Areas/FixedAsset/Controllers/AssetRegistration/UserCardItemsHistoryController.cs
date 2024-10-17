using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Assetregistration
{

    public class UserCardItemsHistoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            var employee = db.Person_Employee.AsQueryable();
            ViewData["userCard"] = employee;

            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.EmployeeCode + " " +i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();

            ViewData["personEmployees"] = selectedEmployee;

            return View();
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult UserCardItems_Read([DataSourceRequest]DataSourceRequest request,string subcategory, string searchkey)
        {
            IQueryable<UserCardItems> usercarditems = db.UserCardItems.Where(x=>x.Active==false);
            if (!String.IsNullOrEmpty(subcategory))
            {
                int id = int.Parse(subcategory);
                usercarditems = usercarditems.Where(x => x.AssetRegistration.FixedAssetSubCategoryID == id);
            }
            if (!String.IsNullOrEmpty(searchkey))
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == searchkey);
                usercarditems = employee != null ? usercarditems.Where(x => x.Tag == searchkey || x.SerialNo == searchkey || x.EmployeeId == employee.EmployeeId) : usercarditems.Where(x => x.Tag == searchkey || x.SerialNo == searchkey);
            }
            DataSourceResult result = usercarditems.ToDataSourceResult(request, userCardItems => new
            {
                UserCardItemsID = userCardItems.UserCardItemsID,
                EmployeeId = userCardItems.EmployeeId,
                EmployeeFullName = userCardItems.EmployeeFullName,
                Name = userCardItems.Name,
                UOM = userCardItems.UOM,
                Quanity = userCardItems.Quanity,
                UnitPrice = userCardItems.UnitPrice,
                Tag = userCardItems.Tag,
                SerialNo = userCardItems.SerialNo,
                IssueDate = userCardItems.IssueDate,
                SIVNo = userCardItems.SIVNo,
                GRNDate = userCardItems.GRNDate,
                GRN = userCardItems.GRN,
                Remark = userCardItems.Remark,
                Active = userCardItems.Active,

                OrgBranchName = userCardItems.OrgBranchName,
                DateCreated = userCardItems.DateCreated,
                LastModified = userCardItems.LastModified,
                CreatedBy = userCardItems.CreatedBy,
                ModifiedBy = userCardItems.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserCardItems_Create([DataSourceRequest]DataSourceRequest request, UserCardItems userCardItems )
        {
                            
                #region validations
                if (userCardItems.Quanity <= 0)
                ModelState.AddModelError("qty", "Quantity should be positive number");

                //if (userCardItems.UnitPrice <= 0)
                //    ModelState.AddModelError("price", "Price should be positive number");
            #endregion
                var employee = db.Person_Employee.Find(userCardItems.EmployeeId);
                string fullName = "";
                if (employee != null)
                {
                    fullName = employee.FirstNameEnglish + "_" + employee.MiddleNameEnglish + "_" + employee.LastNameEnglish;
                }
                var asset = db.AssetRegistrations.Find(userCardItems.AssetRegistrationId);

                var exist = db.UserCardItems
                    .Where(x => x.EmployeeId == userCardItems.EmployeeId)
                    .Where(x => x.AssetRegistrationId == userCardItems.AssetRegistrationId).ToList();

                if (exist.Any())
                {
                    ModelState.AddModelError("exist","The item is already under the selected employee user card.");
                }

                if (ModelState.IsValid)
                {
                    if (asset != null)
                    {
                        var entity = new UserCardItems
                        {
                            AssetRegistrationId=asset.AssetRegistrationId,
                            Name = asset.Name,
                            UOM = userCardItems.UOM,
                            Quanity = userCardItems.Quanity,
                            UnitPrice = userCardItems.UnitPrice,
                            Tag = asset.Tag,
                            SerialNo = asset.SerialNo,
                            IssueDate = userCardItems.IssueDate,
                            SIVNo = userCardItems.SIVNo,
                            GRNDate = asset.GRNDate,
                            GRN = asset.GRN,
                            Remark = userCardItems.Remark,
                            EmployeeId = userCardItems.EmployeeId,
                            EmployeeFullName = fullName,
                            Active=false,

                            OrgBranchName = userCardItems.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = userCardItems.LastModified,
                            CreatedBy =User.Identity.Name,
                            ModifiedBy = userCardItems.ModifiedBy
                        };

                        db.UserCardItems.Add(entity);
                        db.SaveChanges();
                        userCardItems.UserCardItemsID = entity.UserCardItemsID;

                    //    #region update asset location

                    //    int branchId = UserInformationHelper.GetEmployeeBranchById(userCardItems.EmployeeId);

                    //    string user = User.Identity.Name;

                    //    FixedAssetHelper.UpdateLocationTracking(userCardItems, User.Identity.Name, branchId);

                    //#endregion
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "UserCard ", "Success", User.Identity.Name, $"created User card {entity.UserCardItemsID} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                    #endregion
                }
            }

              return Json(new[] { userCardItems }.ToDataSourceResult(request, ModelState));
            }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserCardItems_Update([DataSourceRequest]DataSourceRequest request, UserCardItems userCardItems)
        {
            #region validations
            if (userCardItems.Quanity <= 0)
                ModelState.AddModelError("qty", "Quantity should be positive number");

            if (userCardItems.UnitPrice <= 0)
                ModelState.AddModelError("price", "Price should be positive number");
            #endregion
            var employee = db.Person_Employee.Find(userCardItems.EmployeeId);

            string fullName = "";

            if (employee != null)
            {
                fullName = employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
            }

            var asset = db.AssetRegistrations.Find(userCardItems.AssetRegistrationId);

            var exist = db.UserCardItems.Where(x => x.UserCardItemsID != userCardItems.UserCardItemsID)
                                        .Where(x => x.EmployeeId == userCardItems.EmployeeId)
                                        .Where(x => x.AssetRegistrationId == userCardItems.AssetRegistrationId).ToList();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "The item is already under the selected employee user card.");
            }

            if (ModelState.IsValid)
            {
                if (asset != null)
                {
                    var entity = new UserCardItems
                    {
                  
                        UserCardItemsID = userCardItems.UserCardItemsID,
                        AssetRegistrationId = asset.AssetRegistrationId,
                        EmployeeId = userCardItems.EmployeeId,
                        EmployeeFullName = fullName,
                        Name = asset.Name,
                        UOM = userCardItems.UOM,
                        Quanity = userCardItems.Quanity,
                        UnitPrice = userCardItems.UnitPrice,
                        Tag = asset.Tag,
                        SerialNo = asset.SerialNo,
                        IssueDate = userCardItems.IssueDate,
                        SIVNo = userCardItems.SIVNo,
                        GRNDate = asset.GRNDate,
                        GRN = asset.GRN,
                        Remark = userCardItems.Remark,
                        Active=userCardItems.Active,

                        OrgBranchName = userCardItems.OrgBranchName,
                        DateCreated = userCardItems.DateCreated,
                        LastModified = DateTime.Now.Date,
                        CreatedBy = userCardItems.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.UserCardItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region update asset location

                   
                    int branchId = UserInformationHelper.GetEmployeeBranchById(userCardItems.EmployeeId);

                    string user = User.Identity.Name;

                    FixedAssetHelper.UpdateLocationTracking(userCardItems, user, branchId);

                    #endregion
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "UserCard ", "Success", User.Identity.Name, $"Updated User card {entity.UserCardItemsID} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                    #endregion
                }

            }

            return Json(new[] { userCardItems }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserCardItems_Destroy([DataSourceRequest]DataSourceRequest request, UserCardItems userCardItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new UserCardItems
                {
                    AssetRegistrationId = userCardItems.AssetRegistrationId,
                    EmployeeId = userCardItems.EmployeeId,
                    UserCardItemsID = userCardItems.UserCardItemsID,
                    Name = userCardItems.Name,
                    UOM = userCardItems.UOM,
                    Quanity = userCardItems.Quanity,
                    UnitPrice = userCardItems.UnitPrice,
                    Tag = userCardItems.Tag,
                    SerialNo = userCardItems.SerialNo,
                    IssueDate = userCardItems.IssueDate,
                    SIVNo = userCardItems.SIVNo,
                    GRNDate = userCardItems.GRNDate,
                    GRN = userCardItems.GRN,
                    Remark = userCardItems.Remark,

                    OrgBranchName = userCardItems.OrgBranchName,
                    DateCreated = userCardItems.DateCreated,
                    LastModified = userCardItems.LastModified,
                    CreatedBy = userCardItems.CreatedBy,
                    ModifiedBy = userCardItems.ModifiedBy
                };

                db.UserCardItems.Attach(entity);
                db.UserCardItems.Remove(entity);
                db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted UserCard {userCardItems.UserCardItemsID } and Tag {userCardItems.AssetRegistrationId}");
                #endregion
            }

            return Json(new[] { userCardItems }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
