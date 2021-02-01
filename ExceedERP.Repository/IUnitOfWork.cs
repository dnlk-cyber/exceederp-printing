
using System;

using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System.Threading.Tasks;

namespace ExceedERP.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        #region 1 - Fleet Registration
        IGenericRepository<Accessories> AccessoriesRepository { get; }
        IGenericRepository<Accident> AccidentRepository { get; }
        IGenericRepository<EquipmentIdentity> EquipmentIdentityRepository { get; }
        IGenericRepository<EquipmentType> EquipmentTypeRepository { get; }
        IGenericRepository<FleetHistory> FleetHistoryRepository { get; }
        IGenericRepository<Fleets> FleetsRepository { get; }
        IGenericRepository<FleetInsurance> InsuranceRepository { get; }
        IGenericRepository<Operator> OperatorRepository { get; }
        IGenericRepository<TimeSheet> TimeSheetRepository { get; }
        IGenericRepository<OperatorPosition> OperatorPositionRepository { get; }
        IGenericRepository<Manufacturer> ManufacturerRepository { get; }
        IGenericRepository<Educationallevel> EducationallevelRepository { get; }
        IGenericRepository<EquipmentName> EquipmentNameRepository { get; }
        IGenericRepository<Structure> StructureRepository { get; }
        IGenericRepository<Branch> BranchRepository { get; }
        #endregion

        #region 2 - EquipmentInspection
        IGenericRepository<AnnualEquipmentInventoryInspection> AnnualEquipmentInventoryInspectionRepository { get; }
        IGenericRepository<DisposalandReplacementRecommendation> DisposalandReplacementRecommendationRepository { get; }
        IGenericRepository<DisposalAndReplacementApproval> DisposalAndReplacementApprovalRepository { get; }
        IGenericRepository<AnnualInventoryInspectionApproval> AnnualInventoryInspectionApprovalRepository { get; }

        #endregion

        #region 3 - Equipment Plan and Evaluation
        IGenericRepository<EquipmentPlanandEvaluations> EquipmentPlanandEvaluationsRepository { get; }
        IGenericRepository<PlanAndEvaluationApproval> PlanAndEvaluationApprovalRepository { get; }

        #endregion

        #region 2 - Fleet Equipment Rent
        IGenericRepository<RentCustomerFeedBack> RentCustomerFeedBackRepository { get; }
        IGenericRepository<RentInternalProject> RentInternalProjectRepository { get; }
        IGenericRepository<RentOtherSuppliers> RentOtherSuppliersRepository { get; }
        IGenericRepository<RentRequest> RentRequestRepository { get; }
        IGenericRepository<RentFromExternal> RentFromExternalRepository { get; }
        IGenericRepository<RentFromExternalApproval> RentFromExternalApprovalRepository { get; }

        IGenericRepository<RentForInternalApproval> RentForInternalApprovalRepository { get; }
      //  IGenericRepository<RentFromExternalApproval> RentFromExternalApprovalRepository { get; }
        IGenericRepository<RentOtherSupplierApproval> RentOtherSupplierApprovalRepository { get; }
        IGenericRepository<RentRequestApproval> RentRequestApprovalRepository { get; }
     
        #endregion

        #region 5 - Fleet Maintenance
        IGenericRepository<JobStoreRequisition> JobStoreRequisitionRepository { get; } 
        IGenericRepository<JobOrder> JobOrderRepository { get; }
        IGenericRepository<JobOrderDetail> JobOrderDetailRepository { get; }
        IGenericRepository<MaintenanceTool> MaintenanceToolRepository { get; }
        IGenericRepository<MechanicCrew> MechanicCrewRepository { get; }
        IGenericRepository<OtherCompanyMaintenance> OtherCompanyMaintenanceRepository { get; }
        IGenericRepository<Planner> PlannerRepository { get; }
        IGenericRepository<Service> ServiceRepository { get; }
        IGenericRepository<SparePart> SparePartRepository { get; }
        IGenericRepository<ServiceCrew> ServiceCrewRepository { get; }
        IGenericRepository<ServiceSpare> ServiceSpareRepository { get; }
        IGenericRepository<ServiceTool> ServiceToolRepository { get; }
        IGenericRepository<OutsourceMaintenance> OutsourceMaintenanceRepository { get; }
        IGenericRepository<MaintenanceCenter> MaintenanceCenterRepository { get; }
        IGenericRepository<RepairType> RepairTypeRepository { get; }
        IGenericRepository<AdministrativeJobApproval> AdministrativeJobApprovalRepository { get; }
        IGenericRepository<OtherCompanyMaintenaceApproval> OtherCompanyMaintenaceApprovalRepository { get; }
        IGenericRepository<OutsourceMaintenanceApproval> OutsourceMaintenanceApprovalRepository { get; }
        IGenericRepository<TechnicalJobApproval> TechnicalJobApprovalRepository { get; }
     
        #endregion

        #region 6 - Fleet Salvage Store
        IGenericRepository<Cannibalization> CannibalizationRepository { get; }
        IGenericRepository<SalvageStore> SalvageStoreRepository { get; }
        IGenericRepository<SalvageStoreGoodReceiving> SalvageStoreGoodReceivingRepository { get; }
        IGenericRepository<CannibalizationApproval> CannibalizationApprovalRepository { get; }
        IGenericRepository<SalvageStoreApproval> SalvageStoreApprovalRepository { get; }
        IGenericRepository<GoodRecivingNoteApproval> GoodRecivingNoteApprovalRepository { get; }

        #endregion

        #region 7 - Fleet Tire Management
        IGenericRepository<TireIssue> TireIssueRepository { get; }
        IGenericRepository<TireReturn> TireReturnRepository { get; }
        IGenericRepository<TireIssueApproval> TireIssueApprovalRepository { get; }

        IGenericRepository<TireReturnApproval> TireReturnApprovalRepository { get; }

        #endregion

        #region 8 -  Fuel Management
        IGenericRepository<Fuel> FuelRepository { get; }
        IGenericRepository<FuelIssueApproval> FuelIssueApprovalRepository { get; }

        #endregion

        #region 9 - Fleet Resource

        IGenericRepository<AssignedEquipment> AssignedEquipmentRepository { get; }
        IGenericRepository<DriverVehicleTransfer> DriverVehicleTransferRepository { get; }
        IGenericRepository<SitesVehicleTransfer> SitesVehicleTransferRepository { get; }
        IGenericRepository<SiteVechicleTransferDetail> SiteVehicleDetailTransferRepository { get; }
        IGenericRepository<VehicleDemand> VehicleDemandRepository { get; }
        IGenericRepository<VehicleDistribution> VehicleDistributionRepository { get; }
        IGenericRepository<VehicleReturn> VehicleReturnRepository { get; }

#endregion

        void Save();
        bool SaveChanges();
        Task<bool> SaveChangeAsync();
        ExceedDbContext GetContext();
    }
}
