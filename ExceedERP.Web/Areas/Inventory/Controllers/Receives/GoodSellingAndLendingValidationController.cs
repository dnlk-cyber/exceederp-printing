using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{

	public class GoodSellingAndLendingValidationController : Controller
	{
		private ExceedDbContext db = new ExceedDbContext();

		[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
		public ActionResult Index()
		{
			return null;
		}

		[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
		public ActionResult GoodSellingAndLendingValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
		{
			IQueryable<GoodSellingAndLendingValidation> goodsellingandlendingvalidations = db.GoodSellingAndLendingValidations.Where(gsl => gsl.GoodSellingAndLendingID == id);
			DataSourceResult result = goodsellingandlendingvalidations.ToDataSourceResult(request, goodSellingAndLendingValidation => new
			{
				GoodSellingAndLendingValidationID = goodSellingAndLendingValidation.GoodSellingAndLendingValidationID,
				GoodSellingAndLendingID = goodSellingAndLendingValidation.GoodSellingAndLendingID,
				Status = goodSellingAndLendingValidation.Status,
				Remark = goodSellingAndLendingValidation.Remark,
				OrgBranchName = goodSellingAndLendingValidation.OrgBranchName,
			});

			return Json(result);
		}

		[AuthorizeRoles(InventoryConstants.PropertyApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult GoodSellingAndLendingValidations_Create([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingValidation goodSellingAndLendingValidation, int id)
		{
			#region Checking the Issue
			var record = db.GoodSellingAndLendings.Find(id);
			var pricesetting = db.AveragePriceSettings.FirstOrDefault();
			if (record != null)
			{
				if (record.IsOnlineApproved)
				{
					string message = "You cannot manipulate this record,it  is already approved";
					ModelState.AddModelError("approved", message);
				}
				{
					if (goodSellingAndLendingValidation.Status == ApprovalStatuses.Approve)
					{
						foreach (var i in record.GoodSellingAndLendingItems)
						{
							var inventoryItem = db.InventoryItems.FirstOrDefault(ic => ic.ItemCode == i.ItemCode);

							if (inventoryItem != null)
							{
								#region qty validation
								if (i.QuantitySold <= 0)
									ModelState.AddModelError("store", "Please fill quantity for all items.");

								#endregion
								#region stock

								string itemcode = inventoryItem.ItemCode;
								var store = db.InventoryStores.FirstOrDefault(sc => sc.StoreCode == record.StoreCode);
								if (store != null)
								{
									string storecode = store.StoreCode;

									decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, storecode);
									if (avgPrice == 0)
									{
										ModelState.AddModelError("invalidValue", " Item Price of " + inventoryItem.Name + " is " + 0);
									}

									StoreItemAssignment storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == itemcode).FirstOrDefault(x => x.StoreCode == storecode);
									if (storeItem == null)
									{
										string message = "Item: " + i.InventoryItem.Name + " is not available in store: " + record.StoreCode;
										ModelState.AddModelError("store", message);

									}

									if (storeItem != null && storeItem.Balance - i.QuantitySold < 0)
									{
										ModelState.AddModelError("enoughQuant", "No Enough Quantity To Issue for item " + i.ItemCode);
									}
								}
								else
								{
									string message = "Store " + record.StoreCode + " is not defined in the system.";
									ModelState.AddModelError("stockItem", message);
								}

								#endregion

							}
							else
							{
								string message = "Item " + i.InventoryItem.Name + " is not defined in the system.";
								ModelState.AddModelError("stockItem", message);
							}
						}
					}
					if (goodSellingAndLendingValidation.Status == ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Rejected";
					}
				}
			}
			else
			{
				ModelState.AddModelError("No Found", "Issue record not found");
			}


			#endregion
			if (ModelState.IsValid)
			{
				if (record != null)
				{
					//validate 
					if (goodSellingAndLendingValidation.Status == ApprovalStatuses.Approve)
					{
						record.GoodSellingAndLendingID = id;
						record.Status = "Approved";
						record.IsOnlineApproved = true;
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
						db.Entry(record).State = EntityState.Modified;
					   // db.SaveChanges();

						#region update stock based on the issue

						string storecode = record.StoreCode;

						foreach (var i in record.GoodSellingAndLendingItems)
						{
							
							if (i.QuantitySold > 0)
							{
								string itemCode = i.ItemCode;
								decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, storecode);
								i.UnitPurchasingPrice = avgPrice;

								#region stock

								var storeItem = db.StoreItemAssignments.FirstOrDefault(
											x => x.StoreCode == storecode && x.ItemCode == itemCode);

								if (storeItem != null)
								{
									if (i.QuantitySold > storeItem.Balance)
									{
										i.QuantitySold = storeItem.Balance;
										storeItem.Balance = 0;

									}
									else if (i.QuantitySold <= storeItem.Balance)
									{
										storeItem.Balance = storeItem.Balance - i.QuantitySold;
									}
									
									db.Entry(i).State = EntityState.Modified;
									// db.SaveChanges();

									db.Entry(storeItem).State = EntityState.Modified;
									// db.SaveChanges();
									#region update bin card

									#region bin card line

									var binCardLine = new BinCardLine
									{
										StoreItemAssignment = storeItem,
										ReferenceDoc = "SalesDonation_" + record.GoodSellingAndLendingID.ToString(),
										Reference = record.Remark,
										Date = record.Date,
										IssuedQTY = i.QuantitySold,
										IssuedUPrice = i.UnitPurchasingPrice,
										IssuedTPrice = i.UnitPurchasingPrice*i.QuantitySold,
										BalanceQTY = storeItem.Balance,
										BalanceUPrice = i.UnitPurchasingPrice,
										BalanceTPrice = i.UnitPurchasingPrice * storeItem.Balance,
										IssuedTo = "",
										//new    
										StoreCode = record.StoreCode,
										GLPeriodId = record.GLPeriodId,
										TransactionType = BinCardTransactionTypes.SalesOrLend,
										StatusType = BinCardTransactionStatus.Regular,
										TransactionOrder = BinCardTransactionOrder.SalesOrLend

									};
									db.BinCardLines.Add(binCardLine);
									// db.SaveChanges();

									#endregion

									#endregion
									var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.Date));
								}

								//if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
								//{
								//	int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);
								//	var branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
								//		.FirstOrDefault(x => x.ItemCode == i.ItemCode);
								//	if (branchItem != null)
								//	{
								//		branchItem.Balance = branchItem.Balance - i.QuantitySold;
								//		db.Entry(branchItem).State = EntityState.Modified;
								//		// db.SaveChanges();
								//	}
								//}

								if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
								{
									var stockItem = db.InventoryItems.Find(itemCode);
									if (stockItem != null)
									{
										stockItem.Balance = stockItem.Balance - i.QuantitySold;
										db.Entry(stockItem).State = EntityState.Modified;
										// db.SaveChanges();
									}
								}

								#endregion
							}

						}


						#endregion

					}

					if (goodSellingAndLendingValidation.Status == ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Rejected";
						db.Entry(record).State = EntityState.Modified;
					   // db.SaveChanges();
					}

					var entity = new GoodSellingAndLendingValidation
					{
						GoodSellingAndLendingID = id,
						Status = goodSellingAndLendingValidation.Status,
						Remark = goodSellingAndLendingValidation.Remark,
						OrgBranchName = goodSellingAndLendingValidation.OrgBranchName,
						DateCreated = DateTime.Now,
						CreatedBy = User.Identity.Name,
					};

					db.GoodSellingAndLendingValidations.Add(entity);
					db.SaveChanges();
					goodSellingAndLendingValidation.GoodSellingAndLendingValidationID =
						entity.GoodSellingAndLendingValidationID;

					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "good selling approval", "Success", User.Identity.Name, $"approved good selling {entity.GoodSellingAndLendingID} ", Modules.Inventory);
					#endregion
				}
			}

			return Json(new[] { goodSellingAndLendingValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(InventoryConstants.PropertyApprover)]
		[AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult GoodSellingAndLendingValidations_Update([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingValidation goodSellingAndLendingValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new GoodSellingAndLendingValidation
				{
					GoodSellingAndLendingValidationID = goodSellingAndLendingValidation.GoodSellingAndLendingValidationID,
					Status = goodSellingAndLendingValidation.Status,
					Remark = goodSellingAndLendingValidation.Remark,
					OrgBranchName = goodSellingAndLendingValidation.OrgBranchName,
				};

				db.GoodSellingAndLendingValidations.Attach(entity);
				db.Entry(entity).State = EntityState.Modified;
				db.SaveChanges();
			}

			return Json(new[] { goodSellingAndLendingValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(InventoryConstants.PropertyApprover)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult GoodSellingAndLendingValidations_Destroy([DataSourceRequest]DataSourceRequest request, GoodSellingAndLendingValidation goodSellingAndLendingValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new GoodSellingAndLendingValidation
				{
					GoodSellingAndLendingValidationID = goodSellingAndLendingValidation.GoodSellingAndLendingValidationID,
					Status = goodSellingAndLendingValidation.Status,
					Remark = goodSellingAndLendingValidation.Remark,
					OrgBranchName = goodSellingAndLendingValidation.OrgBranchName,
				};

				db.GoodSellingAndLendingValidations.Attach(entity);
				db.GoodSellingAndLendingValidations.Remove(entity);
				db.SaveChanges();
			}

			return Json(new[] { goodSellingAndLendingValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(InventoryConstants.PropertyApprover)]
		[HttpPost]
		public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
		{
			var fileContents = Convert.FromBase64String(base64);

			return File(fileContents, contentType, fileName);
		}

		[AuthorizeRoles(InventoryConstants.PropertyApprover)]
		[HttpPost]
		public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
		{
			var fileContents = Convert.FromBase64String(base64);

			return File(fileContents, contentType, fileName);
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}
