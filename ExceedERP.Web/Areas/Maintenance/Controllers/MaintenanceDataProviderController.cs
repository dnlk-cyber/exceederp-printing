
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class MaintenanceDataProviderController : Controller
    {

        public const string SparePart = "1001";
        public const string Tool = "1002";
        public const string Operator = "Operator";
        public const string Mechanic = "Mechanic";

        ExceedDbContext db = new ExceedDbContext();


        public JsonResult GetBusinessUnit()
        {
            var businessUnits = db.BusinessUnits;
            return Json(businessUnits.Select(s => new
            {
                Value = s.BusinessUnitId,
                Text = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranchForMaintenanceCenter()
        {
            var branches = db.Branch;
            return Json(branches.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentIdentities()
        {
            var equipIdentities = db.EquipmentIdentity.Select(
                i => new
                {
                    Id = i.EquipmentIdentityID,
                    Name = i.EquipmentCategoryName
                }).ToList();
            return Json(equipIdentities, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMechanicTypes()
        {
            var mechanicTypes = db.MechanicTypeClasses.AsQueryable();
            var result  = mechanicTypes.Select(s => new
            {
                Value = s.MechanicTypeClassId,
                Title = s.MechanicTypeTitle
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllMechaniCrew(string text)
        {
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = db.Person_Employee
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateWorkShopStatus(MechanicType? mechanicType)
        {
            var jobOrders = db.JobOrder.Where(q => q.IsOnlineTransferred).AsQueryable();
            foreach(var jobOrder in jobOrders)

                {
                jobOrder.IsSubmittedToServiceCenter = true;
                jobOrder.IsSentToWorkShop = true;

                db.JobOrder.Attach(jobOrder);
                db.Entry(jobOrder).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();

            return Json("Updated Succefully!", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateServiceCenterStatus()
        {
            var jobOrders = db.JobOrder.Where(q => q.Status =="Open").AsQueryable();
            foreach (var jobOrder in jobOrders)

            {
                jobOrder.IsSubmittedToServiceCenter = false;
                jobOrder.IsSentToWorkShop = false;
                jobOrder.IsOnlineTransferred = false;
                jobOrder.IsSecondOnlineApproved = false;

                db.JobOrder.Attach(jobOrder);
                db.Entry(jobOrder).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();

            return Json("Updated Succefully!", JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMechanics(MechanicType? mechanicType,string text)
        {
            var mechanics = db.MechanicCrew.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                mechanics=mechanics
                 .Where(x=>x.Person_Employee.EmployeeCode.Contains(text)
                || x.Person_Employee.FirstNameEnglish.ToUpper().Contains(text.ToUpper())
                || x.Person_Employee.MiddleNameEnglish.ToUpper().Contains(text.ToUpper())
                || x.Person_Employee.LastNameEnglish.ToUpper().Contains(text.ToUpper())
                ).AsQueryable();
            }
            if (mechanicType != null)

            {
                mechanics = mechanics.Where(q => q.MechanicType == mechanicType).AsQueryable();
            }

           var list= mechanics.Select(s => new
             {
                 Code = s.MechanicCrewID,
                 s.MechanicType,
                 Name = s.Person_Employee.EmployeeCode + " " + s.Person_Employee.FirstNameEnglish + " " + s.Person_Employee.MiddleNameEnglish + " " + s.Person_Employee.LastNameEnglish
             }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubCategories(Guid? jobOrderId,string filterText)
        {
           
            var subcategories = db.RepairType.Select(s => new { s.SubCategoryCode,
                s.RepairTypeID
            })
                .Distinct()
                .ToList();
            IEnumerable<string> subCategoryIds;
            if (jobOrderId != null)
            {

                var repairsTypes = db.JobOrderServiceCenterDetails.Where(q => q.ServiceCenterJobId == jobOrderId)
                    .Select(s => s.RepairTypeID);
                 subCategoryIds= subcategories.Where(q => repairsTypes.Contains(q.RepairTypeID)).Select(s=>s.SubCategoryCode);
            }
            else
            {
                 subCategoryIds = subcategories.Select(s=>s.SubCategoryCode);
            }
            var serviceSubcategories = db.ItemCategorys.Where(s => subCategoryIds.Contains(s.ItemCategoryCode))
                .Select(s => new {
                Code = s.ItemCategoryCode,
                Name = s.Name
                })
                .ToList();

            if (!string.IsNullOrEmpty(filterText))
            {
                serviceSubcategories = serviceSubcategories.Where(l => l.Name.ToLower().Contains(filterText.ToLower()) || l.Code.Contains(filterText)).ToList();
            }
            return Json(serviceSubcategories, JsonRequestBehavior.AllowGet);

        }
       
        public JsonResult GetServiceCategoryEnums(string subCategoryCode, Guid? jobOrderId,string filterText)
        {
           
            var enumCategories = db.RepairType.Where(q=>q.SubCategoryCode == subCategoryCode)
                .Select(s => new {
                    Code =  s.ServiceGroup,
                    Name = s.ServiceGroup.ToString()
                } ).Distinct().ToList();
            if (jobOrderId != null)
            {
                var repairTypes = db.JobOrderServiceCenterDetails.Where(q => q.ServiceCenterJobId == jobOrderId).Select(s => s.ServiceGroup);
                enumCategories = enumCategories.Where(q => repairTypes.Contains(q.Code)).Distinct().ToList();

            }
            if (!string.IsNullOrEmpty(filterText))
            {
                enumCategories = enumCategories.Where(l => l.Name.ToLower().Contains(filterText.ToLower()) || l.Code.ToString().Contains(filterText)).ToList();
            }
            return Json(enumCategories, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetServices(string subCategoryCode , ServiceGroup ServiceGroup, Guid? jobOrderId,string filterText)
        {
            var repairTypes = db.RepairType.Where(q=>q.SubCategoryCode == subCategoryCode&&q.ServiceGroup == ServiceGroup)
                .Select(s => new {
                    Code = s.RepairTypeID,
                    Name = s.Code +"-"+s.Name

                })
                .ToList();
            if (jobOrderId != null)
            {
                var jobOrderDetails = db.JobOrderServiceCenterDetails.Where(q => q.ServiceCenterJobId == jobOrderId)
                    .Select(s=>new{
                        Code= s.JobOrderServiceCenterDetailId,
                        Name= s.RepairType.Code+"-"+ s.RepairType.Name
                    }).ToList();

                if (!string.IsNullOrEmpty(filterText))
                {
                    jobOrderDetails = jobOrderDetails.Where(l => l.Name.ToLower().Contains(filterText.ToLower()) || l.Code.ToString().Contains(filterText)).ToList();
                }
                return Json(jobOrderDetails, JsonRequestBehavior.AllowGet);

            }

            if (!string.IsNullOrEmpty(filterText))
            {
                repairTypes = repairTypes.Where(l => l.Name.ToLower().Contains(filterText.ToLower()) || l.Code.ToString().Contains(filterText)).ToList();
            }
            return Json(repairTypes, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ServiceCenterUderMaintenanceCenter(string maintenanceCenterId)
        {

            
            Guid maintenanceId;
            Guid.TryParse(maintenanceCenterId, out maintenanceId);
            if (!string.IsNullOrWhiteSpace(maintenanceCenterId))
            {
                var entity = db.ServiceCenters.Where(x => x.MaintenanceCenterId == maintenanceId).Select(s => new
                {
                    Name = s.Name,
                    Id = s.ServiceCenterId
                });
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
           

            return Json(null);

        }
        public JsonResult getServiceCenterInServiceCenter(int serviceCenterId)
        {

            if (serviceCenterId != 0)
            {
                var entity = db.ServiceCenters.Where(x => x.ServiceCenterId == serviceCenterId).Select(s => new
                {
                    Name = s.Name,
                    Id = s.ServiceCenterId
                });
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }
        public JsonResult GetServiceDetail(Guid id)
        {

            //var repairTypeId = db.JobOrderDetail.Find(id)?.RepairTypeID;
            //var service = db.RepairType.Find(repairTypeId);
            //     if(service!=null)
            //{
            //    return Json(new { service.AutomotiveStandaredHour }, JsonRequestBehavior.AllowGet);

            //}
            //return Json(new {  }, JsonRequestBehavior.AllowGet);
            var repairTypeId = db.JobOrderServiceCenterDetails.Find(id)?.RepairTypeID;
            var service = db.RepairType.Find(repairTypeId);
            if (service != null)
            {
                return Json(new { service.AutomotiveStandaredHour }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetJobCategoriesPerJobOrder(Guid jobOrderId)
        //{
        //    var jobServices = db.JobOrderDetail.Where(q => q.JobOrderID == jobOrderId)
        //        .ToList()
        //         .Select(s => 
        //             s.SubCategoryCode
        //         );
        //    var subCategories = db.ItemCategorys.Where(q => jobServices.Contains(q.ItemCategoryCode))
        //        .Select(s=>new {
        //            s.ItemCategoryCode,
        //            s.Name,

        //        });
        //    return Json(subCategories, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetJobServiceGroupPerJobOrder(Guid jobOrderId)
        //{
        //    var jobServices = db.JobOrderDetail.Where(q => q.JobOrderID == jobOrderId)
        //         .Select(s => new
        //          {
        //              Code = s.JobOrderDetailID,
        //              Name = s.RepairType.Name + "-" + s.JobCauses.Name
        //          }).ToList();
        //    return Json(jobServices, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetJobServicePerJobOrder(Guid jobOrderId)
        {
            var jobServices = db.JobOrderDetail.Where(q => q.JobOrderID == jobOrderId)
                 .Select(s => new
                  {
                      Code = s.JobOrderDetailID,
                      Name = s.RepairType.Name + "-" + s.JobCauses.Name
                  }).ToList();
            return Json(jobServices, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetStoreByBranch(int? id)
        //{
          
        //    var branches = db.InventoryStores.Where(o => BranchId == null || o.BranchId == BranchId)
        //          .Select(s => new
        //    {
        //        s.Name,
        //        s.StoreCode
        //    }).ToList();
        //    return Json(branches, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetStoreByBranch(string id, string text)
        {
            int did = int.Parse(id);
            var store = db.InventoryStores.Where(x => x.Enable).Where(x => x.BranchId == did).AsQueryable();

            if (!string.IsNullOrEmpty(text))
                store = store.Where(x => x.StoreCode.Contains(text) || x.Name.Contains(text));

            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetUom(string itemCode)
        {
            var item = db.InventoryItems.Find(itemCode);
            return Json(new { item.UoMName, item.ItemCode, item.AvgPrice},JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUoMs()
        {
            var branches = db.UoMs.Select(s => new
            {
                s.UoMName,
                s.Description
            }).ToList();
            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranches()
        {
            var branches = db.Branch.Select(s => new
            {
                s.Code,
                s.BranchId,
                s.Name
            }).ToList();
            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStoreByKeeper()
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //var store = db.InventoryStores.Where(x => mybraches.Contains(x.BranchId.ToString())).Where(x => x.Enable).AsQueryable();

           // List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            var store = db.InventoryStores.Where(x => x.Enable).AsQueryable();

            return Json(store.Select(p => new
            {
                StoreCode = p.StoreCode,
                Name = p.StoreCode + "-" + p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetWorkUnitCostCenters()
        {
            var costCenter = db.MainCompanyStructures;
            return
                Json(
                    costCenter.Select(
                        i => new
                        {
                            Name = i.Name,
                            Code = i.OrgStructureId
                        }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodDateRangeById(int id)
        {

            var glPeriod = db.InventoryPeriods.Find(id);
            if (glPeriod != null)
            {
                var formattedData = new
                {
                    Min = glPeriod.DateFrom,
                    Max = glPeriod.DateTo
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }

            return null;
        }

        public JsonResult GetOpenInventoryPeriodsByStore(string store)
        {
            var periodStatuses = db.InventoryStorePeriodStatuses
                                .Where(x => x.StoreCode == store)
                                .Where(x => x.Status == FiscalYearStatus.Open)
                                .AsQueryable();

            List<SelectorVm> list = new List<SelectorVm>();

            foreach (var period in periodStatuses)
            {
                var p = new SelectorVm
                {
                    Code = period.InventoryPeriodId.ToString(),
                    Name = db.InventoryPeriods.Find(period.InventoryPeriodId)?.Name
                };
                list.Add(p);
            }
            return Json(list.Select(p => new
            {
                GLPeriodId = p.Code,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public class SelectorVm
        {
            public string Name { get; set; }
            public string Code { get; set; }
        }
        public JsonResult GetEmployes()
        {
            var employees = db.Person_Employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrgnizationalCustomers()
        {
            var customers = db.OrganizationCustomers.ToList().Select(s => new
            {

                s.OrganizationCustomerID,
                s.TradeName
            });
            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPlateNos(JobRequestSource source, string text)
        {
            if(source == JobRequestSource.ExternalVehicle)
            {

                var equipmentInformation = db.EquipmentInformation.Where(x => x.LicensePlate != null).AsQueryable();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    equipmentInformation = equipmentInformation.Where(x => x.LicensePlate.ToLower().Contains(text.ToLower())).AsQueryable();
                }
                var licensePlates = equipmentInformation.ToList().Select(s => new
                {
                    Name = s?.LicensePlate,
                    PlateNumber = s?.LicensePlate
                }).ToList();
                return Json(licensePlates, JsonRequestBehavior.AllowGet);
            }

            var fleets = db.Fleets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            
            var plates = fleets.ToList().Select(s => new
            {
                //Name = s?.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID)?.Name ?? s?.SerialNo + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID)?.Name,
                Name = s.PlateNumber + "-" + s.SerialNo,
                PlateNumber = s.PlateNumber == "Not Assigned" || string.IsNullOrWhiteSpace(s.PlateNumber)? s.SerialNo : s.PlateNumber /*s?.PlateNumber ?? s?.SerialNo*/
            }).ToList();
            return Json(plates,JsonRequestBehavior.AllowGet);
        }
        //Changed to the above for ecwc 
        //public JsonResult GetPlateNos()
        //{
        //    var fleets = db.Fleets.Where(q => q.PlateNumber != null).ToList();
        //    var plates = fleets.Select(s => new
        //    {
        //        Name = s?.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID)?.Name ?? s?.SerialNo + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID)?.Name,
        //        PlateNumber = s?.PlateNumber ?? s?.SerialNo
        //    });
        //    return Json(plates, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetEquipmentModel(string plateNo)
        {
            var plate = db.EquipmentInformation.Where(q => q.LicensePlate == plateNo).FirstOrDefault();
            return Json(plate, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerPlateNos(string customerId, string text, string source)
        {
            int intSource = int.Parse(source);
            if (intSource == 1)
            {
                var platesInternal = db.Fleets.AsQueryable();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    platesInternal = platesInternal.Where(x => x.PlateNumber.Contains(text));
                }
                return Json(platesInternal.Select(x => new { Name = x.PlateNumber }).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var platesExternal = db.EquipmentInformation.AsQueryable();
                if (!string.IsNullOrWhiteSpace(customerId))
                {
                    int customerIdValue = int.Parse(customerId);
                    platesExternal = platesExternal.Where(x => x.CustomerId == customerIdValue);
                }
                return Json(platesExternal.Select(x => new { Name = x.LicensePlate }).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ServiceCrewActiveByService(string Id)
        {
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            var Crews = new List<ServiceCrew>();
            var JobOrder = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();

            var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

            var Services = db.JobOrderDetail.Where(x => x.JobOrderID == JobOrder.JobOrderID || x.JobOrderID == Request.JobRequestId).ToList();
            foreach (var Service in Services)
            {
                var ServiceCrew = _ServiceCrewService.GetAll().Where(x => x.ServiceID == Service.ServiceID).ToList();

                Crews.AddRange(ServiceCrew);
                Crews = Crews.Where(x => x.IsActive == true).ToList();

            }

            return Json(Crews.Select(s => new { Value = s.MechanicCrewID, Name = s.MechanicCrews.FullName }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetMechanicCrewAssigned(string Id)
        {
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            var Crews = new List<ServiceCrew>();
            var JobOrder = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();

            //var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

            var assignedCrews = _ServiceCrewService.GetAll().Where(sc => sc.ServiceID.ToString() == Id).ToList();
            if (assignedCrews.Any())
            {
                return Json(assignedCrews.Select(c => new { c.MechanicCrews.FullName, c.MechanicCrewID }), JsonRequestBehavior.AllowGet);
            }

            else
            {
                return null;
            }
        }

        //public JsonResult GetAssignEquipmentDDBind(Guid Id)
        //{
        //    var Assignment = db.AssignedEquipment.Where(x => x.AssignedEquipmentID == Id).FirstOrDefault();
        //    var Fleet = db.Fleets.Where(x => x.FleetsID == Assignment.EquipmentID).FirstOrDefault();
        //    string OwnedBy = "Company";
        //    if (Fleet.IsExternal)
        //    {
        //        OwnedBy = "External";
        //    }
        //    string Response = Fleet.EquipmentType.CategoryName + "_" + Fleet.CurentOdoMeter + "_" + Fleet.Model + "_" + Fleet.OperatorName + "_" + Fleet.UnitNum + "_" + OwnedBy;
        //    return Json(Response, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult OperatorDDBind()
        {
            var Placment = new List<FleetBindingHelper.HelperObject>();
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.OPERATOR || x.PlacmentType == FleetPlacmentType.DRIVER).ToList();
            foreach (var map in mapping)
            {
                var JobTitles = db.Lookups.Where(x => x.LookupId == map.JobTitleCode && x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE).ToList();
                foreach (var JobTitle in JobTitles)
                {
                    var sparePart = db.Placements.Where(x => x.LookupId == JobTitle.LookupId).Select(s => new FleetBindingHelper.HelperObject
                    {
                        Value = s.EmployeeId.ToString(),
                        Name = db.Person_Employee.Find(s.EmployeeId).FirstName + " " + db.Person_Employee.Find(s.EmployeeId).MiddleName + " " + db.Person_Employee.Find(s.EmployeeId).LastName
                    }).ToList();
                    Placment.AddRange(sparePart);
                }
            }
            return Json(Placment, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InventoryStoreDDBind()
        {
            var InventoryStores = db.InventoryStores.Select(s => new { Name = s.Name, StoreCode = s.StoreCode }).ToList();

            return Json(InventoryStores, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEquipmentByType(string Id)
        {
            var Equipments = db.FleetIFuelConsumptions.Where(x => x.EquipmentTypeID.ToString() == Id || x.EquipmentType.EquipmentTypeName == Id).FirstOrDefault();


            return Json(Equipments, JsonRequestBehavior.AllowGet);

        }
        public JsonResult EquipmentTypeFuelStandard(string Id)
        {
            var FleetIFuelConsumption = db.FleetIFuelConsumptions.Where(x => x.EquipmentTypeID.ToString() == Id).FirstOrDefault();

            if (FleetIFuelConsumption != null)
            {
                return Json(FleetIFuelConsumption.MinConsupmtion + "_" + FleetIFuelConsumption.MaxConsupmtion, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EquipmentDDBind()
        {
            var Equipments = db.Fleets.Where(x => x.IsOnlineApproved == true && x.IsReturned == false).Select(s => new { Name = s.PlateNumber, Value = s.PlateNumber }).ToList();

            return Json(Equipments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FleetPettyCashTypesDDBind()
        {
            var FleetPettyCashTypes = db.FleetPettyCashTypes.ToList();

            return Json(FleetPettyCashTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeDDBind()
        {
            var Equipments = db.Person_Employee.Where(x => x.IsActive == true).Select(s => new { Name = s.FirstName + " " + s.MiddleName, Value = s.FirstName + " " + s.MiddleName }).ToList();

            return Json(Equipments, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ArrivalTypeDDBind()
        {
            var ArrivalTypes = new List<FleetBindingHelper.HelperObject>();

            var ArrivalType = new FleetBindingHelper.HelperObject
            {
                Name = "By ItSelf",
                Value = "",
            };
            ArrivalTypes.Add(ArrivalType);

            ArrivalType = new FleetBindingHelper.HelperObject
            {
                Name = " Other Equipment ",
                Value = "",
            };
            ArrivalTypes.Add(ArrivalType);

            return Json(ArrivalTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IdealTypeDDBind()
        {
            var IdealTypes = new List<FleetBindingHelper.HelperObject>();
            var IdealType = new FleetBindingHelper.HelperObject
            {
                Name = "Operational Hour",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            IdealType = new FleetBindingHelper.HelperObject
            {
                Name = "Due to Non Avilablity of Construction Assignment",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            IdealType = new FleetBindingHelper.HelperObject
            {
                Name = "Due To Other Machine non Avilablity",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            IdealType = new FleetBindingHelper.HelperObject
            {
                Name = "Due to Lack of Fuel",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            IdealType = new FleetBindingHelper.HelperObject
            {
                Name = "Due To lack of Operators / Drivers ",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            IdealType = new FleetBindingHelper.HelperObject
            {
                Name = "Due To Accident ",
                Value = "",
            };
            IdealTypes.Add(IdealType);

            IdealType = new FleetBindingHelper.HelperObject
            {
                Name = " Other",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            return Json(IdealTypes, JsonRequestBehavior.AllowGet);
        }



        public JsonResult EquipmentStatusDDBind()
        {
            var EquipmentStatuss = new List<FleetBindingHelper.HelperObject>();
            var EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Operational",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due to Non Avilablity of Construction Assignment",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due To Other Machine non Avilablity",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due to Lack of Fuel",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due To lack of Operators / Drivers ",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due To Accident ",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            /* EquipmentStatus = new BindingHelper.HelperObject
             {
                 Name = "Under Maintenace",
                 Value = "",
             };*/
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = " Other",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            return Json(EquipmentStatuss, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ServiceStatusDDBind()
        {
          
                var FollowUPTypes = new List<FleetBindingHelper.HelperObject>();
                var FollowUPType = new FleetBindingHelper.HelperObject
                {
                    Name = "On Maintenance",
                    Value = "",
                };
                FollowUPTypes.Add(FollowUPType);
                FollowUPType = new FleetBindingHelper.HelperObject
                {
                    Name = "Waiting for SparePart",
                    Value = "",
                };
                FollowUPTypes.Add(FollowUPType);
                FollowUPType = new FleetBindingHelper.HelperObject
                {
                    Name = "SparePart Unreachable",
                    Value = "",
                };
                FollowUPTypes.Add(FollowUPType);
                FollowUPType = new FleetBindingHelper.HelperObject
                {
                    Name = "Waiting For Maintenace Tool",
                    Value = "",
                };
                FollowUPTypes.Add(FollowUPType);
                FollowUPType = new FleetBindingHelper.HelperObject
                {
                    Name = " Lack of Mechanics Crew  ",
                    Value = "",
                };
                FollowUPTypes.Add(FollowUPType);

                FollowUPType = new FleetBindingHelper.HelperObject
                {
                    Name = " Waiting For Approval",
                    Value = "",
                };
                FollowUPTypes.Add(FollowUPType);

                FollowUPType = new FleetBindingHelper.HelperObject
                {
                    Name = "Completed",
                    Value = "",
                };
                FollowUPTypes.Add(FollowUPType);

                return Json(FollowUPTypes, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult FollowUpTypeDDBind()
        {
            var FollowUPTypes = new List<FleetBindingHelper.HelperObject>();
            var FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = "Waiting for Additional Order",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = "Waiting for Inspection",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = "Waiting for root test",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = "Waiting For Spare Parts",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = " Waiting for man power",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = " Waiting for Space ",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = " Waiting for Special tool",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = " Waiting For External main",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);

           
            return Json(FollowUPTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TyreReasonTypeDDBind()
        {
            var ReasonTypes = new List<FleetBindingHelper.HelperObject>();
            var ReasonType = new FleetBindingHelper.HelperObject
            {
                Name = "UnUsable",
                Value = "",
            };
            ReasonTypes.Add(ReasonType);
            ReasonType = new FleetBindingHelper.HelperObject
            {
                Name = "Transfer",
                Value = "",
            };
            ReasonTypes.Add(ReasonType);
            ReasonType = new FleetBindingHelper.HelperObject
            {
                Name = "Faliure of Equipment",
                Value = "",
            };
            ReasonTypes.Add(ReasonType);


            return Json(ReasonTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MechanicDDBind()
        {

            var Placment = new List<FleetBindingHelper.HelperObject>();
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.MECHANIC).ToList();
            foreach (var map in mapping)
            {
                var JobTitles = db.Lookups.Where(x => x.LookupId == map.JobTitleCode && x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE).ToList();
                foreach (var JobTitle in JobTitles)
                {
                    var sparePart = db.Placements.Where(x => x.LookupId == JobTitle.LookupId).Select(s => new FleetBindingHelper.HelperObject
                    {
                        Value = s.EmployeeId.ToString(),
                        Name = db.Person_Employee.Find(s.EmployeeId).FirstName + " " + db.Person_Employee.Find(s.EmployeeId).MiddleName + " " + db.Person_Employee.Find(s.EmployeeId).LastName
                    }).ToList();
                    Placment.AddRange(sparePart);
                }
            }
            return Json(Placment, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMappedInventoryItem()
        {

            var InventoryItems = new List<FleetBindingHelper.HelperObject>();
            var mapping = db.InventoryCateoryMaps.ToList();
            foreach (var map in mapping)
            {

                var Items = db.InventoryItems.Where(x => x.ItemCategory.ItemCategoryCode == map.InventoryCategoryId).Select(s => new FleetBindingHelper.HelperObject
                {
                    Value = s.ItemCode.ToString(),
                    Name = s.Name,
                }).ToList();
                InventoryItems.AddRange(Items);
            }
            if (!InventoryItems.Any())
            {

                var Items = db.InventoryItems.Take(100).Select(s => new FleetBindingHelper.HelperObject
                {
                    Value = s.ItemCode.ToString(),
                    Name = s.Name,
                }).ToList();
                InventoryItems.AddRange(Items);
            }
            return Json(InventoryItems, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult DailyCareDDBind(string Category)
        {

            var DailyCares = db.DailyCares.Where(x => x.DailyCareCategoryId.ToString() == Category).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.DailyCareId.ToString(),
            }).ToList();
            return Json(DailyCares, JsonRequestBehavior.AllowGet);

        }


        [AllowAnonymous]
        public JsonResult JobOrderDDBind()
        {

            var JobOrders = db.JobOrder.Select(s => new
            {
                Name = s.Num.ToString(),
                Value = s.JobOrderID.ToString(),
            }).ToList();
            return Json(JobOrders, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public JsonResult JobDetailDDBind()
        {

            var DailyCares = db.JobOrderDetail.Select(s => new FleetBindingHelper.HelperObject
            {
                //Name = s.Service.ServiceName,
                Name = s.RepairTypeID.ToString(),
                Value = s.JobOrderDetailID.ToString(),
            }).ToList();
            return Json(DailyCares, JsonRequestBehavior.AllowGet);

        }

        [AllowAnonymous]
        //public JsonResult GetCheckListByEquipmentDDBind(string Type)
        //{
        //    List<CheckListViewModel> list = new List<CheckListViewModel>();
        //    List<CheckList> checkLists = db.CheckLists.Where(x => x.EquipmentCategory.ToString() == Type && x.IsMaintenace == false).ToList();


        //    foreach (var checkList in checkLists)
        //    {
        //        List<LineCheckList> lines = db.LineCheckLists.Where(x => x.CheckListId == checkList.CheckListId).ToList();
        //        string Category = Enum.GetName(typeof(CheckListType), checkList.Type);
        //        foreach (var line in lines)
        //        {
        //            if (!string.IsNullOrWhiteSpace(line.Name))
        //            {
        //                if (line.Name.Contains(" "))
        //                {
        //                    line.Name = line.Name.Replace(" ", "_");
        //                }
        //                list.Add(new CheckListViewModel { LineId = line.LineCheckListId, CategoryName = Category, IsActive = "false", IsActiveInspection = false, LineCode = line.Code, LineName = line.Name });
        //            }
        //        }

        //    }

        //    return Json(list, JsonRequestBehavior.AllowGet);

        //}
        public JsonResult GetSparePartAndMaterialCategory()
        {

            var Parts = db.SpareMaterialCategory.Select(s => new
            {
                Description = s.Description,
                SpareMaterialCategoryId = s.SpareMaterialCategoryId,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SparePartDDBind()
        {

            var Parts = db.InventoryItems.Where(x => x.ItemCategoryCode == SparePart).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPlacmentCategory(string Emp)
        {

            var Placements = db.Placements.Where(x => x.EmployeeId.ToString() == Emp).FirstOrDefault();
            var Title = db.Lookups.Where(x => x.LookupId == Placements.LookupId).FirstOrDefault();
            return Json(Title.Description, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSuperVisor(string Emp)
        {

            Placement Placements = db.Placements.Where(x => x.EmployeeId.ToString() == Emp).FirstOrDefault();
            if (Placements != null)
            {
                Person_Employee Person_Employee = db.Person_Employee.Where(x => x.EmployeeId == Placements.ImmediateSupervisor).FirstOrDefault();
                if (Person_Employee != null)
                {
                    return Json(Person_Employee.FirstName + " " + Person_Employee.FirstName + " " + Person_Employee.LastName, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(" Not Assigned ", JsonRequestBehavior.AllowGet);

        }
        public JsonResult ToolDDBind()
        {
            var Tools = db.InventoryItems.Where(x => x.ItemCategoryCode == Tool).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Tools, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetDailyCareByCategory(string Category)
        {
            var DailyCareCategory = db.DailyCares.Where(x => x.DailyCareCategoryId.ToString() == Category).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Description,
                Value = s.DailyCareCategoryId.ToString(),
            }).ToList();
            return Json(DailyCareCategory, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetFleetAssignedOperator()
        {
            var Fleets = db.Fleets.Where(x => x.OperatorName != "UnAssigned").Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.PlateNumber + " - " + s.UnitNum,
                Value = s.FleetsID.ToString(),
            }).ToList();
            return Json(Fleets, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetBranch()
        {
            var Fleets = db.Branch.Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Fleets, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPMServiceDDBind()
        {
            var Fleets = db.Service.Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.ServiceName,
                Value = s.ServiceID.ToString(),
            }).ToList();
            return Json(Fleets, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DailyCareCategoryDDBind()
        {

            var DailyCareCategory = db.DailyCareCategories.Select(s => new
            {
                Text = s.Name,
                Value = s.DailyCareCategoryId
            });

            return Json(DailyCareCategory, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetByType(string Type)
        {
            var Fleets = db.Fleets.Where(x => x.EquipmentIdentityID.ToString() == Type).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.PlateNumber + " - " + s.UnitNum,
                Value = s.FleetsID.ToString(),
            }).ToList();
            return Json(Fleets, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetFleetModelByType(string Type)
        {
            var Fleets = db.Fleets.Where(x => x.EquipmentIdentityID.ToString() == Type).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Model,
                Value = s.Model,
            }).ToList();
            return Json(Fleets.DistinctByField(x => x.Name).ToList(), JsonRequestBehavior.AllowGet);

        }
        public JsonResult FiscalYearDDBind()
        {
            var FiscalYears = db.GLFiscalYears.Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.GlFiscalYearId.ToString(),
            }).ToList();
            return Json(FiscalYears, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPeriodByYear(string Year)
        {
            var Periods = db.GLPeriods.Where(x => x.GlFiscalYearId.ToString() == Year).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.GLPeriodId.ToString(),
            }).ToList();
            return Json(Periods, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetModelByType(string Type)
        {
            var Models = db.Fleets.Where(x => x.EquipmentTypeID.ToString() == Type).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Model,
                Value = s.Model.ToString(),
            }).ToList();
            return Json(Models, JsonRequestBehavior.AllowGet);

        }
        public JsonResult InsuranceTypeDDBind()
        {
            var InsuranceTypes = db.InsuranceType.Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(InsuranceTypes, JsonRequestBehavior.AllowGet);

        }

        [AllowAnonymous]
        public ActionResult YearDDBind()
        {
            List<SelectListItem> Years = new List<SelectListItem>();
            for (int i = DateTime.Now.Year; i > 1980; i--)
            {
                Years.Add(new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            return Json(Years, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult ManufacturedYearDDBind()
        {
            var Fleet = db.Fleets.DistinctByField(x => x.ManufacturerYear).Select(s => new
            {
                ManufacturerYear = s.ManufacturerYear
            }).ToList();

            return Json(Fleet, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult InventoryStoresDDBind()
        {
            var Store = db.InventoryStores.Select(s => new
            {
                Name = s.Name
            }).ToList();

            return Json(Store, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSparePrice(string Id)
        {
            var Tools = db.InventoryItems.Where(x => x.Name == Id).FirstOrDefault();
            if (Tools != null)
            {
                return Json(Tools.AvgPrice, JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEquipmentFuelEfficency(string Id, string From, string To)
        {
            try
            {
                IFleetService _FleetsService = new FleetService();
                ITimeSheetService _TimeSheetService = new TimeSheetService();
                FleetReportHelper fleetReportHelper = new FleetReportHelper();
                decimal Rate = 0;
                decimal Issued = 0;
                var AlertLevels = db.FuelConsumptionAlertLevel.ToList();
                var Fleet = _FleetsService.GetById(Guid.Parse(Id));
                DateTime FromDate = DateTime.Parse(From);
                DateTime ToDate = DateTime.Parse(To);
                decimal TotalReading = decimal.Zero;
                if (Fleet != null)
                {

                    var Asset = db.AssetRegistrations.Where(x => x.Tag == Fleet.UnitNum).FirstOrDefault();
                    if (Asset != null)
                    {
                        var FuelConsumptions =
                       fleetReportHelper.GetInventoryIssueItem(x => x.Issue.IsFuel == true && x.Issue.PlateNo == Asset.PlateNo && x.DateCreated >= FromDate && x.DateCreated <= ToDate).ToList();
                        var Sheet = _TimeSheetService.GetAll().Where(x => x.FleetsID == Fleet.FleetsID && x.DateCreated >= FromDate && x.DateCreated <= ToDate);
                        if (Fleet.ReadingType == ReadingType.HR)
                        {
                            foreach (var Work in Sheet)
                            {
                                TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                                TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                                TotalReading = TotalReading + (MorningDuration.Hours + AfterNoonDuration.Hours);

                            }
                        }
                        else
                        {
                            foreach (var Work in Sheet)
                            {

                                TotalReading = TotalReading + (Work.FinalKMHrReading - Work.KMHrReading);

                            }

                        }




                        Issued = FuelConsumptions.Where(x => x.DateCreated > FromDate && x.DateCreated < ToDate).Sum(x => x.ApprovedQuantity);
                    }
                    if (Issued == 0)
                    {
                        Issued = 1;
                    }
                }


                Rate = (TotalReading / FleetGenericHelper.DivideByZero(Issued * ((Fleet.MinFuelStandard + Fleet.MaxFuelStandard) / 2) / 100));



                var Level = AlertLevels.Where(x => x.FromValue > Rate && x.ToValue < Rate).FirstOrDefault();

                if (Level != null)
                {
                    return Json(Rate + "_" + Level.Name, JsonRequestBehavior.AllowGet);

                }

                return Json(Rate + "_(" + Rate + "%) Alert Level Not Found ", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(0 + "_", JsonRequestBehavior.AllowGet);
            }



        }
        public decimal GetSpareCost(Guid Id)
        {
            decimal cost = 0;
            var Services = db.JobOrderDetail.Where(x => x.JobOrderID == Id).ToList();

            foreach (var Service in Services)
            {
                var Spares = db.ServiceSpare.Where(x => x.ServiceID == Service.JobOrderID).ToList();
                foreach (var Spare in Spares)
                {

                    var InventoryItem = db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                    var RequestionRef = db.JobStoreRequisition.Where(x => x.JobId == Spare.ServiceSpareID).FirstOrDefault();
                    var Requestion = db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId).FirstOrDefault();

                    cost = cost + (InventoryItem.AvgPrice * Requestion.IssuedQuantity);

                }

            }
            return cost;
        }

        public JsonResult GetSparePartMaterialCategory()
        {
            var inventoryCategories = db.ItemCategorys.ToList().Select(s => new
            {

                s.ItemCategoryCode,
                s.Name
            });
            return Json(inventoryCategories, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCategory()
        {
            var inventoryCategories = db.ItemCategorys.Where(q => q.HaveParent == false).ToList().Select(s => new
            {

                s.ItemCategoryCode,
                s.Name
            });
            return Json(inventoryCategories, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubcategoriesBasedOnCategory(string categoryCode)
        {
            var inventoryCategories = db.ItemCategorys.Where(q=>q.ParentName== categoryCode).ToList().Select(s => new
            {

                s.ItemCategoryCode,
                s.Name
            });
            return Json(inventoryCategories, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetItemsUnderCategory(string categoryId, string filterText)
        {
            var inventoryItems = db.InventoryItems.Where(q => q.ItemCategoryCode == categoryId).ToList().Select(s => new
            {

                s.ItemCode,
                Name = s.ItemCode + "_" + s.Name
            });
            if (!string.IsNullOrEmpty(filterText))
            {
                inventoryItems = inventoryItems.Where(l => l.Name.ToLower().Contains(filterText.ToLower()) || l.ItemCode.Contains(filterText)).ToList();
            }
           
            return Json(inventoryItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRepairTypes()
        {
            var types = db.RepairType.Select(t => new
            {
                Name = t.Code + " - " + t.Name,
                RepairTypeID = t.RepairTypeID
            }).ToList();
            return Json(types, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult ApprovalDDBind()
        {
            var approvals = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();
            var approval = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
            {
                Name = MaintenanceConst.Approved,
                Id = 1,
            };
            approvals.Add(approval);

            var approval2 = new FleetBindingHelper.HelperObject()
            {
                Name = MaintenanceConst.Pending,
                Id = 2,
            };
            approvals.Add(approval2);
            var approval3 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
            {
                Name = MaintenanceConst.Rejected,
                Id = 3,
            };
            approvals.Add(approval3);
            return Json(approvals, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult ServiceStatusDDBind()
        //{
        //    var FollowUPTypes = new List<FleetBindingHelper.HelperObject>();
        //    var FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "On Maintenance",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "Waiting for SparePart",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "SparePart Unreachable",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "Waiting For Maintenace Tool",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = " Lack of Mechanics Crew  ",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);

        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = " Waiting For Approval",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);

        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "Completed",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);

        //    return Json(FollowUPTypes, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult FollowUpTypeDDBind()
        //{
        //    var FollowUPTypes = new List<FleetBindingHelper.HelperObject>();
        //    var FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "On Maintenance",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "Waiting for SparePart",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "SparePart Unreachable",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = "Waiting For Maintenace Tool",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = " Lack of Mechanics Crew  ",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = " Accident ",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = " On Road BreakDown",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = " Waiting For Approval",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);

        //    FollowUPType = new FleetBindingHelper.HelperObject
        //    {
        //        Name = " Insurance ",
        //        Value = "",
        //    };
        //    FollowUPTypes.Add(FollowUPType);
        //    return Json(FollowUPTypes, JsonRequestBehavior.AllowGet);
        //}
    }
}

