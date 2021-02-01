using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class DataProviderController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public const string SparePart = "1001";
        public const string Tool = "1002";
        public const string Operator = "Operator";
        public const string Mechanic = "Mechanic";

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
            var Placment = new List<BindingHelper.HelperObject>();
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.OPERATOR).ToList();
            foreach (var map in mapping)
            {
                var JobTitles = db.JobTitleCodes.Where(x => x.JobTitleCodeId == map.JobTitleCode).ToList();
                foreach (var JobTitle in JobTitles)
                {
                    var sparePart = db.Placements.Where(x => x.JobTitleName == JobTitle.JobTitleCodeId.ToString() && x.isCurrent == true).Select(s => new BindingHelper.HelperObject
                    {
                        Value = s.Employee.FirstName + " " + s.Employee.LastName,
                        Name = s.Employee.FirstName + " " + s.Employee.LastName
                    }).ToList();
                    Placment.AddRange(sparePart);
                }
            }
            return Json(Placment, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InventoryStoreDDBind()
        {
          var InventoryStores = db.InventoryStores.Select( s=> new  { Name = s.Name, StoreCode = s.StoreCode }).ToList();
        
            return Json(InventoryStores, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ArrivalTypeDDBind()
        {
            var ArrivalTypes = new List<BindingHelper.HelperObject>();

            var ArrivalType = new BindingHelper.HelperObject
            {
                Name = "By ItSelf",
                Value = "",
            };
            ArrivalTypes.Add(ArrivalType);

            ArrivalType = new BindingHelper.HelperObject
           {
               Name = " Other Equipment ",
               Value = "",
           };
            ArrivalTypes.Add(ArrivalType);

            return Json(ArrivalTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IdealTypeDDBind()
        {
            var IdealTypes = new List<BindingHelper.HelperObject>();
            var IdealType = new BindingHelper.HelperObject
            {
                Name = "Operational Hour",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            IdealType = new BindingHelper.HelperObject
           {
               Name = "Due to Non Avilablity of Construction Assignment",
               Value = "",
           };
            IdealTypes.Add(IdealType);
            IdealType = new BindingHelper.HelperObject
           {
               Name = "Due To Other Machine non Avilablity",
               Value = "",
           };
            IdealTypes.Add(IdealType);
            IdealType = new BindingHelper.HelperObject
           {
               Name = "Due to Lack of Fuel",
               Value = "",
           };
            IdealTypes.Add(IdealType);
            IdealType = new BindingHelper.HelperObject
           {
               Name = "Due To lack of Operators / Drivers ",
               Value = "",
           };
            IdealTypes.Add(IdealType);
            IdealType = new BindingHelper.HelperObject
            {
                Name = "Due To Accident ",
                Value = "",
            };
            IdealTypes.Add(IdealType);
 
            IdealType = new BindingHelper.HelperObject
            {
                Name = " Other",
                Value = "",
            };
            IdealTypes.Add(IdealType);
            return Json(IdealTypes, JsonRequestBehavior.AllowGet);
        }



        public JsonResult EquipmentStatusDDBind()
        {
            var EquipmentStatuss = new List<BindingHelper.HelperObject>();
            var EquipmentStatus = new BindingHelper.HelperObject
            {
                Name = "Operational",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new BindingHelper.HelperObject
            {
                Name = "Due to Non Avilablity of Construction Assignment",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new BindingHelper.HelperObject
            {
                Name = "Due To Other Machine non Avilablity",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new BindingHelper.HelperObject
            {
                Name = "Due to Lack of Fuel",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new BindingHelper.HelperObject
            {
                Name = "Due To lack of Operators / Drivers ",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new BindingHelper.HelperObject
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
            EquipmentStatus = new BindingHelper.HelperObject
            {
                Name = " Other",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            return Json(EquipmentStatuss, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ServiceStatusDDBind()
        {
            var FollowUPTypes = new List<BindingHelper.HelperObject>();
            var FollowUPType = new BindingHelper.HelperObject
            {
                Name = "On Maintenance",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = "Waiting for SparePart",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = "SparePart Unreachable",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = "Waiting For Maintenace Tool",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = " Lack of Mechanics Crew  ",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
        
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = " Waiting For Approval",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);

            FollowUPType = new BindingHelper.HelperObject
            {
                Name = "Completed",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
         
            return Json(FollowUPTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FollowUpTypeDDBind()
        {
            var FollowUPTypes = new List<BindingHelper.HelperObject>();
            var FollowUPType = new BindingHelper.HelperObject
            {
                Name = "On Maintenance",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = "Waiting for SparePart",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = "SparePart Unreachable",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = "Waiting For Maintenace Tool",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = " Lack of Mechanics Crew  ",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = " Accident ",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = " On Road BreakDown",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            FollowUPType = new BindingHelper.HelperObject
            {
                Name = " Waiting For Approval",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);

            FollowUPType = new BindingHelper.HelperObject
            {
                Name = " Insurance ",
                Value = "",
            };
            FollowUPTypes.Add(FollowUPType);
            return Json(FollowUPTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TyreReasonTypeDDBind()
        {
            var ReasonTypes = new List<BindingHelper.HelperObject>();
            var ReasonType = new BindingHelper.HelperObject
            {
                Name = "UnUsable",
                Value = "",
            };
            ReasonTypes.Add(ReasonType);
            ReasonType = new BindingHelper.HelperObject
            {
                Name = "Transfer",
                Value = "",
            };
            ReasonTypes.Add(ReasonType);
            ReasonType = new BindingHelper.HelperObject
            {
                Name = "Faliure of Equipment",
                Value = "",
            };
            ReasonTypes.Add(ReasonType);


            return Json(ReasonTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MechanicDDBind()
        {

            var Placment = new List<BindingHelper.HelperObject>();
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.MECHANIC).ToList();
            foreach (var map in mapping)
            {
                var JobTitles = db.JobTitleCodes.Where(x => x.JobTitleCodeId == map.JobTitleCode).ToList();
                foreach (var JobTitle in JobTitles)
                {
                    var sparePart = db.Placements.Where(x => x.JobTitleName == JobTitle.JobTitleCodeId.ToString() && x.isCurrent == true).Select(s => new BindingHelper.HelperObject
                    {
                        Value = s.Employee.FirstName + " " + s.Employee.LastName,
                        Name = s.Employee.FirstName + " " + s.Employee.LastName
                    }).ToList();
                    Placment.AddRange(sparePart);
                }
            }
            return Json(Placment, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SparePartDDBind()
        {

            var Parts = db.InventoryItems.Where(x => x.ItemCategoryCode == SparePart).Select(s => new BindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ToolDDBind()
        {
            var Tools = db.InventoryItems.Where(x => x.ItemCategoryCode == Tool).Select(s => new BindingHelper.HelperObject
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Tools, JsonRequestBehavior.AllowGet);

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
            var Fleet = db.Fleets.DistinctBy(x => x.ManufacturerYear).Select(s => new
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
    }
}