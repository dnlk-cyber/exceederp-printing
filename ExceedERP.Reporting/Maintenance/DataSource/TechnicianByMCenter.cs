using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
   public class TechnicianByMCenter
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TechnicianListVM> GetTechnicians(string MCId)
        {
            List<TechnicianListVM> list = new List<TechnicianListVM>();
            if(!Guid.TryParse(MCId,out Guid MCenterId))
            {
                return list;
            }
            var FetchBranchId = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == MCenterId).BranchId;
            var FetchPlacement = db.Placements.Where(x => x.BranchId == FetchBranchId&&x.isCurrent).ToList();
            var EmployeeIds = FetchPlacement.Select(x => x.EmployeeId).Distinct();
            var FetchMechanics = db.MechanicCrew.Where(x => EmployeeIds.Contains(x.EmployeeId)).ToList();
            if (FetchMechanics.Any())
            {
              foreach(var Crew in FetchMechanics)
                {
                    var Technician = new TechnicianListVM();
                    Technician.TechnicianId = Crew.MechanicCrewID.ToString();
                    var FetchFullName = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == Crew.EmployeeId);
                    if(FetchFullName!=null)
                    {
                        Technician.TechnicianName = FetchFullName.FirstNameEnglish + " " + FetchFullName.MiddleNameEnglish + " " + FetchFullName.LastNameEnglish;
                    }
                 
                    list.Add(Technician);
                }
            }
            return list;
        }

    }
    public class TechnicianListVM
    {
        public string TechnicianId { get; set; }
        public string TechnicianName { get; set; }
    }
}
