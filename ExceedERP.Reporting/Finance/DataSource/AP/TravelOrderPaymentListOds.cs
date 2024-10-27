using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.AP
{
    [DataObject]
    public class TravelOrderPaymentListOds : List<APTravelOrderPayListVm>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<APTravelOrderPayListVm> GetPaymentList(bool paid, DateTime ? from ,DateTime ? to, object[] workunitid, object[] branchid, string user, string type)
        {
            List<APTravelOrderPayListVm> list = new List<APTravelOrderPayListVm> { };
            var bankReference = _db.BankReferences.Where(x => x.ReferenceType == "Employee" && x.Type == BankAccountType.PAYROLL).ToList();


            if (type == "Travel Order")
            {
                var directPayments = _db.ApDirectPayments
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.SourceId.StartsWith("TRAVEL-ORDER"))
                    .Include(x => x.ApDirectPaymentLines)
                    .AsQueryable();

                if (from != null)
                    directPayments = directPayments.Where(x => x.PaymentDate >= from);
                if (to != null)
                    directPayments = directPayments.Where(x => x.PaymentDate <= to);

                if (!string.IsNullOrEmpty(user))
                    directPayments = directPayments.Where(x => x.CreatedBy == user);

                //if (workunitid!=null && workunitid.Length>0)
                //{    
                //    var workunitlist=new List<int>();
                //    for (int i= 0; i < workunitid.Length;i++)
                //    {
                //        workunitlist.Add(int.Parse((string)workunitid[i])); 
                //    }
                //    orders = orders.Where(x => workunitlist.Contains(x.wo)); 
                //}

                if (branchid != null && branchid.Length > 0)
                {
                    var branchList = new List<int>();
                    for (int i = 0; i < branchid.Length; i++)
                    {
                        branchList.Add(int.Parse((string)branchid[i]));
                    }
                    directPayments = directPayments.Where(x => (x.BranchId == 0 && branchList.Contains(x.BranchId)));
                }

                var orderList = directPayments.ToList();
                foreach (var order in orderList)
                {
                    int did = int.Parse(order.SourceId.Split('_')[1]);

                    var travelOrder = _db.ApTravelOrders.Find(did);
                    var record = new APTravelOrderPayListVm();
                    if (travelOrder != null)
                    {
                        var emp = _db.Person_Employee.Find(travelOrder.EmployeeId);
                        record.EmployeeCode = emp?.EmployeeCode;
                        record.EmployeeFullName = emp?.FullNameaEnglish;

                        var workunit = _db.MainCompanyStructures.Find(travelOrder.OrgStructureId);
                        record.WorkUnit = workunit?.Name;
                        if (emp != null)
                        {
                            var bank = bankReference.Where(x => x.Reference == emp.EmployeeId.ToString()).FirstOrDefault();
                            record.BankAccount = bank?.AccountNumber;
                        }
                    }
                    record.RequestDate = order.PaymentDate;
                    record.DirectPayId = order.ApDirectPaymentId.ToString();
                    record.Amount = order.ApDirectPaymentLines?.Sum(x => x.TotalAmount) ?? 0; ;//order.AdvanceAmount;
                                                                                               //record.Paid=order.Paid? "Yes":"No";

                    list.Add(record);
                }
            }
            else if (type == "Payment Request")
            {
                var directPayments = _db.ApDirectPayments
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.SourceId.StartsWith("PAYMENT-REQUEST"))
                    .Include(x => x.ApDirectPaymentLines)
                    .AsQueryable();

                if (from != null)
                    directPayments = directPayments.Where(x => x.PaymentDate >= from);
                if (to != null)
                    directPayments = directPayments.Where(x => x.PaymentDate <= to);

                if (!string.IsNullOrEmpty(user))
                    directPayments = directPayments.Where(x => x.CreatedBy == user);

                //if (workunitid!=null && workunitid.Length>0)
                //{    
                //    var workunitlist=new List<int>();
                //    for (int i= 0; i < workunitid.Length;i++)
                //    {
                //        workunitlist.Add(int.Parse((string)workunitid[i])); 
                //    }
                //    orders = orders.Where(x => workunitlist.Contains(x.wo)); 
                //}

                if (branchid != null && branchid.Length > 0)
                {
                    var branchList = new List<int>();
                    for (int i = 0; i < branchid.Length; i++)
                    {
                        branchList.Add(int.Parse((string)branchid[i]));
                    }
                    directPayments = directPayments.Where(x => (x.BranchId == 0 && branchList.Contains(x.BranchId)));
                }

                var orderList = directPayments.ToList();
                foreach (var order in orderList)
                {
                    int did = int.Parse(order.SourceId.Split('_')[1]);

                    var paymentRequest = _db.PaymentRequests.Find(did);
                    var record = new APTravelOrderPayListVm();
                    if (paymentRequest != null)
                    {
                        var emp = _db.Person_Employee.Find(paymentRequest.EmployeeId);
                        record.EmployeeCode = emp?.EmployeeCode;
                        record.EmployeeFullName = emp?.FullNameaEnglish;

                        var workunit = _db.MainCompanyStructures.Find(paymentRequest.OrgStuructureId);
                        record.WorkUnit = workunit?.Name;
                        if (emp != null)
                        {
                            var bank = bankReference.Where(x => x.Reference == emp.EmployeeId.ToString()).FirstOrDefault();
                            record.BankAccount = bank?.AccountNumber;
                        }
                    }
                    record.SupplierName = order.SupplierName;
                    record.RequestDate = order.PaymentDate;
                    record.DirectPayId = order.ApDirectPaymentId.ToString();
                    record.Amount = order.ApDirectPaymentLines?.Sum(x => x.TotalAmount) ?? 0; ;//order.AdvanceAmount;
                                                                                               //record.Paid=order.Paid? "Yes":"No";

                    list.Add(record);
                }
            }

             return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPaymentType()
        {
            List<string> list = new List<string> { };

            list.Add("Travel Order");
            list.Add("Payment Request");
            return list;
        }

    }
}
