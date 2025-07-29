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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    // [AuthorizeRoles(FleetManagementRoles.RentRequestor)] 
    public class RentFromExternalController : Controller
    {
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
            ViewData["Operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            });
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();

            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["EquipmentName"] = db.EquipmentName.Select(x => new { Value = x.EquipmentNameID, Text = x.Name }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID
            }).ToList();

            ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            Refresh();
            return View();
        }

        public ActionResult RentHistory()
        {
            ViewData["Operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            });
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();

            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["EquipmentName"] = db.EquipmentName.Select(x => new { Value = x.EquipmentNameID, Text = x.Name }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID
            }).ToList();

            ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            
            return View();
        }
        private void Refresh()
        {
            var entity = db.RentFromExternal.Where(x => !x.IsReplaced && !x.IsExpired).Where(x => x.EndDate.Value < DateTime.Now).ToList();
            foreach (var item in entity)
            {
                item.IsExpired = true;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }
            //entity.ForEach(e => e.IsExpired = true);
            //db.Entry(RentFromExternal).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        public JsonResult RentFromExternal_Read([DataSourceRequest] DataSourceRequest request)
        {
            FleetCostHelper FleetCostHelper = new FleetCostHelper();
      
          
            var RentFromExternal = _RentFromExternalervice.GetAll().Where(x => x.IsReplaced == false).ToList();//.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));
 
            var RentFromExternals = RentFromExternal.Select(a => new RentFromExternal
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
                WorkingCapacity = a.WorkingCapacity,
                StartDate = a.StartDate,
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
                OperatorId = a.OperatorId,
                IsReplaced = a.IsReplaced,
                Operator = a.Operator

            }).ToList();
            return Json(RentFromExternals.ToDataSourceResult(request));
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
                WorkingCapacity = a.WorkingCapacity,
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

                _RentFromExternalervice.Insert(RentFromExternal);
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
                rentFromExternal.WorkingCapacity = model.WorkingCapacity;
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
                    WorkingCapacity = rentFromExternal.WorkingCapacity,
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

            return Json(new { success = false, message = "Rental not found or not expired" });
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

    }
}