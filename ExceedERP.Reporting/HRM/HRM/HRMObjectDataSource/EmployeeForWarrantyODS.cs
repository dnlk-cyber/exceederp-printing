using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
   
    [DataObject]
    public class EmployeeForWarrantyODS : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WarrantyLetterViewModel> GetWarrantyLetter()
        {
            List<WarrantyLetterViewModel> warrantyList = new List<WarrantyLetterViewModel>();
       

            var requestedWarrantys = db.ExprianceRequests.Where(x=>x.RequestType == Core.Domain.Common.SelfServiceRequestType.Warranty_Letter).ToList();
            foreach(var requestedWarranty in requestedWarrantys)
            {
                var waranty = new WarrantyLetterViewModel();
                
                    var approvedRequest = db.RequestesStatus.FirstOrDefault(x => x.RequestId == requestedWarranty.ExperienceRequestId &&
                                                                         x.ReferenceType == "Experience" && x.Status == Core.Domain.Common.RequestStatus.APPROVE);
                if (approvedRequest != null)
                {
                    var empName = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requestedWarranty.EmployeeId);
                    if (empName != null)
                    {
                        waranty.Name = empName.FullNameAmharic;
                        waranty.EmployeeId = empName.EmployeeId;

                        warrantyList.Add(waranty);
                    }
                }
            }

            return warrantyList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
