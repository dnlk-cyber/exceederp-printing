using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Reporting.Fleet.ViewModel;
using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.InspectionUser, FleetManagementRoles.InspectionApprover)]

    public class AnnualEquipmentInventoryInspectionController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        IAnnualEquipmentInventoryInspectionService _AnnualEquipmentInventoryInspectionService;
        public AnnualEquipmentInventoryInspectionController()
        {
            _AnnualEquipmentInventoryInspectionService = new AnnualEquipmentInventoryInspectionService();
        }     
        public ActionResult Index()
        {
            ViewData["StatusKey"] = db.InspectionStatusKeys.Select(x => new
            {

                Value = x.InspectionStatusKeyId,
                Name = x.StatusKey
            }).ToList();
            
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

            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            }).ToList();

            return View();
        }


        public ActionResult AnnualInspectionlog_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var AnnualEquipmentInventoryInspection = db.InspectionLogs.Where(x => x.AnnualEquipmentInventoryInspectionID == Id);

            var result = AnnualEquipmentInventoryInspection
                        .Select(model => new 
                        {
                            InspectionLogId = model.InspectionLogId,
                            DateCreated = model.DateCreated,
                            IsSendForApproval = model.IsSendForApproval,
                            IsOnlineApproved = model.IsOnlineApproved,
                            OrgBranchName = model.OrgBranchName,
                            AnnualEquipmentInventoryInspectionID = model.AnnualEquipmentInventoryInspectionID,
                            CheckList = model.CheckList,
                            StatusKey = model.StatusKey,
                            SystemName = model.SystemName,   
                            CreatedBy = model.CreatedBy,
                            IsOnlineTransferred = model.IsOnlineTransferred,
                            IsOnlineTransferredBy = model.IsOnlineTransferredBy,
                            IsVoid = model.IsVoid,
                            StatusKeyId = model.StatusKeyId,
                            LastModified = model.LastModified,
                            ModifiedBy = model.ModifiedBy,
                            OnlineApprovedBy = model.OnlineApprovedBy,
                            OnlineApprovedTime = model.OnlineApprovedTime,
                            OnlineTransferredTime = model.OnlineTransferredTime,
                            Remark = model.Remark,
                            SendForApprovalBy = model.SendForApprovalBy,
                            SendForApprovalTime = model.SendForApprovalTime,
                            Status = model.Status,
                            VoidBy = model.VoidBy,
                          
                            VoidTime = model.VoidTime
                        });

            return Json(result.ToDataSourceResult(request)); 
        }


        public ActionResult InspectionLog_Update([DataSourceRequest] DataSourceRequest request, InspectionLog model)
        {
            if (model != null)
            {
                model.LastModified = DateTime.Now;
                model.ModifiedBy = User.Identity.Name;
                model.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "InspectionLogId", "Success", User.Identity.Name, $"updated InspectionLog reg {model.InspectionLogId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request));
        }

        public ActionResult AnnualEquipmentInventoryInspection_Read([DataSourceRequest] DataSourceRequest request)
        {
            var AnnualEquipmentInventoryInspection = _AnnualEquipmentInventoryInspectionService.GetAll().Select(model => new AnnualEquipmentInventoryInspection
            {
                DateofInspection = model.DateofInspection,
                FleetsID = model.FleetsID,
                IsOnlineApproved = model.IsOnlineApproved,
                IsSendForApproval = model.IsSendForApproval,
                Status = model.Status,
                CountryOfOrigin = model.CountryOfOrigin,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                EngineModel = model.EngineModel,
                EngineSerailNumber = model.EngineSerailNumber,
                EquipmentCategoryId = model.EquipmentCategoryId,
                EquipmentNameId = model.EquipmentNameId,
                EquipmentTypeId = model.EquipmentTypeId,
                Make = model.Make,
               // StatuKeyId = model.StatuKeyId,
                ModelNumber = model.ModelNumber,
                SerialNumber = model.SerialNumber,
                Operator = model.Operator,
                YearOfManufacturing = model.YearOfManufacturing,
                InspectedBy = model.InspectedBy,
                IsOnlineTransferred = model.IsOnlineTransferred,
                IsOnlineTransferredBy = model.IsOnlineTransferredBy,
                IsVoid = model.IsVoid,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                OnlineApprovedBy = model.OnlineApprovedBy,
                OnlineApprovedTime = model.OnlineApprovedTime,
                OnlineTransferredTime = model.OnlineTransferredTime,
                SendForApprovalBy = model.SendForApprovalBy,
                SendForApprovalTime = model.SendForApprovalTime,
                AnnualEquipmentInventoryInspectionID = model.AnnualEquipmentInventoryInspectionID
                

            });

            return Json(AnnualEquipmentInventoryInspection.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualEquipmentInventoryInspection_Create([DataSourceRequest] DataSourceRequest request, AnnualEquipmentInventoryInspection model)
        {
            if (model != null)
            {
                model.AnnualEquipmentInventoryInspectionID = Guid.NewGuid();
                model.Status =MaintenanceConst.Pending;
                model.CreatedBy = User.Identity.Name;
                model.InspectedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _AnnualEquipmentInventoryInspectionService.Insert(model);
                GenerateInspection(model.EquipmentCategoryId, model.EquipmentTypeId, model.AnnualEquipmentInventoryInspectionID);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Annual equipment inventory inspection", "Success", User.Identity.Name, $"created Annual equipment inventory inspection {model.AnnualEquipmentInventoryInspectionID} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualEquipmentInventoryInspection_Update([DataSourceRequest] DataSourceRequest request, AnnualEquipmentInventoryInspection model)
        {
            if (model != null)
            {
                model.LastModified = DateTime.Now;
                model.ModifiedBy = User.Identity.Name;
                model.InspectedBy = User.Identity.Name;
                model.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                var inspectionLogs = db.InspectionLogs.Where(x => x.AnnualEquipmentInventoryInspectionID == model.AnnualEquipmentInventoryInspectionID).ToList();
                db.InspectionLogs.RemoveRange(inspectionLogs);
                db.SaveChanges();


                _AnnualEquipmentInventoryInspectionService.Update(model);
                GenerateInspection(model.EquipmentCategoryId, model.EquipmentTypeId, model.AnnualEquipmentInventoryInspectionID);


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Annual Equipment Inventory Inspection", "Success", User.Identity.Name, $"updated AnnualEquipmentInventoryInspection reg {model.AnnualEquipmentInventoryInspectionID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request));
        }
        public ActionResult AnnualEquipmentInventoryInspection_Delete([DataSourceRequest] DataSourceRequest request, AnnualEquipmentInventoryInspection model)
        {
            if (model != null)
            {

                var entity = db.AnnualEquipmentInventoryInspection.FirstOrDefault(x => x.AnnualEquipmentInventoryInspectionID == model.AnnualEquipmentInventoryInspectionID);
                db.AnnualEquipmentInventoryInspection.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Annual Equipment Inventory Inspection", "Success", User.Identity.Name, $"updated AnnualEquipmentInventoryInspection reg {model.AnnualEquipmentInventoryInspectionID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request));
        }

        private void GenerateInspection(Guid equipmentIdentityId, Guid equipmentTypeId, Guid AnnualEquipmentInventoryInspectionID)
        {
            FleetCheckList fleetCheckList = new FleetCheckList();
            fleetCheckList = db.FleetCheckLists.FirstOrDefault(x => x.EquipmentTypeID == equipmentTypeId);
            if (fleetCheckList != null)
            {
                var fleetLineCheckLists = db.FleetLineCheckLists.Where(x => x.FleetCheckListId == fleetCheckList.FleetCheckListId).ToList();
                if (fleetLineCheckLists.Any())
                {
                    foreach (var fleetLineCheckList in fleetLineCheckLists)
                    {
                        InspectionLog inspectionLog = new InspectionLog
                        {
                            CheckList = fleetLineCheckList.CheckList,
                            SystemName = fleetLineCheckList.SystemName,
                            AnnualEquipmentInventoryInspectionID = AnnualEquipmentInventoryInspectionID,
                        };

                        db.InspectionLogs.Add(inspectionLog);
                    }

                    db.SaveChanges();
                }

            }
            else
            {
                fleetCheckList = db.FleetCheckLists.FirstOrDefault(x => x.EquipmentIdentityID == equipmentIdentityId);
                if (fleetCheckList != null)
                {
                    var fleetLineCheckLists = db.FleetLineCheckLists.Where(x => x.FleetCheckListId == fleetCheckList.FleetCheckListId).ToList();
                    if (fleetLineCheckLists.Any())
                    {
                        foreach (var fleetLineCheckList in fleetLineCheckLists)
                        {
                            InspectionLog inspectionLog = new InspectionLog
                            {
                                CheckList = fleetLineCheckList.CheckList,
                                SystemName = fleetLineCheckList.SystemName,
                                AnnualEquipmentInventoryInspectionID = AnnualEquipmentInventoryInspectionID,

                            };

                            db.InspectionLogs.Add(inspectionLog);
                        }

                        db.SaveChanges();
                    }
                }
            }

        }

        public ActionResult SendForOnlineApproval(Guid id)
        {
            var annualEquipmentInspection = db.AnnualEquipmentInventoryInspection.FirstOrDefault(x => x.AnnualEquipmentInventoryInspectionID == id);
            if(annualEquipmentInspection != null)
            {
                annualEquipmentInspection.IsSendForApproval = true;
                annualEquipmentInspection.IsOnlineTransferredBy = User.Identity.Name;
                db.Entry(annualEquipmentInspection).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);
            }
            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Approval_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {

            var approval = db.InspectionApprovals.Where(x => x.InspectionId == id.ToString());
            DataSourceResult result = approval.ToDataSourceResult(request, model => new InspectionApproval
            {
                InspectionId = model.InspectionId,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                InspectionApprovalId = model.InspectionApprovalId,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                Remark = model.Remark,
                IsApproved = model.IsApproved
            });
            return Json(result);
        }
        public ActionResult Approval_Create([DataSourceRequest] DataSourceRequest request, InspectionApproval model, Guid id)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == MaintenanceConst.Approved)
                {
                    Approved(id);
                    model.Status = MaintenanceConst.Approved;
                    model.IsApproved = true;
                }
                else
                {
                    Rejected(id);
                    model.Status = MaintenanceConst.Rejected;
                    model.IsApproved = true;
                }
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.InspectionId = id.ToString();
                db.InspectionApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        private void Approved(Guid id)
        {
            var entity = db.AnnualEquipmentInventoryInspection.Find(id);
            if (entity != null)
            {
                entity.IsOnlineApproved = true;
                entity.Status = MaintenanceConst.Approved;
                entity.OnlineApprovedBy = User.Identity.Name;
                entity.OnlineApprovedTime = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(Guid id)
        {
            var entity = db.AnnualEquipmentInventoryInspection.Find(id);
            if (entity != null)
            {
                entity.IsOnlineApproved = false;
                entity.Status = MaintenanceConst.Rejected;
                
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        public ActionResult AnnualEquipmentInventoryInspectionlog_Read([DataSourceRequest] DataSourceRequest request)
        {
            var logs = db.CheckListLogs.ToList();
            return Json(logs.ToDataSourceResult(request));
        }

    }
}
