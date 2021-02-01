using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class MaintainceDueViewModel
    {
        public System.Guid AssetId { get; set; }
        public string PlateNumber { get; set; }

        public String LastMaintained { get; set; }
        public string ServiceName { get; set; }
        public Nullable<int> LabourCost { get; set; }
        public Guid MaintenaceServiceId { get; set; }
        public int PartCost { get; set; }
        public String PartUsed { get; set; }
        public String Type { get; set; }
        public String DetailType { get; set; }

        public MaintainceDueViewModel()
        { }
        public MaintainceDueViewModel(Guid DueID, Guid AssetId, String PlateNumber, String LastMaintained, String ServiceName, int LabourCost, int PartCost, String PartUsed, String Type)
        {
            this.AssetId = AssetId;
            this.PlateNumber = PlateNumber;
            this.LastMaintained = LastMaintained;
            this.ServiceName = ServiceName;
            this.LabourCost = LabourCost;
            this.PartCost = PartCost;
            this.PartUsed = PartUsed;
            this.Type = Type;
            this.MaintenaceServiceId = DueID;

        }
        public MaintainceDueViewModel(Guid DueID, Guid AssetId, String PlateNumber, String LastMaintained, String ServiceName, int LabourCost, int PartCost, String PartUsed, Guid MaintenaceServiceId)
        {
            this.AssetId = AssetId;
            this.PlateNumber = PlateNumber;
            this.LastMaintained = LastMaintained;
            this.ServiceName = ServiceName;
            this.LabourCost = LabourCost;
            this.MaintenaceServiceId = MaintenaceServiceId;
            this.PartCost = PartCost;
            this.PartUsed = PartUsed;
            this.MaintenaceServiceId = DueID;

        }

        public MaintainceDueViewModel(Guid DueID, Guid AssetId, String PlateNumber, String LastMaintained, String ServiceName, int LabourCost, int PartCost, String PartUsed, String Type, String DetailType)
        {
            this.AssetId = AssetId;
            this.PlateNumber = PlateNumber;
            this.LastMaintained = LastMaintained;
            this.ServiceName = ServiceName;
            this.LabourCost = LabourCost;
            this.PartCost = PartCost;
            this.PartUsed = PartUsed;
            this.Type = Type;
            this.DetailType = DetailType;
            this.MaintenaceServiceId = DueID;
        }
    }
}