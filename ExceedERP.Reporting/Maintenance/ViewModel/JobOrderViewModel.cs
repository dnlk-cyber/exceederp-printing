using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.ViewModel
{
    public class JobOrderViewModel : Organization
    {
        public int Num { get; set; }
        public string JobOrderNo{ get; set; }
        public string Model { get; set; }
        public string Customer { get; set; }
        public string JobOrder { get; set; }
        public string IsRework { get; set; }
        public string Fleets { get; set; }
        public string EquipmentInformation { get; set; }
        public string EquipmentName { get; set; }
        public string OperatorName { get; set; }
        public string AssetNum { get; set; }
        public string DateCreated { get; set; }
        public string IsExternal { get; set; }
        public string MaintenanceCenter { get; set; }
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }
        public decimal Odometer { get; set; }
        public decimal Mileage { get; set; }
        public string FailureReason { get; set; }
        public string AdvanceMeasure { get; set; }
        public string EquipmentNo { get; set; }
        public string EquipmentCategory { get; set; }
        public string EquipmentType { get; set; }
        public string CurrentFuel { get; set; }
        public string Referance { get; set; }
        public Nullable<DateTime> DateIssued { get; set; }
        public Nullable<DateTime> DateCompleted { get; set; }
        public string InspectedBy { get; set; }
        public string IsPM { get; set; }
        public string InspectorComments { get; set; }
        public string LicensePlate { get; set; }
        public string RejectionReason { get; set; }
        public string IsSentToWorkShop { get; set; }
        public string IsSubmittedToServiceCenter { get; set; }
        public string WorkShopRejectedReason { get; set; }
        public string ServiceCenterStatus { get; set; }
        public string Branch { get; set; }
        public string BusinessUnit { get; set; }
        public string OrgStructure { get; set; }
        public string CreatedAtBranch { get; set; }
        public string CreatedAtBusinessUnit { get; set; }
        public string Requester { get; set; }
        public virtual ICollection<JobCauseViewModel> JobCauseViewModels { get; set; }
    }
    public class JobCauseViewModel
    {
        public int No { get; set; }
        public string Description { get; set; }
        public string JobCause { get; set; }
        public string SubCategoryCode { get; set; }
        public ServiceGroup ServiceGroup { get; set; }
       // public string Mechanic { get; set; }
        public string Service { get; set; }
      //  public decimal RegularTime { get; set; }
        //public decimal OverTime { get; set; }
       // public decimal LaborCost { get; set; }
    }
}
