using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class EquipmentDistributionVewModel : Organization
    { 
       public string Type { get; set; }
    public Nullable<DateTime> StartingDate { get; set; }
    public Nullable<DateTime> ToDate { get; set; }
    public string FromBranchName { get; set; }
    public string FromStructureName { get; set; }

    public string RequestingBranchName { get; set; }

    public string RequestingStructureName { get; set; }
    public string Referance { get; set; }
    public int? RequestNumber { get; set; }
    public Nullable<int> DistrubtionNumber { get; set; }
    public Nullable<int> RequestQuamtity { get; set; }
    public Nullable<int> AssignedQuantity { get; set; }
    public string CategoryRef { get; set; }
    public string EquipmentStatus { get; set; }
    public string RequestDesc { get; set; }

    public bool IsAssigned { get; set; }
}







}
