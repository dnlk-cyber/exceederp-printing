using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class EquipmentDistrbutionViewModel : Organization
    {
        public string EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public string PlateNo { get; set; }
        public string OperatorName { get; set; }
        public string ReasonForTravel { get; set; }
        public string EmployeeName { get; set; }
        public decimal StartOdometer { get; set; }
        public decimal EndOdometer { get; set; } 

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

        // breakdown additional info
        public float KmTravelledPerLitre { get; set; }
       
        public float KmTravelledTotal { get; set; }
    
        public float KmInterSite { get; set; }
       
        public float KmGrandTotal { get; set; }
        
        public float ReqFuelInLiter { get; set; }
        
        public float TotalInLitre { get; set; }
      
        public float FromDeppot { get; set; }
      
        public decimal InCash { get; set; }
        
        public float FromProject { get; set; }
       
        public float OilReqForEngine { get; set; }
        
        public float OilReqForOther { get; set; }
        
        public float GreaseReq { get; set; }
       
        public decimal TotalBirr { get; set; }
       
        public decimal TotalPerdium { get; set; }
       
        public decimal FuelRate { get; set; }
        
        public decimal InCashLiter { get; set; }
    }
}
