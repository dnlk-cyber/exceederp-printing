using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
	[DataObject]
	public class ApMiscPaymentDs
	{
		readonly ExceedDbContext _db = new ExceedDbContext();

		[DataObjectMethod(DataObjectMethodType.Select)]
		public ApMiscPaymentViewModel MiscPaymentHeader(int id)
		{
			var payment = _db.APMiscellaneousPayments.Find(id);
			var paymentlines = _db.ApMiscellaneousPaymentLines.Where(x=>x.ApMiscellaneousPaymentId==id);
			decimal netpayment = paymentlines.Any() ? paymentlines.Sum(x => x.NetPayable):0;
			var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

			ApMiscPaymentViewModel headerDs = new ApMiscPaymentViewModel();

			if (payment != null)
			{
				headerDs.VoucherId = payment.ApMiscellaneousPaymentId.ToString();
				headerDs.DocTitile = payment.IsDirectPayment ? "PAYMENT VOUCHER" : "MISCELLANEOUS PAYMENT VOUCHER";
				headerDs.SupplierName = payment.SupplierName;
				headerDs.NetPayable = netpayment;
				headerDs.PaymentReference = payment.PaymentReference;
				headerDs.PaymentMethod = payment.PaymentMethod;
				headerDs.PaymentDate = payment.PaymentDate;
				headerDs.Remark = payment.Remark;
				headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.CreatedBy));
				headerDs.PreparedDate = payment.DateCreated != null ? payment.DateCreated : null;

				headerDs.AccountantCheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.SendForApprovalBy));
				headerDs.AccountantCheckedDate = payment.SendForApprovalTime != null ? payment.SendForApprovalTime : null;

				headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(payment.OnlineApprovedBy));
				headerDs.ApprovedDate = payment.OnlineApprovedTime;
			}

			if (organization != null)
			{
				headerDs.Name = organization.TradeName;
				headerDs.AmharicName = organization.AmharicName;
				var address =
					_db.Addresses.Where(
						a =>
							a.Reference == organization.MainCompanyId.ToString() &&
							a.ReferenceType.ToLower() == "company").ToList();
				if (address.Count > 0)
				{
					foreach (var a in address)
					{

						if (headerDs.Address == null)
						{
							headerDs.Address = a.Category + ":- " + a.Value;
						}
						else
						{
							headerDs.Address += ", " + a.Category + "-" + a.Value;
						}

					}

				}
				var logo =
					_db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
							a.ReferenceType == AttachmentType.LOGO);
				if (logo != null)
				{
					headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
				}
			}
			return headerDs;
		}
		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<ApMiscellaneousPaymentLine> ApMiscPaymentLines(int id)
		{
			return _db.ApMiscellaneousPaymentLines.Where(x => x.ApMiscellaneousPaymentId == id).ToList();
		}
		
		  [DataObjectMethod(DataObjectMethodType.Select)]
		public List<ApMiscellaneousPaymentDistribution> ApMiscPaymentDistribution(int id)
		{
			return _db.APMiscellaneousPaymentDistributions.Where(x => x.ApMiscellaneousPaymentId == id).ToList();
		}
	}
}
