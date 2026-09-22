using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using System.Data.Entity.Validation;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Hubs;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using System.IO;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    // [AuthorizeRoles(FleetManagementRoles.RentRequestor)] 
    public class RentFromExternalController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();
        IRentFromExternalService _RentFromExternalervice;

        public RentFromExternalController()
        {
            _RentFromExternalervice = new RentFromExternalService();

        }
        //
        // GET: /FleetManagement/RentFromExternal/
        public ActionResult Index()
        {
            GetViewDatas();
            var setting = db.RequestToInspections.FirstOrDefault();
            ViewBag.setting = setting != null ? setting.Remark : " ";
            ViewData["DistanceFrom"] =db.RentFromExternalEquipmentRates.Where(x=>x.RoadType==null).Select(s => new
            {
                value = s.DistanceFrom,
                text = s.DistanceFrom
            });
            ViewData["DistanceTo"] =db.RentFromExternalEquipmentRates.Where(x => x.RoadType == null).Select(s => new
            {
                value = s.DistanceTo,
                text = s.DistanceTo
            });
            ViewData["DistanceFromm"] =db.RentFromExternalEquipmentRates.Where(x=>x.DumpTruckTypeOfWork==null).Select(s => new
            {
                value = s.DistanceFrom,
                text = s.DistanceFrom
            });
            ViewData["DistanceToo"] =db.RentFromExternalEquipmentRates.Where(x => x.DumpTruckTypeOfWork == null).Select(s => new
            {
                value = s.DistanceTo,
                text = s.DistanceTo
            });
            ViewData["Branch"] = db.Branch.Select(x => new
            {
                Value = x.BranchId,
                Name = x.Name
            }).AsQueryable();
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            string isECWC = (fetchSetting != null && fetchSetting.Remark == "ECWC") ? "ECWC" : "Not";

            ViewData["IsECWC"] = isECWC;
            Refresh();
            return View();
        }

        public ActionResult RentHistory()
        {
            //ViewData["Operator"] = db.Operator.Select(s => new
            //{
            //    Value = s.OperatorID,
            //    Text = s.FullName
            //});
            //ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            //{
            //    Value = s.EquipmentIdentityID,
            //    Text = s.EquipmentCategoryName
            //}).ToList();

            //ViewData["Type"] = db.EquipmentType.Select(s => new
            //{
            //    Value = s.EquipmentTypeID,
            //    Text = s.EquipmentTypeName
            //}).ToList();

            //ViewData["EquipmentName"] = db.EquipmentName.Select(x => new { Value = x.EquipmentNameID, Text = x.Name }).ToList();
            //ViewData["Fleet"] = db.Fleets.Select(s => new
            //{
            //    Text = s.PlateNumber + " " + s.SerialNo,
            //    Value = s.FleetsID
            //}).AsQueryable();

            //ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).AsQueryable();
            GetViewDatas();
            
            return View();
        }

        private void GetViewDatas()
        {
            ViewData["Operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            });

            ViewData["Category"] = CacheData.GetEquipmentIdentityCaches().Select(
      i => new
      {
          Text = i.Name,
          Value = i.Id
      }).AsQueryable();
            ViewData["Type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID
            }).AsQueryable();
            ViewData["RentId"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.RentId
            }).AsQueryable();

            ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).AsQueryable();
        }
        private void Refresh()
        {
            var entity = db.RentFromExternal.Where(x => !x.IsReplaced && !x.IsExpired).Where(x => x.EndDate.Value < DateTime.Now).ToList();
            foreach (var item in entity)
            {
                item.IsExpired = true;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
        }
        public JsonResult RentFromExternal_Read([DataSourceRequest] DataSourceRequest request)
        {
            FleetCostHelper FleetCostHelper = new FleetCostHelper();
            IQueryable<RentFromExternal> RentFromExternal = db.RentFromExternal.Where(x => x.IsReplaced == false).AsQueryable();//.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

            DataSourceResult RentFromExternals = RentFromExternal.ToDataSourceResult(request, a => new RentFromExternal
            {
                IsExpired = a.IsExpired,
                EndDate = a.EndDate,
                Num = a.Num,
                RentFromExternalID = a.RentFromExternalID,
                AdvancePayment = a.AdvancePayment,
                BankFullAddress = a.BankFullAddress,
                BankSlipNo = a.BankSlipNo,
                CompanyAddress = a.CompanyAddress,
                CompanyName = a.CompanyName,
                CompanyEmail = a.CompanyEmail,
                CompanyPhno = a.CompanyPhno,
                Description = a.Description,
                EquipmentType = a.EquipmentType,
                FuelCoveredBy = a.FuelCoveredBy,
                NameBankBranch = a.NameBankBranch,
                ProjectLocation = a.ProjectLocation,
                Quantity = a.Quantity,
                RentalRateIncludingVAT = a.RentalRateIncludingVAT,
                Status = a.Status,
                UnitOfWorkMeasurement = a.UnitOfWorkMeasurement,
                StartDate = a.StartDate,
                DateDifference = a.EndDate.HasValue
                     ? (a.EndDate.Value.Date - DateTime.Today.Date).Days
                     : 0,
                IsOnlineApproved = a.IsOnlineApproved,
                IsSendForApproval = a.IsSendForApproval,
                DateCreated = a.DateCreated,
                LastModified = a.LastModified,
                CreatedBy = a.CreatedBy,
                ModifiedBy = a.ModifiedBy,
                ProcurementSupplierID = a.ProcurementSupplierID,
                TotalCost = a.TotalCost,
                PlateNo = GetPlateNo(a.RentFromExternalID),
                HP_CC = a.HP_CC,
                BranchId = a.BranchId,
                Hour = a.Hour,
                OperatorId = a.OperatorId,
                IsReplaced = a.IsReplaced,
                Operator = a.Operator

            });
            return Json(RentFromExternals, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RentFromExternalHistory_Read([DataSourceRequest] DataSourceRequest request)
        {
            FleetCostHelper FleetCostHelper = new FleetCostHelper();

            var RentFromExternal = _RentFromExternalervice.GetAll().Where(x => x.IsReplaced).ToList();//.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

            var RentFromExternals = RentFromExternal.Select(a => new RentFromExternal
            {
                Num = a.Num,
                RentFromExternalID = a.RentFromExternalID,
                AdvancePayment = a.AdvancePayment,
                BankFullAddress = a.BankFullAddress,
                BankSlipNo = a.BankSlipNo,
                CompanyAddress = a.CompanyAddress,
                CompanyName = a.CompanyName,
                CompanyEmail = a.CompanyEmail,
                CompanyPhno = a.CompanyPhno,
                Description = a.Description,
                EquipmentType = a.EquipmentType,
                FuelCoveredBy = a.FuelCoveredBy,
                NameBankBranch = a.NameBankBranch,
                ProjectLocation = a.ProjectLocation,
                Quantity = a.Quantity,
                RentalRateIncludingVAT = a.RentalRateIncludingVAT,
                Status = a.Status,
                UnitOfWorkMeasurement = a.UnitOfWorkMeasurement,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                DateDifference = (a.EndDate.Value.Date - DateTime.Today.Date).Days,
                IsOnlineApproved = a.IsOnlineApproved,
                IsSendForApproval = a.IsSendForApproval,
                DateCreated = a.DateCreated,
                LastModified = a.LastModified,
                CreatedBy = a.CreatedBy,
                ModifiedBy = a.ModifiedBy,
                ProcurementSupplierID = a.ProcurementSupplierID,
                TotalCost = a.TotalCost,
                PlateNo = GetPlateNo(a.RentFromExternalID),
                HP_CC = a.HP_CC,
                BranchId = a.BranchId,
                Hour = a.Hour,
                IsExpired = a.IsExpired,
                OperatorId = a.OperatorId,
                IsReplaced = a.IsReplaced,
                Operator = a.Operator

            }).ToList();
            return Json(RentFromExternals.ToDataSourceResult(request));
        }
        public string GetPlateNo(Guid id)
        {

            var fleet = db.Fleets.FirstOrDefault(x => x.RentId == id);
            if (fleet != null)
            {
                return fleet.PlateNumber ?? fleet.SerialNo;
            }
            return "Empty";
        }
        public JsonResult RentFromExternal_Create([DataSourceRequest] DataSourceRequest request, RentFromExternal RentFromExternal)
        {
            ModelState.Remove("RentFromExternalID");
            if (RentFromExternal.StartDate > RentFromExternal.EndDate)
            {
                ModelState.AddModelError("InvalidDate", " Start Date Must Be Greater Thhan  End Date");

            }
            else
            {
                RentFromExternal.RentFromExternalID = Guid.NewGuid();
                RentFromExternal.CreatedBy = User.Identity.Name;
                RentFromExternal.DateCreated = DateTime.Now;
                RentFromExternal.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                // _RentFromExternalervice.Insert(RentFromExternal);
                
                try
                {
                    db.RentFromExternal.Add(RentFromExternal);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    var sqlEx = ex.InnerException;

                    while (sqlEx.InnerException != null)
                    {
                        sqlEx = sqlEx.InnerException;
                    }

                    throw new Exception(sqlEx.Message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }
                if (RentFromExternal.EndDate != null)
                {
                    Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = RentFromExternal.EndDate.Value, Message = "Rent From External DeadLine On" + RentFromExternal.EndDate.Value.Date, Title = " Rent From External DeadLine ", URL = Url.Action("Index", "RentFromExternal", new { area = "FleetManagment" }) });
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rent from external", "Success", User.Identity.Name, $"created rent from external reg {RentFromExternal.RentFromExternalID}", Modules.FleetManagement);
                #endregion
            }



            return Json(new[] { RentFromExternal }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RentFromExternal_Update([DataSourceRequest] DataSourceRequest request, RentFromExternal model)
        {

            if (model.StartDate > model.EndDate)
            {
                ModelState.AddModelError("InvalidDate", " Start Date Must Be Greater Than  End Date");
            }
            else if (model.IsExpired)
            {
                var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == model.RentFromExternalID);
                rentFromExternal.StartDate = model.StartDate;
                rentFromExternal.EndDate = model.EndDate;
                rentFromExternal.ProcurementSupplierID = model.ProcurementSupplierID;
                rentFromExternal.ProjectLocation = model.ProjectLocation;
                rentFromExternal.CompanyAddress = model.CompanyAddress;
                rentFromExternal.RentalRateIncludingVAT = model.RentalRateIncludingVAT;
                rentFromExternal.FuelCoveredBy = model.FuelCoveredBy;
                rentFromExternal.AdvancePayment = model.AdvancePayment;
                rentFromExternal.HP_CC = model.HP_CC;
                rentFromExternal.Hour = model.Hour;
                rentFromExternal.OperatorId = model.OperatorId;
                rentFromExternal.LastModified = DateTime.Now;
                rentFromExternal.ModifiedBy = User.Identity.Name;
                rentFromExternal.OrgBranchName= UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                db.RentFromExternal.Attach(rentFromExternal);
                db.Entry(rentFromExternal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var entity = new RentFromExternal
                {
                    RentFromExternalID = Guid.NewGuid(),
                    AdvancePayment = rentFromExternal.AdvancePayment,
                    ProcurementSupplierID = rentFromExternal.ProcurementSupplierID,
                    ProjectLocation = rentFromExternal.ProjectLocation,
                    CompanyAddress = rentFromExternal.CompanyAddress,
                    StartDate = rentFromExternal.StartDate,
                    EndDate = rentFromExternal.EndDate,
                    FuelCoveredBy = rentFromExternal.FuelCoveredBy,
                    RentalRateIncludingVAT = rentFromExternal.RentalRateIncludingVAT,
                    Description = rentFromExternal.Description,
                    UnitOfWorkMeasurement = rentFromExternal.UnitOfWorkMeasurement,
                    Hour = rentFromExternal.Hour,
                    HP_CC = rentFromExternal.HP_CC,
                    BranchId = rentFromExternal.BranchId,
                    IsExpired = false,
                    IsReplaced = false,
                    ExpKey = rentFromExternal.RentFromExternalID,
                    OperatorId = rentFromExternal.OperatorId,
                    Operator = rentFromExternal.Operator
                };
                _RentFromExternalervice.Insert(entity);

                var equips = db.RentFromExternalEquipments.Where(x => x.RentFromExternalId == rentFromExternal.RentFromExternalID).ToList();
                if (equips != null)
                {
                    foreach (var equip in equips)
                    {
                        var rentEquip = new RentFromExternalEquipment
                        {
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Today,
                            EquipmentIdentityID = equip.EquipmentIdentityID,
                            EquipmentNameID = equip.EquipmentNameID,
                            EquipmentTypeID = equip.EquipmentTypeID,
                            FleetId = equip.FleetId,
                            IdleRentalRate = equip.IdleRentalRate,
                            RentalRate = equip.RentalRate,
                            RentType = equip.RentType,
                            RentFromExternalId = entity.RentFromExternalID
                        };
                        db.RentFromExternalEquipments.Add(rentEquip);
                        db.SaveChanges();
                        var rates = db.RentFromExternalEquipmentRates.Where(x => x.RentFromExternalEquipmentId == equip.RentFromExternalEquipmentId).ToList();

                        foreach (var item in rates)
                        {
                            var rate = new RentFromExternalEquipmentRate
                            {
                                RentType = item.RentType,
                                IdleRentalRate = item.IdleRentalRate,
                                RentalRate = item.RentalRate,
                                RentFromExternalEquipmentId = rentEquip.RentFromExternalEquipmentId,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Today
                            };
                            db.RentFromExternalEquipmentRates.Add(rate);
                            db.SaveChanges();
                        }                       
                    }
                }
                rentFromExternal.IsReplaced = true;
                
                db.Entry(rentFromExternal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Now;
                model.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _RentFromExternalervice.Update(model);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rent from external", "Success", User.Identity.Name, $"updated rent from external reg {model.RentFromExternalID}", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { model }.ToDataSourceResult(request));
        }

        public JsonResult checkIfExpired(Guid id)
        {
            var expiredRentals = db.RentFromExternal
                .SingleOrDefault(x => x.RentFromExternalID == id && x.EndDate < DateTime.Now);

            if (expiredRentals != null) // Ensure the record exists
            {
                expiredRentals.IsExpired = true;
                db.Entry(expiredRentals).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated successfully" });
            }

            return Json(new { success = false, message = "Rental not found or not expired" },JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RentFromExternal_Destroy([DataSourceRequest] DataSourceRequest request, RentFromExternal rentFromExternal)
        {
            var RentFromExternal = _RentFromExternalervice.GetById(rentFromExternal.RentFromExternalID);

            if (RentFromExternal != null)
            {
                _RentFromExternalervice.Delete(RentFromExternal);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Rent for External number " + rentFromExternal.RentFromExternalID);

            }

            return Json(new[] { RentFromExternal }.ToDataSourceResult(request));
        }


        public ActionResult RentFromExternalPrint(string id)
        {
            ViewBag.id = id;
            ViewBag.FormTitle = "Rent From External Print";
            ViewBag.FormNumber = 5;
            return View();
        }

        public ActionResult RentFromExternalDoc_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            var RentDoc = db.RentFromExternalDocument.Where(x => x.RentFromExternalId == id);
            return Json(RentDoc.ToDataSourceResult(request));
        }
        public ActionResult RentFromExternalDoc_Delete([DataSourceRequest]DataSourceRequest request, RentFromExternalDocument model)
        {
            var RentDoc = db.RentFromExternalDocument.FirstOrDefault(x => x.RentFromExternalDocumentId == model.RentFromExternalDocumentId);
            db.RentFromExternalDocument.Remove(RentDoc);
            db.SaveChanges();
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentFromExternalAttachment_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, string id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/App_Data/RentFromExternal/";
                string shortPath = "/App_Data/RentFromExternal/";

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in attachments)
                {
                    if (file != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                        }
                        if (file.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"File size exceeds the {MaxFileSize / (1024 * 1024)} MB limit.");
                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string physicalPath = Server.MapPath(subPath + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            file.SaveAs(physicalPath);
                            db.RentFromExternalDocument.Add(new RentFromExternalDocument()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                RentFromExternalId = id,
                                ShortPath = shortPath,
                                FilePath = physicalPath

                            });
                            db.SaveChanges();
                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "RentFromExternal attachment", "Success", User.Identity.Name, $"created attachement under po {id} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadRentFromExternalDocument(int CompanyDocumentId)
        {
            var record = db.RentFromExternalDocument.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public JsonResult UpdateRentedEquipments(string timeSheet)
        {
            var externalFleets = db.Fleets.Where(x => x.IsExternal && x.RentFromExternalNum != null).Select(x => x.FleetsID).ToList();
            if (timeSheet == "l")
            {
                var checkLightVehicles = db.LightVehicleTimeSheets.Where(x => externalFleets.Contains(x.FleetsID)).ToList();
                foreach (var lightVehicle in checkLightVehicles)
                {
                    if (lightVehicle.ProcurementSupplierID == null)
                    {
                        var Imediatevehilce = db.LightVehicleTimeSheets.Where(x => x.Date < lightVehicle.Date && x.ProcurementSupplierID != null && x.FleetsID == lightVehicle.FleetsID).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (Imediatevehilce != null)
                        {
                            lightVehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                            db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //var duplicatedTimeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID == lightVehicle.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(lightVehicle.Date)).ToList();
                    //if (duplicatedTimeSheets.Count > 1)
                    //{
                    //    var highestTimeSheet = duplicatedTimeSheets
                    //        .OrderByDescending(x => x.TotalTime)
                    //        .FirstOrDefault();
                    //    foreach (var sheet in duplicatedTimeSheets)
                    //    {
                    //        if (sheet.LightVehicleTimeSheetId != highestTimeSheet.LightVehicleTimeSheetId)
                    //        {
                    //            try
                    //            {
                    //                db.LightVehicleTimeSheets.Remove(sheet);
                    //                db.SaveChanges();
                    //            }
                    //            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    //            {
                    //                var sqlEx = ex.InnerException;

                    //                while (sqlEx.InnerException != null)
                    //                {
                    //                    sqlEx = sqlEx.InnerException;
                    //                }

                    //                throw new Exception(sqlEx.Message);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                System.Diagnostics.Debug.WriteLine(ex.ToString());
                    //                throw;
                    //            }
                    //        }
                    //    }
                    //}
                    if (lightVehicle.FinalKMR == 0 && lightVehicle.InitialKMR > 0)
                    {
                        lightVehicle.FinalKMR = lightVehicle.InitialKMR;
                        lightVehicle.DifferenceKMR = 0;
                        db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (lightVehicle.CostSheetApproved == false)
                    {
                        lightVehicle.TotalTime = lightVehicle.NetHour;
                        db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                        {
                            var sqlEx = ex.InnerException;

                            while (sqlEx.InnerException != null)
                            {
                                sqlEx = sqlEx.InnerException;
                            }

                            throw new Exception(sqlEx.Message);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                            throw;
                        }
                    }
                    if (lightVehicle.CostSheetApproved && lightVehicle.TotalTime == 0)
                    {
                        lightVehicle.TotalTime = lightVehicle.IdleHours + lightVehicle.NetHour;
                        db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            if (timeSheet == "m")
            {
                var checkMachineryVehicles = db.MachineryWorkingTimeSheets.Where(x => externalFleets.Contains(x.FleetsID)).ToList();
                foreach (var Vehicle in checkMachineryVehicles)
                {
                    if (Vehicle.ProcurementSupplierID == null)
                    {
                        var Imediatevehilce = db.MachineryWorkingTimeSheets.Where(x => x.Date < Vehicle.Date && x.ProcurementSupplierID != null && x.FleetsID == Vehicle.FleetsID).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (Imediatevehilce != null)
                        {
                            Vehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                            db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //var duplicatedTimeSheets = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == Vehicle.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(Vehicle.Date)).ToList();
                    //if (duplicatedTimeSheets.Count > 1)
                    //{
                    //    var highestTimeSheet = duplicatedTimeSheets
                    //        .OrderByDescending(x => x.TotalTime)
                    //        .FirstOrDefault();
                    //    foreach (var sheet in duplicatedTimeSheets)
                    //    {
                    //        if (sheet.MachineryWorkingTimeSheetId != highestTimeSheet.MachineryWorkingTimeSheetId)
                    //        {
                    //            db.MachineryWorkingTimeSheets.Remove(sheet);
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //}
                    if (Vehicle.FinalEHR == 0 && Vehicle.InitialEHR > 0)
                    {
                        Vehicle.FinalEHR = Vehicle.InitialEHR;
                        Vehicle.DifferenceEHR = 0;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (Vehicle.CostSheetApproved == false)
                    {
                        Vehicle.TotalTime = Vehicle.NetHour;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                    }
                    if (Vehicle.CostSheetApproved && Vehicle.TotalTime == 0)
                    {
                        Vehicle.TotalTime = Vehicle.IdleHours + Vehicle.NetHour;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (timeSheet == "d")
            {
                var checkDumpTruckVehicles = db.DumpTruckTimeSheets.Where(x => externalFleets.Contains(x.FleetsID)).ToList();
                foreach (var Vehicle in checkDumpTruckVehicles)
                {
                    if (Vehicle.ProcurementSupplierID == null)
                    {
                        var Imediatevehilce = db.DumpTruckTimeSheets.Where(x => x.Date < Vehicle.Date && x.ProcurementSupplierID != null && x.FleetsID == Vehicle.FleetsID).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (Imediatevehilce != null)
                        {
                            Vehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                            db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //var duplicatedTimeSheets = db.DumpTruckTimeSheets.Where(x => x.FleetsID == Vehicle.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(Vehicle.Date)).ToList();
                    //if (duplicatedTimeSheets.Count > 1)
                    //{
                    //    var highestTimeSheet = duplicatedTimeSheets
                    //        .OrderByDescending(x => x.TotalTime)
                    //        .FirstOrDefault();
                    //    foreach (var sheet in duplicatedTimeSheets)
                    //    {
                    //        if (sheet.DumpTruckTimeSheetId != highestTimeSheet.DumpTruckTimeSheetId)
                    //        {
                    //            db.DumpTruckTimeSheets.Remove(sheet);
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //}
                    if (Vehicle.FinalKMR == 0 && Vehicle.InitialKMR > 0)
                    {
                        Vehicle.FinalKMR = Vehicle.InitialKMR;
                        Vehicle.DifferenceKMR = 0;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (Vehicle.CostSheetApproved == false)
                    {
                        Vehicle.TotalTime = Vehicle.NetworkingHour;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (Vehicle.CostSheetApproved && Vehicle.TotalTime == 0)
                    {
                        Vehicle.TotalTime = Vehicle.IdleHour + Vehicle.NetworkingHour;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (timeSheet == "w")
            {
                var checkWagonDrillVehicles = db.WagonDrillTimesheets.Where(x => externalFleets.Contains(x.FleetsID)).ToList();
                foreach (var Vehicle in checkWagonDrillVehicles)
                {
                    if (Vehicle.ProcurementSupplierID == null)
                    {
                        var Imediatevehilce = db.WagonDrillTimesheets.Where(x => x.Date < Vehicle.Date && x.ProcurementSupplierID != null && x.FleetsID == Vehicle.FleetsID).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (Imediatevehilce != null)
                        {
                            Vehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                            db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //var duplicatedTimeSheets = db.WagonDrillTimesheets.Where(x => x.FleetsID == Vehicle.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(Vehicle.Date)).ToList();
                    //if (duplicatedTimeSheets.Count > 1)
                    //{
                    //    var highestTimeSheet = duplicatedTimeSheets
                    //        .OrderByDescending(x => x.TotalTime)
                    //        .FirstOrDefault();
                    //    foreach (var sheet in duplicatedTimeSheets)
                    //    {
                    //        if (sheet.WagonDrillTimesheetId != highestTimeSheet.WagonDrillTimesheetId)
                    //        {
                    //            db.WagonDrillTimesheets.Remove(sheet);
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //}
                    if (Vehicle.FinalEHR == 0 && Vehicle.InitialEHR > 0)
                    {
                        Vehicle.FinalEHR = Vehicle.InitialEHR;
                        Vehicle.DifferenceEHR = 0;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (Vehicle.CostSheetApproved == false)
                    {
                        Vehicle.TotalTime = Vehicle.OperationNetWorkingHour;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (Vehicle.CostSheetApproved && Vehicle.TotalTime == 0)
                    {
                        Vehicle.TotalTime = Vehicle.IdleHour + Vehicle.OperationNetWorkingHour;
                        db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (timeSheet == "o")
            {
                //var rentfromExternalIds = db.RentFromExternal.Where(x => x.IsOnlineApproved && !x.IsExpired).Select(x => x.RentFromExternalID);
                //var equipments = db.RentFromExternalEquipments.Where(x => rentfromExternalIds.Contains(x.RentFromExternalId)).ToList();
                //foreach (var equipment in equipments)
                //{
                //    equipment.IsOnlineApproved = true;
                //    db.Entry(equipment).State = System.Data.Entity.EntityState.Modified;

                //}
            }
            if(timeSheet=="dup")
            { 
            var fleets = db.Fleets.Where(x => x.IsExternal).ToList();
            var FleetsbyPlates = fleets.Where(x=>x.PlateNumber!=null).Select(x => x.PlateNumber).ToList();
            //var FleetsBySerials = fleets.Where(x=>x.PlateNumber==null&&x.SerialNo!=null).Select(x => x.SerialNo).ToList();
            //var FleetsByModels = fleets.Where(x=>x.PlateNumber==null&&x.SerialNo==null).Select(x => x.Model).ToList();
            List<string> DuplicatedPlates = FleetsbyPlates
                        .GroupBy(p => p)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();
                foreach (var plate in DuplicatedPlates)
                {
                    var duplicatedFleets = db.Fleets.Where(x => x.IsExternal && x.PlateNumber == plate).ToList();
                    if (duplicatedFleets.Count > 1)
                    {
                        var first = duplicatedFleets.First();
                        var timesheetType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID).TimeSheetType;
                        if (timesheetType == TimeSheetType.LightVehicle)
                        {
                            var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                            var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                            if (approvedFleet != null && notApprovedFleet != null)
                            {
                                var timeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                                foreach (var timesheet in timeSheets)
                                {
                                    timesheet.FleetsID = approvedFleet.FleetsID;
                                    db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                                }
                                db.SaveChanges();
                                db.Fleets.Remove(notApprovedFleet);
                                db.SaveChanges();
                            }
                        }
                        if (timesheetType == TimeSheetType.DumpTruck)
                        {
                            var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                            var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                            if (approvedFleet != null && notApprovedFleet != null)
                            {
                                var timeSheets = db.DumpTruckTimeSheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                                foreach (var timesheet in timeSheets)
                                {
                                    timesheet.FleetsID = approvedFleet.FleetsID;
                                    db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                                }
                                db.SaveChanges();
                                db.Fleets.Remove(notApprovedFleet);
                                db.SaveChanges();
                            }
                        }
                        if (timesheetType == TimeSheetType.Machinery)
                        {
                            var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                            var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                            if (approvedFleet != null && notApprovedFleet != null)
                            {
                                var timeSheets = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                                foreach (var timesheet in timeSheets)
                                {
                                    timesheet.FleetsID = approvedFleet.FleetsID;
                                    db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                                }
                                db.SaveChanges();
                                db.Fleets.Remove(notApprovedFleet);
                                db.SaveChanges();
                            }
                        }
                        if (timesheetType == TimeSheetType.WagonDrill)
                        {
                            var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                            var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                            if (approvedFleet != null && notApprovedFleet != null)
                            {
                                var timeSheets = db.WagonDrillTimesheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                                foreach (var timesheet in timeSheets)
                                {
                                    timesheet.FleetsID = approvedFleet.FleetsID;
                                    db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                                }
                                db.SaveChanges();
                                db.Fleets.Remove(notApprovedFleet);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Go to your timeSheet summary try to check if it is comming",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ArrangeFleetReferenceInTimeSheets()
        {
            
            var rentfromExternalIds = db.RentFromExternal.Where(x => x.IsOnlineApproved&&!x.IsExpired).Select(x => x.RentFromExternalID);
            var equipments = db.RentFromExternalEquipments.Where(x => rentfromExternalIds.Contains(x.RentFromExternalId)).ToList();
            foreach(var equipment in equipments)
            {
                equipment.IsOnlineApproved = true;
                db.Entry(equipment).State = System.Data.Entity.EntityState.Modified;
                
            }
            db.SaveChanges();
            var fleets = db.Fleets.Where(x => x.IsExternal).ToList();
            var FleetsbyPlates = fleets.Where(x=>x.PlateNumber!=null).Select(x => x.PlateNumber).ToList();
            //var FleetsBySerials = fleets.Where(x=>x.PlateNumber==null&&x.SerialNo!=null).Select(x => x.SerialNo).ToList();
            //var FleetsByModels = fleets.Where(x=>x.PlateNumber==null&&x.SerialNo==null).Select(x => x.Model).ToList();
            List<string> DuplicatedPlates = FleetsbyPlates
                        .GroupBy(p => p)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();
            foreach(var plate in DuplicatedPlates)
            {
                var duplicatedFleets = db.Fleets.Where(x => x.IsExternal && x.PlateNumber == plate).ToList();
                if(duplicatedFleets.Count>1)
                {
                    var first = duplicatedFleets.First();
                    var timesheetType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID).TimeSheetType;
                    if(timesheetType==TimeSheetType.LightVehicle)
                    {
                        var approvedFleet = db.Fleets.Where(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null).ToList();
                        //var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if(approvedFleet.Any()&&approvedFleet.Count>1)
                        {
                            var fleetIDS = approvedFleet.Select(x => x.FleetsID).Distinct().ToList();
                            var timeSheets = db.LightVehicleTimeSheets.Where(x => fleetIDS.Contains(x.FleetsID)).ToList();
                            var allAgreementNums = approvedFleet.Select(x => x.RentFromExternalNum).ToList();
                            int lastAgreementNum = db.RentFromExternal.Where(x=>allAgreementNums.Contains(x.Num)).OrderByDescending(x=>x.Num).FirstOrDefault().Num;
                            var latestAgreementFleet = db.Fleets.FirstOrDefault(x => x.RentFromExternalNum == lastAgreementNum && x.IsExternal == true);

                            foreach(var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = latestAgreementFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                           foreach(var fleet in approvedFleet)
                            {
                                if(latestAgreementFleet.FleetsID!=fleet.FleetsID)
                                {
                                    db.Fleets.Remove(fleet);
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                    if (timesheetType==TimeSheetType.DumpTruck)
                    {
                        var approvedFleet = db.Fleets.Where(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null).ToList();
                        //var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if (approvedFleet.Any() && approvedFleet.Count > 1)
                        {
                            var fleetIDS = approvedFleet.Select(x => x.FleetsID).Distinct().ToList();
                            var timeSheets = db.DumpTruckTimeSheets.Where(x => fleetIDS.Contains(x.FleetsID)).ToList();
                            var allAgreementNums = approvedFleet.Select(x => x.RentFromExternalNum).ToList();
                            int lastAgreementNum = db.RentFromExternal.Where(x => allAgreementNums.Contains(x.Num)).OrderByDescending(x => x.Num).FirstOrDefault().Num;
                            var latestAgreementFleet = db.Fleets.FirstOrDefault(x => x.RentFromExternalNum == lastAgreementNum && x.IsExternal == true);

                            foreach (var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = latestAgreementFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                            foreach (var fleet in approvedFleet)
                            {
                                if (latestAgreementFleet.FleetsID != fleet.FleetsID)
                                {
                                    db.Fleets.Remove(fleet);
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                    if (timesheetType==TimeSheetType.Machinery)
                    {
                        var approvedFleet = db.Fleets.Where(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null).ToList();
                        //var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if (approvedFleet.Any() && approvedFleet.Count > 1)
                        {
                            var fleetIDS = approvedFleet.Select(x => x.FleetsID).Distinct().ToList();
                            var timeSheets = db.MachineryWorkingTimeSheets.Where(x => fleetIDS.Contains(x.FleetsID)).ToList();
                            var allAgreementNums = approvedFleet.Select(x => x.RentFromExternalNum).ToList();
                            int lastAgreementNum = db.RentFromExternal.Where(x => allAgreementNums.Contains(x.Num)).OrderByDescending(x => x.Num).FirstOrDefault().Num;
                            var latestAgreementFleet = db.Fleets.FirstOrDefault(x => x.RentFromExternalNum == lastAgreementNum && x.IsExternal == true);

                            foreach (var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = latestAgreementFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                            foreach (var fleet in approvedFleet)
                            {
                                if (latestAgreementFleet.FleetsID != fleet.FleetsID)
                                {
                                    db.Fleets.Remove(fleet);
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                    if (timesheetType==TimeSheetType.WagonDrill)
                    {
                        var approvedFleet = db.Fleets.Where(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null).ToList();
                        //var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if (approvedFleet.Any() && approvedFleet.Count > 1)
                        {
                            var fleetIDS = approvedFleet.Select(x => x.FleetsID).Distinct().ToList();
                            var timeSheets = db.WagonDrillTimesheets.Where(x => fleetIDS.Contains(x.FleetsID)).ToList();
                            var allAgreementNums = approvedFleet.Select(x => x.RentFromExternalNum).ToList();
                            int lastAgreementNum = db.RentFromExternal.Where(x => allAgreementNums.Contains(x.Num)).OrderByDescending(x => x.Num).FirstOrDefault().Num;
                            var latestAgreementFleet = db.Fleets.FirstOrDefault(x => x.RentFromExternalNum == lastAgreementNum && x.IsExternal == true);

                            foreach (var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = latestAgreementFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                            foreach (var fleet in approvedFleet)
                            {
                                if (latestAgreementFleet.FleetsID != fleet.FleetsID)
                                {
                                    db.Fleets.Remove(fleet);
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                }
            }
            var formattedData = new
            {
                Message = "Go to your timeSheet summary try to check if it is comming",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult VoidAgreement(Guid id)
        {
            var record = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == id);
            var equipments = db.RentFromExternalEquipments.Where(x => x.RentFromExternalId == id).ToList();
            int count = 1;
            foreach(var equipment in equipments)
            {
                var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == equipment.EquipmentTypeID);
                if(equipmentType.TimeSheetType==TimeSheetType.DryLoading)
                {
                    var timeSheet = db.DryLoadingTimeSheets.FirstOrDefault(x => x.FleetsID == equipment.FleetId && x.RentalAgreementNum == record.Num);
                    if(timeSheet!=null)
                    {
                        count += 1;
                    }
                }
                if (equipmentType.TimeSheetType==TimeSheetType.LightVehicle)
                {
                    var timeSheet = db.LightVehicleTimeSheets.FirstOrDefault(x => x.FleetsID == equipment.FleetId && x.RentalAgreementNum == record.Num);
                    if(timeSheet!=null)
                    {
                        count += 1;
                    }
                }
                if (equipmentType.TimeSheetType==TimeSheetType.DumpTruck)
                {
                    var timeSheet = db.DumpTruckTimeSheets.FirstOrDefault(x => x.FleetsID == equipment.FleetId && x.RentalAgreementNum == record.Num);
                    if(timeSheet!=null)
                    {
                        count += 1;
                    }
                }
                if (equipmentType.TimeSheetType==TimeSheetType.WagonDrill)
                {
                    var timeSheet = db.WagonDrillTimesheets.FirstOrDefault(x => x.FleetsID == equipment.FleetId && x.RentalAgreementNum == record.Num);
                    if(timeSheet!=null)
                    {
                        count += 1;
                    }
                }
                if (equipmentType.TimeSheetType==TimeSheetType.Machinery)
                {
                    var timeSheet = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.FleetsID == equipment.FleetId && x.RentalAgreementNum == record.Num);
                    if(timeSheet!=null)
                    {
                        count += 1;
                    }
                }
            }
            if(count==1)
            {
                foreach(var fleet in equipments)
                {
                    var dbFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleet.FleetId);
                    dbFleet.RentFromExternalNum = null;
                    db.Entry(dbFleet).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if(count>1)
                {
                var formattedDataa = new
                {
                    Message = "You can not void this one timesheet is recorded",
                    Status = "OK"
                };
                return Json(formattedDataa, JsonRequestBehavior.AllowGet);
            }
            record.IsSendForApproval = false;
            record.IsOnlineApproved = false;
            record.IsVoid = true;
            db.Entry(record).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            var approval = db.RentFromExternalApproval.FirstOrDefault(x => x.RentFromExternalId == id);
            db.RentFromExternalApproval.Remove(approval);
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Void Successfully",
                Status = "OK"
            };
            return Json(formattedData,JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}