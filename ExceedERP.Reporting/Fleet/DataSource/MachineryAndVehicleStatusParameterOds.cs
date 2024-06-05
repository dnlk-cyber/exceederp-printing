using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class MachineryAndVehicleStatusParameterOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ListVm> GetEquipmentType()
        {
            var list = db.EquipmentType.Select(s => new ListVm
            {
                Value = s.EquipmentTypeID.ToString(),
                Text = s.EquipmentTypeName
            });
            return list.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ListVm> GetStatus()
        {
            return new List<ListVm>()
            {
                new ListVm{Value = FleetConst.ACTIVE, Text = FleetConst.ACTIVE},
                new ListVm{Value = FleetConst.ONDUTY, Text = FleetConst.ONDUTY},
                new ListVm{Value = FleetConst.OnMaintenance, Text = FleetConst.OnMaintenance},
                new ListVm{Value = FleetConst.Idle, Text = FleetConst.Idle},
                new ListVm{Value = FleetConst.Down, Text = FleetConst.Down}
            };
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ListVm> GetStatusType()
        {
            return new List<ListVm>()
            {
                new ListVm{Value = "false", Text = "Own" },
                new ListVm{Value = "true", Text = "Rental"}
            };
        }

        
    }
    public class FleetConst
    {
        public const string ACTIVE = "Operational";
        public const string ONDUTY = "On Duty";
        public const string OnMaintenance = "Under_Maintenace";
        public const string Idle = "Idle";
        public const string Down = "Down";
    }
    public class ListVm
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
