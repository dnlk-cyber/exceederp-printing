using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance.Settings
{
    public class VehiclePartDamageType : TrackUserSettingOperation
    {
        public int VehiclePartDamageTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
        public string Code_Name
        {
            get { return Code + "_" + Name; }
        }

    }
    public class VehiclePart : TrackUserSettingOperation
    {
        public int VehiclePartId { get; set; }
        [Display(Name = "Category")]
        public Guid? VehicleCategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public bool ApplyForAll { get; set; }
        public string Description { get; set; }
        public string Code_Name
        {
            get { return Code + "_" + Name; }
        }

    }
}
