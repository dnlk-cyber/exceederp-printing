using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class VehicleReceptionForm : TrackUserSettingOperation
    {
        public int VehicleReceptionFormId { get; set; }
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }
        [Display(Name = "Reception")]
        public int RecipientId { get; set; }
        public Guid? VehicleCategoryId { get; set; }
        [Display(Name = "Technician Assigned")]
        public int ? TechnicianAssignedId { get; set; }
        [Display(Name = "Plate No")]
        public string PlateNo { get; set; }
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }
        [Display(Name = "Vehicle Model")]
        public string VehicleModel { get; set; }
        [Display(Name = "Mileage")]
        public decimal Mileage { get; set; }
        [Display(Name = "Fuel")]
        public FuelValue Fuel { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        public Source Source { get; set; }
    }
    public enum Source { External,Internal}
    public enum FuelValue
    {
        [Display(Name = "E-empty")]
        [Description("E-empty")]
        E_empty,
        [Display(Name = "Red light")]
        [Description("Red light")]

        Red_light,
        [Display(Name = "1/2")]
        [Description("1/2")]
        One_over_two,
        [Display(Name = "B/n 1/2 and 3/4")]
        [Description("B/n 1/2 and 3/4")]
        Bn_1_over_2_and_3_over_4,
        [Display(Name = "3/4")]
        [Description("3/4")]
        Three_over_four,
        [Display(Name = "B/n 3/4 and full")]
        [Description("B/n 3/4 and full")]
        Bn_3_over_4_and_full,
        Full,


    }
}
