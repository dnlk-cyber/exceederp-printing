using ExceedERP.ApplicationServices.FleetManagement.FuelManagement;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System.Data.Entity;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Inventory;
using System.Threading.Tasks;
using Kendo.Mvc;
using System.Linq.Expressions;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FuelRequestor, FleetManagementRoles.FuelApprover)]
    public class FuelController : Controller
    {
        IFuelService _FuelService;
        IFleetService _FleetService;
        IOperatorService _OperatorService;
        ExceedDbContext db = new ExceedDbContext();
        public FuelController()
        {
            _FuelService = new FuelService();
            _FleetService = new FleetService();
            _OperatorService = new OperatorService();
        }
        //
        // GET: /FleetManagement/Fuel/
        public ActionResult Index()
        {

            return View();
        }

        public string GetBranch(int Id)
        {
            var IssueQty = db.Branch.Where(x => x.BranchId == Id).FirstOrDefault();
            if (IssueQty != null)
            {
                return IssueQty.Name;
            }
            return string.Empty;
        }


        public ActionResult Fuel_Read([DataSourceRequest] DataSourceRequest request)
        {


            List<IssueItem> IssuesFuelItem = new List<IssueItem>();
            IFleetService _FleetService = new FleetService();
            var FleetList = _FleetService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

            string dateFrom = string.Empty;
            string dateTo = string.Empty;
            string plateAssetNo = string.Empty;

            if (request.Filters.Any())

            {


                foreach (var f in request.Filters)
                {

                    var filterDescriptor = f as CompositeFilterDescriptor;
                    var compositeFilterDescriptor = f as List<CompositeFilterDescriptor>;


                    var param = (Kendo.Mvc.FilterDescriptor)filterDescriptor.FilterDescriptors[0];
                    var paramcomposite = (Kendo.Mvc.CompositeFilterDescriptor)filterDescriptor.FilterDescriptors[1];
                    var firstparam = (Kendo.Mvc.FilterDescriptor)paramcomposite.FilterDescriptors[0];
                    var secondparam = (Kendo.Mvc.FilterDescriptor)paramcomposite.FilterDescriptors[1];

                    if (param != null && param.Member == "PlateNo")
                    {
                        plateAssetNo = param.Value.ToString();
                    }
             
                    if (firstparam != null && firstparam.Member == "To")
                    {
                        dateTo = firstparam.Value.ToString();
                    }

                    if (secondparam != null && secondparam.Member == "From")
                    {
                        dateFrom = secondparam.Value.ToString();
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(dateFrom) && !string.IsNullOrWhiteSpace(dateTo) && !string.IsNullOrWhiteSpace(plateAssetNo))
            {
                var DateFrom = DateTime.Parse(dateFrom);
                var DateTo = DateTime.Parse(dateTo);
                IssuesFuelItem = db.IssueItems.AsNoTracking().Where(x => x.Issue.IsFuel == true && x.Issue.PlateNo.ToUpper().Contains(plateAssetNo.ToUpper()) && x.DateCreated >= DateFrom && x.DateCreated <= DateTo).OrderByDescending(x => x.DateCreated).ToList();
            }

            if (!string.IsNullOrWhiteSpace(dateFrom) && !string.IsNullOrWhiteSpace(dateTo) && string.IsNullOrWhiteSpace(plateAssetNo))
            {
                var DateFrom = DateTime.Parse(dateFrom);
                var DateTo = DateTime.Parse(dateTo);
                IssuesFuelItem = db.IssueItems.AsNoTracking().Where(x => x.Issue.IsFuel == true && x.DateCreated >= DateFrom && x.DateCreated <= DateTo).OrderByDescending(x => x.DateCreated).ToList();
            }
            if (string.IsNullOrWhiteSpace(dateFrom) && string.IsNullOrWhiteSpace(dateTo) && !string.IsNullOrWhiteSpace(plateAssetNo))
            {

                IssuesFuelItem = db.IssueItems.AsNoTracking().Where(x => x.Issue.IsFuel == true && x.Issue.PlateNo.ToUpper().Contains(plateAssetNo.ToUpper())).OrderByDescending(x => x.DateCreated).Take(100).ToList();
            }
            if (string.IsNullOrWhiteSpace(dateFrom) && string.IsNullOrWhiteSpace(dateTo) && string.IsNullOrWhiteSpace(plateAssetNo))
            {

                IssuesFuelItem = db.IssueItems.AsNoTracking().Where(x => x.Issue.IsFuel == true).OrderByDescending(x => x.DateCreated).Take(100).ToList();
            }

            request.Filters = null;



            return Json(IssuesFuelItem.Select(o => new
            {
                FuelID = o.IssueID,
                FleetsID = o.Issue.OrgStructureId,
                ReceiptNo = o.Issue.StoreCode,
                FuelType = o.ItemCategory.Name,
                FuelFilledAmount = o.ApprovedQuantity,
                Description = o.Issue.Reference,
                PlanKmHr = o.Issue.ReadingKM,
                InitialKmHr = o.Issue.ReadingKM,
                FinalKmHr = o.Issue.ReadingKM,
                EquipmentName = GetEquipmentName(o.Issue.PlateNo, FleetList),
                AssetNo = GetAssetNo(o.Issue.PlateNo, FleetList),
                UnitPrice = o.UnitPrice,
                TotalFuelcost = o.Total,
                Remark = o.Issue.PlateNo,
                Project = o.Issue.OrgBranchName,
                IsSendForApproval = o.Issue.IsSendForApproval,
                IsOnlineApproved = o.Issue.IsOnlineApproved,
                Status = o.Issue.Status,
                DateCreated = o.DateCreated,
                LastModified = o.LastModified,
                CreatedBy = o.Issue.IssueDate,
                ModifiedBy = o.ModifiedBy,
                RequestedQty = GetReqQty(o.Issue.PlateNo)

            }).ToDataSourceResult(request));
        }

        private decimal GetReqQty(string plateNo)
        {
            decimal requestedQty = 0;

            if (!string.IsNullOrWhiteSpace(plateNo))
            {


                var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == plateNo);
                if(fleet != null)
                {
                    var vehicleDemandIds = db.AssignedEquipment
                        .Where(x => x.FleetId == fleet.FleetsID)
                        .Select(s => s.VehicleDemandId)
                        .ToList();

                    var requestedEquipments = db.RequestedEquipments
                        .Where(re => vehicleDemandIds.Contains(re.VehicleDemandId))
                        .ToList();

                    var requestNumbers = requestedEquipments
                        .Select(re => re.RequestNumber)
                        .ToList();

                    var vehicleDistributions = db.VehicleDistribution
                        .Where(vd => requestNumbers.Contains(vd.RequestID))
                        .ToList();
                    foreach (var vd in vehicleDistributions)
                    {
                        var storeRequisitionIds = db.StoreRequisitions
                            .Where(x => x.IsFuel == true && x.VehicleDistributionID == vd.VehicleDistributionID)
                            .Where(x => x.IsForMaintenance == false)
                            .Select(s => s.StoreRequisitionID)
                            .ToList();
                        var qty = db.StoreRequisitionItems
                            .Where(p => storeRequisitionIds.Contains(p.StoreRequisitionID))
                            .Sum(x => (decimal?)x.RequestedQuantity) ?? 0;

                        requestedQty += qty;
                    }
                    return requestedQty;
                }
               
            }
            return requestedQty;
        }

        public string GetAssetNo(string p, List<Fleets> fleets)
        {
            var Fleet = fleets.Where(x => x.PlateNumber == p).FirstOrDefault();
            if (Fleet != null)
            {
                return Fleet.UnitNum;
            }
            return string.Empty;
        }

        public string GetEquipmentName(string plateNo, List<Fleets> fleets)
        {
            var Fleet = fleets.Where(x => x.PlateNumber == plateNo).FirstOrDefault();
            if (Fleet != null)
            {
                return Fleet.EquipmentName;
            }
            return string.Empty;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Fuel_Create([DataSourceRequest] DataSourceRequest request, Fuel Fuel)
        {
            ModelState.Remove("FuelID");
            ModelState.Remove("FleetsID");

            string OperatorName = "Not Assigned";
            Fuel.FuelID = Guid.NewGuid();
            Fleets fleet = _FleetService.GetById(Fuel.FleetsID);
            Operator operators = _OperatorService.GetAll().Where(x => x.FleetsID == Fuel.FleetsID).FirstOrDefault();
            if (operators != null)
            {
                OperatorName = operators.FullName;
            }

            var Inventory = db.FuelInventorys.Where(x => x.FuelType == fleet.TypeOfFuel && x.SiteName == fleet.Location && x.Quantity > Fuel.FuelFilledAmount).FirstOrDefault();
            if (Inventory == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingPlateNumber", " Insufficient Amount In Depot !");
            }
            else
            {
                if (Fuel != null)
                {
                    /*
                    Inventory.Quantity = Inventory.Quantity - Fuel.FuelFilledAmount;
                    db.FuelInventorys.Attach(Inventory);
                    db.Entry(Inventory).State = EntityState.Modified;
                    db.SaveChanges();
                    */
                    db = new ExceedDbContext();

                    Fuel.FuelStandard = fleet.MinFuelStandard.ToString();
                    Fuel.Project = fleet.Location;
                    Fuel.FuelType = fleet.TypeOfFuel;
                    Fuel.OperatorName = OperatorName;
                    Fuel.TotalFuelcost = Fuel.FuelFilledAmount * Fuel.UnitPrice;
                    Fuel.CreatedBy = User.Identity.Name;
                    Fuel.DateCreated = DateTime.Now;
                    _FuelService.Insert(Fuel);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fuel Registration", "Success", User.Identity.Name, $"created fuel reg {Fuel.FuelID}", Modules.FleetManagement);
                    #endregion
                }
            }
            return Json(new[] { Fuel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Fuel_Update([DataSourceRequest] DataSourceRequest request, Fuel Fuel)
        {
            if (Fuel != null)
            {
                Fuel.ModifiedBy = User.Identity.Name;
                Fuel.LastModified = DateTime.Now;
                _FuelService.Update(Fuel);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fuel Registration", "Success", User.Identity.Name, $"updated fuel reg {Fuel.FuelID}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { Fuel }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Fuel_Destroy([DataSourceRequest] DataSourceRequest request, Fuel fuel)
        {
            var Fuel = _FuelService.GetById(fuel.FuelID);
            if (Fuel != null)
            {
                _FuelService.Delete(Fuel);
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Fuel number " + fuel.FuelID);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted fuel registration number {Fuel.FuelID }");
                #endregion

            }

            return Json(new[] { Fuel }.ToDataSourceResult(request));
        }



    }
}