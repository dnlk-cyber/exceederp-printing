using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{

	[DataObject]
	public class NewPurchasedObjectDataSource : List<NewPurchasedViewModel>
	{
		private ExceedDbContext _db = new ExceedDbContext();
		[DataObjectMethod(DataObjectMethodType.Select)]
		public NewPurchaseHeaderViewModel PurchaseHeader(string periodFrom, string periodTo)
		{

			var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

			NewPurchaseHeaderViewModel headerDs = new NewPurchaseHeaderViewModel();
			if (!String.IsNullOrWhiteSpace(periodFrom) && !String.IsNullOrWhiteSpace(periodTo))
			{
				headerDs.From = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodFrom).DateFrom;
				headerDs.To = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodTo).DateTo;
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
		public List<NewPurchasedViewModel> GetPurchased(string periodFrom, string periodTo, string businessUnitId)
		{
			List<NewPurchasedViewModel> purchasedLists = new List<NewPurchasedViewModel>();

			if (!String.IsNullOrWhiteSpace(periodFrom)&& !String.IsNullOrWhiteSpace(periodTo))
			{
				var perFrom = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodFrom)?.DateFrom;
				var perTo = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodTo)?.DateTo;

			   

				if (perFrom != null && perTo != null)
				{
					var catagories = _db.FixedAssetCategories.ToList();
					var groups = _db.FixedAssetGroups.ToList();
					var branches = _db.Branch.ToList();

					var receivedAssets =_db.FixedAssetReceiveItems
						.Where(x => x.FixedAssetReceive.IsOnlineApproved)
						.Where(x => x.FixedAssetReceive.IsVoid == false)
						.Where(x =>x.FixedAssetReceive.ReceiveDate >= perFrom && x.FixedAssetReceive.ReceiveDate <= perTo)
						.AsQueryable();

					if (!String.IsNullOrEmpty(businessUnitId))
					{
						int did = int.Parse(businessUnitId);
						List<string> stores = InventoryHelper.GetStoreByBusinessUnit(did);

						receivedAssets = receivedAssets.Where(x => stores.Contains(x.FixedAssetReceive.StoreCode));
					}

					foreach (var group in groups)
					{
						var groupCategories = catagories.Where(x => x.FixedAssetGroupID == group.FixedAssetGroupID).ToList();

						foreach (var catagory in groupCategories)
						{
							var catreceivedAssets = receivedAssets.Where(x => x.FixedAssetCategoryID == catagory.FixedAssetCategoryID)
								.AsQueryable();

							foreach (var branch in branches)
							{
							   
								var newPurchasedViewModel = new NewPurchasedViewModel
								{
									Group = group.Name,
									Category = catagory.CategoryName,
									Branch = branch.Name,
								};
								var branchstores = _db.InventoryStores.Where(x => x.BranchId == branch.BranchId)
									.Select(x => x.StoreCode).ToList();

								var branchreceivedAssets = catreceivedAssets.Where(x => branchstores.Contains(x.FixedAssetReceive.StoreCode))
									.AsQueryable();

								if(branchreceivedAssets.Any())
								newPurchasedViewModel.Amount = branchreceivedAssets.Sum(x=>x.UnitPrice*x.Quantity);

								purchasedLists.Add(newPurchasedViewModel);

							}

						}


					}
				}
			}

			return purchasedLists;
		}
	}
}
