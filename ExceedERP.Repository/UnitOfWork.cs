
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
    public class UnitOfWork : IUnitOfWork
    {
        #region  private vars

        private readonly ExceedDbContext _context;
       

        public UnitOfWork()
        {
            _context = new ExceedDbContext();
           
        }

        #endregion
       
       
        public ExceedDbContext GetContext()
        {
            return _context;
        }



        #region 1 - Fleet Registration Module

        //Accessories
        private IGenericRepository<Accessories> AccessoriesRepo { get; set; }
        public IGenericRepository<Accessories> AccessoriesRepository
        {
            get { return AccessoriesRepo ?? (AccessoriesRepo = new GenericRepository<Accessories>(_context)); }
        }
        private IGenericRepository<TimeSheet> TimeSheetRepo { get; set; }
        public IGenericRepository<TimeSheet> TimeSheetRepository
        {
            get { return TimeSheetRepo ?? (TimeSheetRepo = new GenericRepository<TimeSheet>(_context)); }
        }

        //Accident
        private IGenericRepository<Accident> AccidentRepo { get; set; }
        public IGenericRepository<Accident> AccidentRepository
        {
            get { return AccidentRepo ?? (AccidentRepo = new GenericRepository<Accident>(_context)); }
        }

        //EquipmentIdentity
        private IGenericRepository<EquipmentIdentity> EquipmentIdentityRepo { get; set; }

        public IGenericRepository<EquipmentIdentity> EquipmentIdentityRepository
        {
            get { return EquipmentIdentityRepo ?? (EquipmentIdentityRepo = new GenericRepository<EquipmentIdentity>(_context)); }

        }

        private IGenericRepository<OperatorPosition> OperatorPositionRepo { get; set; }

        public IGenericRepository<OperatorPosition> OperatorPositionRepository
        {
            get { return OperatorPositionRepo ?? (OperatorPositionRepo = new GenericRepository<OperatorPosition>(_context)); }

        }

        private IGenericRepository<Branch> BranchRepo { get; set; }
        public IGenericRepository<Branch> BranchRepository
        {
            get { return BranchRepo ?? (BranchRepo = new GenericRepository<Branch>(_context)); }
        }
        private IGenericRepository<Structure> StructureRepo { get; set; }
        public IGenericRepository<Structure> StructureRepository
        {
            get { return StructureRepo ?? (StructureRepo = new GenericRepository<Structure>(_context)); }
        }
        private IGenericRepository<EquipmentName> EquipmentNameRepo { get; set; }
        public IGenericRepository<EquipmentName> EquipmentNameRepository
        {
            get { return EquipmentNameRepo ?? (EquipmentNameRepo = new GenericRepository<EquipmentName>(_context)); }
        }
        private IGenericRepository<Manufacturer> ManufacturerRepo { get; set; }

        public IGenericRepository<Manufacturer> ManufacturerRepository
        {
            get { return ManufacturerRepo ?? (ManufacturerRepo = new GenericRepository<Manufacturer>(_context)); }

        }



        private IGenericRepository<FleetHistory> FleetHistoryRepo { get; set; }

        public IGenericRepository<FleetHistory> FleetHistoryRepository
        {
            get { return FleetHistoryRepo ?? (FleetHistoryRepo = new GenericRepository<FleetHistory>(_context)); }

        }
        private IGenericRepository<EquipmentType> EquipmentTypeRepo { get; set; }

        public IGenericRepository<EquipmentType> EquipmentTypeRepository
        {
            get { return EquipmentTypeRepo ?? (EquipmentTypeRepo = new GenericRepository<EquipmentType>(_context)); }

        }

        private IGenericRepository<Fleets> FleetRepo { get; set; }

        public IGenericRepository<Fleets> FleetsRepository
        {
            get { return FleetRepo ?? (FleetRepo = new GenericRepository<Fleets>(_context)); }

        }
        private IGenericRepository<FleetInsurance> InsuranceRepo { get; set; }

        public IGenericRepository<FleetInsurance> InsuranceRepository
        {
            get { return InsuranceRepo ?? (InsuranceRepo = new GenericRepository<FleetInsurance>(_context)); }

      
        }
        private IGenericRepository<Operator> OperatorRepo { get; set; }

        public IGenericRepository<Operator> OperatorRepository
        {
            get { return OperatorRepo ?? (OperatorRepo = new GenericRepository<Operator>(_context)); }

        }

        private IGenericRepository<Educationallevel> EducationallevelRepo { get; set; }

        public IGenericRepository<Educationallevel> EducationallevelRepository
        {
            get { return EducationallevelRepo ?? (EducationallevelRepo = new GenericRepository<Educationallevel>(_context)); }

        }

        #endregion

        #region Equipment Inspection

        private IGenericRepository<AnnualEquipmentInventoryInspection> AnnualEquipmentInventoryInspectionRepo { get; set; }

        public IGenericRepository<AnnualEquipmentInventoryInspection> AnnualEquipmentInventoryInspectionRepository
        {
            get { return AnnualEquipmentInventoryInspectionRepo ?? (AnnualEquipmentInventoryInspectionRepo = new GenericRepository<AnnualEquipmentInventoryInspection>(_context)); }

        }
        private IGenericRepository<DisposalandReplacementRecommendation> DisposalandReplacementRecommendationRepo { get; set; }

        public IGenericRepository<DisposalandReplacementRecommendation> DisposalandReplacementRecommendationRepository
        {
            get { return DisposalandReplacementRecommendationRepo ?? (DisposalandReplacementRecommendationRepo = new GenericRepository<DisposalandReplacementRecommendation>(_context)); }

        }
        #endregion
        #region Equipment Plan and Evaluations
        private IGenericRepository<EquipmentPlanandEvaluations> EquipmentPlanandEvaluationsRepo { get; set; }

        public IGenericRepository<EquipmentPlanandEvaluations> EquipmentPlanandEvaluationsRepository
        {
            get { return EquipmentPlanandEvaluationsRepo ?? (EquipmentPlanandEvaluationsRepo = new GenericRepository<EquipmentPlanandEvaluations>(_context)); }

        }
        #endregion 
        
        #region Fleet Equipment Rent
        private IGenericRepository<RentCustomerFeedBack> RentCustomerFeedBackRepo { get; set; }

        public IGenericRepository<RentCustomerFeedBack> RentCustomerFeedBackRepository
        {
            get { return RentCustomerFeedBackRepo ?? (RentCustomerFeedBackRepo = new GenericRepository<RentCustomerFeedBack>(_context)); }

        }
        private IGenericRepository<RentInternalProject> RentInternalProjectRepo { get; set; }

        public IGenericRepository<RentInternalProject> RentInternalProjectRepository
        {
            get { return RentInternalProjectRepo ?? (RentInternalProjectRepo = new GenericRepository<RentInternalProject>(_context)); }

        }

        private IGenericRepository<RentOtherSuppliers> RentOtherSuppliersRepo { get; set; }
        public IGenericRepository<RentOtherSuppliers> RentOtherSuppliersRepository
        {
            get { return RentOtherSuppliersRepo ?? (RentOtherSuppliersRepo = new GenericRepository<RentOtherSuppliers>(_context)); }

        }

        private IGenericRepository<RentRequest> RentRequestRepo { get; set; }
      
        public IGenericRepository<RentRequest> RentRequestRepository
        {
            get { return RentRequestRepo ?? (RentRequestRepo = new GenericRepository<RentRequest>(_context)); }

        }

        private IGenericRepository<RentFromExternal> RentFromExternalRepo { get; set; }

        public IGenericRepository<RentFromExternal> RentFromExternalRepository
        {
            get { return RentFromExternalRepo ?? (RentFromExternalRepo = new GenericRepository<RentFromExternal>(_context)); }

        }
        #endregion

        #region Fleet Maintenance 
        private IGenericRepository<JobOrder> JobOrderRepo { get; set; }
      
        public IGenericRepository<JobOrder> JobOrderRepository
        {
            get { return JobOrderRepo ?? (JobOrderRepo = new GenericRepository<JobOrder>(_context)); }

        }
        private IGenericRepository<JobStoreRequisition> JobStoreRequisitionRepo { get; set; }

        public IGenericRepository<JobStoreRequisition> JobStoreRequisitionRepository
        {
            get { return JobStoreRequisitionRepo ?? (JobStoreRequisitionRepo = new GenericRepository<JobStoreRequisition>(_context)); }

        }
        private IGenericRepository<AdministrativeJobApproval> AdministrativeJobApprovalRepo { get; set; }

        public IGenericRepository<AdministrativeJobApproval> AdministrativeJobApprovalRepository
        {
            get { return AdministrativeJobApprovalRepo ?? (AdministrativeJobApprovalRepo = new GenericRepository<AdministrativeJobApproval>(_context)); }

        }
        private IGenericRepository<TechnicalJobApproval> TechnicalJobApprovalRepo { get; set; }

        public IGenericRepository<TechnicalJobApproval> TechnicalJobApprovalRepository
        {
            get { return TechnicalJobApprovalRepo ?? (TechnicalJobApprovalRepo = new GenericRepository<TechnicalJobApproval>(_context)); }

        }
        private IGenericRepository<JobOrderDetail> JobOrderDetailRepo { get; set; }
        public IGenericRepository<JobOrderDetail> JobOrderDetailRepository
        {
            get { return JobOrderDetailRepo ?? (JobOrderDetailRepo = new GenericRepository<JobOrderDetail>(_context)); }

        }
        private IGenericRepository<MaintenanceTool> MaintenanceToolRepo { get; set; }
       
        public IGenericRepository<MaintenanceTool> MaintenanceToolRepository
        {
            get { return MaintenanceToolRepo ?? (MaintenanceToolRepo = new GenericRepository<MaintenanceTool>(_context)); }

        }
        private IGenericRepository<MechanicCrew> MechanicCrewRepo { get; set; }
       
        public IGenericRepository<MechanicCrew> MechanicCrewRepository
        {
            get { return MechanicCrewRepo ?? (MechanicCrewRepo = new GenericRepository<MechanicCrew>(_context)); }

        }


       
        private IGenericRepository<MaintenanceCenter> MaintenanceCenterRepo { get; set; }

        public IGenericRepository<MaintenanceCenter> MaintenanceCenterRepository
        {
            get { return MaintenanceCenterRepo ?? (MaintenanceCenterRepo = new GenericRepository<MaintenanceCenter>(_context)); }

        }

        private IGenericRepository<RepairType> RepairTypeRepo { get; set; }

        public IGenericRepository<RepairType> RepairTypeRepository
        {
            get { return RepairTypeRepo ?? (RepairTypeRepo = new GenericRepository<RepairType>(_context)); }

        }

        private IGenericRepository<OtherCompanyMaintenance> OtherCompanyMaintenanceRepo { get; set; }
       
        public IGenericRepository<OtherCompanyMaintenance> OtherCompanyMaintenanceRepository
        {
            get { return OtherCompanyMaintenanceRepo ?? (OtherCompanyMaintenanceRepo = new GenericRepository<OtherCompanyMaintenance>(_context)); }

        }

        private IGenericRepository<Planner> PlannerRepo { get; set; }
   
        public IGenericRepository<Planner> PlannerRepository
        {
            get { return PlannerRepo ?? (PlannerRepo = new GenericRepository<Planner>(_context)); }

        }

        private IGenericRepository<Service> ServiceRepo { get; set; }
   
        public IGenericRepository<Service> ServiceRepository
        {
            get { return ServiceRepo ?? (ServiceRepo = new GenericRepository<Service>(_context)); }

        }
        private IGenericRepository<SparePart> SparePartRepo { get; set; }
   
        public IGenericRepository<SparePart> SparePartRepository
        {
            get { return SparePartRepo ?? (SparePartRepo = new GenericRepository<SparePart>(_context)); }

        }
        private IGenericRepository<ServiceCrew> ServiceCrewRepo { get; set; }

        public IGenericRepository<ServiceCrew> ServiceCrewRepository
        {
            get { return ServiceCrewRepo ?? (ServiceCrewRepo = new GenericRepository<ServiceCrew>(_context)); }

        }
        private IGenericRepository<ServiceSpare> ServiceSpareRepo { get; set; }

        public IGenericRepository<ServiceSpare> ServiceSpareRepository
        {
            get { return ServiceSpareRepo ?? (ServiceSpareRepo = new GenericRepository<ServiceSpare>(_context)); }

        }
        private IGenericRepository<ServiceTool> ServiceToolRepo { get; set; }

        public IGenericRepository<ServiceTool> ServiceToolRepository
        {
            get { return ServiceToolRepo ?? (ServiceToolRepo = new GenericRepository<ServiceTool>(_context)); }

        }
        private IGenericRepository<OutsourceMaintenance> OutsourceMaintenanceRepo { get; set; }
        public IGenericRepository<OutsourceMaintenance> OutsourceMaintenanceRepository
        {
            get { return OutsourceMaintenanceRepo ?? (OutsourceMaintenanceRepo = new GenericRepository<OutsourceMaintenance>(_context)); }

        }
        #endregion

        #region Salvage Store

        private IGenericRepository<Cannibalization> CannibalizationRepo { get; set; }
   
        public IGenericRepository<Cannibalization> CannibalizationRepository
        {
            get { return CannibalizationRepo ?? (CannibalizationRepo = new GenericRepository<Cannibalization>(_context)); }
        }
        private IGenericRepository<SalvageStore> SalvageStoreRepo { get; set; }
   
        public IGenericRepository<SalvageStore> SalvageStoreRepository
        {
            get { return SalvageStoreRepo ?? (SalvageStoreRepo = new GenericRepository<SalvageStore>(_context)); }
        }
        private IGenericRepository<SalvageStoreGoodReceiving> SalvageStoreGoodReceivingRepo { get; set; }

        public IGenericRepository<SalvageStoreGoodReceiving> SalvageStoreGoodReceivingRepository
        {
            get { return SalvageStoreGoodReceivingRepo ?? (SalvageStoreGoodReceivingRepo = new GenericRepository<SalvageStoreGoodReceiving>(_context)); }
        }
        #endregion

        #region Tire 

        private IGenericRepository<TireIssue> TireIssueRepo { get; set; }
   
        public IGenericRepository<TireIssue> TireIssueRepository
        {
            get { return TireIssueRepo ?? (TireIssueRepo = new GenericRepository<TireIssue>(_context)); }
    
        }
        private IGenericRepository<TireReturn> TireReturnRepo { get; set; }
   
        public IGenericRepository<TireReturn> TireReturnRepository
        {
            get { return TireReturnRepo ?? (TireReturnRepo = new GenericRepository<TireReturn>(_context)); }
    
        }
        #endregion

        #region Fuel

        private IGenericRepository<Fuel> FuelRepo { get; set; }
   
        public IGenericRepository<Fuel> FuelRepository
        {
            get { return FuelRepo ?? (FuelRepo = new GenericRepository<Fuel>(_context)); }
    
        }
        #endregion

        #region Fleet Resource


        private IGenericRepository<SiteVechicleTransferDetail> SiteVehicleDetailTransferRepo { get; set; }

        public IGenericRepository<SiteVechicleTransferDetail> SiteVehicleDetailTransferRepository
        {
            get { return SiteVehicleDetailTransferRepo ?? (SiteVehicleDetailTransferRepo = new GenericRepository<SiteVechicleTransferDetail>(_context)); }

        }
        private IGenericRepository<DriverVehicleTransfer> DriverVehicleTransferRepo { get; set; }

        public IGenericRepository<DriverVehicleTransfer> DriverVehicleTransferRepository
        {
            get { return DriverVehicleTransferRepo ?? (DriverVehicleTransferRepo = new GenericRepository<DriverVehicleTransfer>(_context)); }

        }
        private IGenericRepository<SitesVehicleTransfer> SitesVehicleTransferRepo { get; set; }

        public IGenericRepository<SitesVehicleTransfer> SitesVehicleTransferRepository
        {
            get { return SitesVehicleTransferRepo ?? (SitesVehicleTransferRepo = new GenericRepository<SitesVehicleTransfer>(_context)); }

        }
        private IGenericRepository<VehicleDemand> VehicleDemandRepo { get; set; }

        public IGenericRepository<VehicleDemand> VehicleDemandRepository
        {
            get { return VehicleDemandRepo ?? (VehicleDemandRepo = new GenericRepository<VehicleDemand>(_context)); }

        }
        private IGenericRepository<VehicleDistribution> VehicleDistributionRepo { get; set; }

        public IGenericRepository<VehicleDistribution> VehicleDistributionRepository
        {
            get { return VehicleDistributionRepo ?? (VehicleDistributionRepo = new GenericRepository<VehicleDistribution>(_context)); }

        }
        private IGenericRepository<VehicleReturn> VehicleReturnRepo { get; set; }

        public IGenericRepository<VehicleReturn> VehicleReturnRepository
        {
            get { return VehicleReturnRepo ?? (VehicleReturnRepo = new GenericRepository<VehicleReturn>(_context)); }

        }

        private IGenericRepository<AssignedEquipment> AssignedEquipmentRepo { get; set; }

        public IGenericRepository<AssignedEquipment> AssignedEquipmentRepository
        {
            get { return AssignedEquipmentRepo ?? (AssignedEquipmentRepo = new GenericRepository<AssignedEquipment>(_context)); }

        }

        #endregion

        #region UnitOfWork CRUD method(s)

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool SaveChanges()
        {
            try
            {
                return _context.SaveChanges() > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SaveChangeAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                throw;
            }
        }


        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion




        public IGenericRepository<DisposalAndReplacementApproval> DisposalAndReplacementApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<AnnualInventoryInspectionApproval> AnnualInventoryInspectionApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<PlanAndEvaluationApproval> PlanAndEvaluationApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<RentFromExternalApproval> RentFromExternalApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<RentForInternalApproval> RentForInternalApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<RentOtherSupplierApproval> RentOtherSupplierApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<RentRequestApproval> RentRequestApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<OtherCompanyMaintenaceApproval> OtherCompanyMaintenaceApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<OutsourceMaintenanceApproval> OutsourceMaintenanceApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<CannibalizationApproval> CannibalizationApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<SalvageStoreApproval> SalvageStoreApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<GoodRecivingNoteApproval> GoodRecivingNoteApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<TireIssueApproval> TireIssueApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<TireReturnApproval> TireReturnApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }

        public IGenericRepository<FuelIssueApproval> FuelIssueApprovalRepository
        {
            get { throw new NotImplementedException(); }
        }
    }
}
