using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using System.Data.Entity;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FleetDataProviderController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public const string SparePart = "1001";
        public const string Tool = "1002";
        public const string Operator = "Operator";
        public const string Mechanic = "Mechanic";
        //public JsonResult ServiceCrewActiveByService(string Id)
        //{
        //    IServiceCrewService _ServiceCrewService = new ServiceCrewService();
        //    var Crews = new List<ServiceCrew>();
        //    var JobOrder = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();

        //    var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

        //    var Services = db.JobOrderDetail.Where(x => x.JobOrderID == JobOrder.JobOrderID || x.JobOrderID == Request.JobRequestId).ToList();
        //    foreach (var Service in Services)
        //    {
        //        var ServiceCrew = _ServiceCrewService.GetAll().Where(x => x.ServiceID == Service.ServiceID).ToList();

        //        Crews.AddRange(ServiceCrew);
        //        Crews = Crews.Where(x => x.IsActive == true).ToList();
 
        //    }

        //    return Json(Crews.Select(s => new    {   Value = s.MechanicCrewID  , Name = s.MechanicCrews.FullName  }),JsonRequestBehavior.AllowGet);

        //}

        public JsonResult GetAssignEquipmentDDBind(Guid Id)
        {
            var Assignment = db.AssignedEquipment.Where(x => x.AssignedEquipmentID == Id).FirstOrDefault();
            var Fleet = db.Fleets.Where(x => x.FleetsID == Assignment.EquipmentID).FirstOrDefault();
            string OwnedBy = "Company";
            if(Fleet.IsExternal)
            {
             OwnedBy = "External";
            }
            string Response = Fleet.EquipmentType.CategoryName + "_" + Fleet.CurentOdoMeter + "_" + Fleet.Model + "_" + Fleet.OperatorName + "_" + Fleet.UnitNum + "_" + OwnedBy;
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperatorDDBind()
        {
            var Placment = new List<FleetBindingHelper.HelperObject>();
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.OPERATOR || x.PlacmentType == FleetPlacmentType.DRIVER).ToList();
            foreach (var map in mapping)
            {
                var JobTitles = db.Lookups.Where(x => x.LookupId == map.JobTitleCode && x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE).ToList();
                foreach (var JobTitle in JobTitles)
                {
                    var placements = db.Placements.Where(x => x.LookupId == JobTitle.LookupId  )
                        //.Select(s => new FleetBindingHelper.HelperObject
                        //{
                        //    Value = s.EmployeeId.ToString() ,
                        //    Name = s.Employee.FirstName + " "  +s.Employee.MiddleName + " " + s.Employee.LastName
                        //})
                    .ToList();
                    foreach  (var pl in placements)
                    {
                        var employee = db.Person_Employee.Find(pl.EmployeeId);
                        if(employee != null)
                        {
                            var record = new FleetBindingHelper.HelperObject
                            {
                                Value = employee.EmployeeId.ToString(),
                                Name = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName
                            };
                            Placment.Add(record);
                        }
                    }
                    //Placment.AddRange(sparePart);
                }
            }
            return Json(Placment, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InventoryStoreDDBind()
        {
          var InventoryStores = db.InventoryStores.Select( s=> new  { Name = s.Name, StoreCode = s.StoreCode }).ToList();
        
            return Json(InventoryStores, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEquipmentByType(string Id)
        {
            var Equipments = db.FleetIFuelConsumptions.Where(x => x.EquipmentTypeId.ToString() == Id || x.EquipmentType.EquipmentTypeName == Id).FirstOrDefault();

          
                return Json(Equipments, JsonRequestBehavior.AllowGet);
            
        }
        public JsonResult EquipmentTypeFuelStandard(string Id)
        {
            var FleetIFuelConsumption = db.FleetIFuelConsumptions.Where(x => x.EquipmentTypeId.ToString() == Id).FirstOrDefault();

            if(FleetIFuelConsumption != null)
            {
                return Json(FleetIFuelConsumption.MinConsupmtion+"_"+ FleetIFuelConsumption.MaxConsupmtion, JsonRequestBehavior.AllowGet);
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
                Name = " Accident ",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new FleetBindingHelper.HelperObject
            {
                Name = " On Road BreakDown",
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
                Name = " Insurance ",
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
                    var placements = db.Placements.Where(x => x.LookupId == JobTitle.LookupId).ToList();
                    foreach (var pl in placements)
                    {
                        var employee = db.Person_Employee.Find(pl.EmployeeId);
                        if (employee != null)
                        {
                            var record = new FleetBindingHelper.HelperObject
                            {
                                Value = employee.EmployeeId.ToString(),
                                Name = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName
                            };
                            Placment.Add(record);
                        }
                    }
                    //Placment.AddRange(sparePart);
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
           if(!InventoryItems.Any())
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

            var DailyCares = db.DailyCares.Where(x=>x.DailyCareCategoryId.ToString() == Category).Select(s => new FleetBindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.DailyCareId.ToString(),
            }).ToList();
            return Json(DailyCares, JsonRequestBehavior.AllowGet);

        }


        [AllowAnonymous]
        public JsonResult JobOrderDDBind()
        {

            var JobOrders = db.JobOrder.Select(s =>  new
            {
                Name = s.Num.ToString(),
                Value = s.JobOrderID.ToString(),
            }).ToList();
            return Json(JobOrders, JsonRequestBehavior.AllowGet);

        }
        //[AllowAnonymous]
        //public JsonResult  JobDetailDDBind()
        //{

        //    var DailyCares = db.JobOrderDetail.Select(s => new FleetBindingHelper.HelperObject
        //    {
        //        Name = s.Service.ServiceName,
        //        Value = s.JobOrderDetailID.ToString(),
        //    }).ToList();
        //    return Json(DailyCares, JsonRequestBehavior.AllowGet);

        //}

        [AllowAnonymous]
        public JsonResult GetCheckListByEquipmentDDBind(string Type)
        {
            List<CheckListViewModel> list = new List<CheckListViewModel>();
            List<CheckList> checkLists = db.CheckLists.Where(x => x.EquipmentCategory.ToString() == Type && x.IsMaintenace == false).ToList();


            foreach (var checkList in checkLists)
            {
                List<LineCheckList> lines = db.LineCheckLists.Where(x => x.CheckListId == checkList.CheckListId).ToList();
                string Category = Enum.GetName(typeof(CheckListType), checkList.Type);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace( line.Name ))
                    {
                        if (line.Name.Contains(" "))
                        {
                            line.Name = line.Name.Replace(" ", "_");
                        }
                        list.Add(new CheckListViewModel { LineId = line.LineCheckListId, CategoryName = Category, IsActive = "false", IsActiveInspection = false, LineCode = line.Code, LineName = line.Name });
                    }
                }

            }

            return Json(list, JsonRequestBehavior.AllowGet);
     
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
        
        public JsonResult GetPlacmentCategory( string Emp )
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
            var DailyCareCategory = db.DailyCares.Where(x=>x.DailyCareCategoryId.ToString() == Category).Select(s => new FleetBindingHelper.HelperObject
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
                Name = s.PlateNumber+" - "+s.UnitNum,
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
            return Json(Fleets.DistinctByField(x=>x.Name).ToList(), JsonRequestBehavior.AllowGet);

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


        public JsonResult GetEquipmentFuelEfficency(string Id, string From , string To)
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
                DateTime ToDate = DateTime.Parse(To) ;
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
                    return Json(Rate  + "_" + Level.Name, JsonRequestBehavior.AllowGet);
             
                  }

                return Json(Rate+"_("+Rate+"%) Alert Level Not Found ", JsonRequestBehavior.AllowGet);
                   }
            catch (Exception e)
            {
                return Json(0+"_" , JsonRequestBehavior.AllowGet);
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

                    //var InventoryItem = db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                    //var RequestionRef = db.JobStoreRequisition.Where(x => x.JobId == Spare.ServiceSpareID).FirstOrDefault();
                    //var Requestion = db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId).FirstOrDefault();

                    //cost = cost + (InventoryItem.AvgPrice * Requestion.IssuedQuantity);

                }

            }
            return cost;
        }
    }
}