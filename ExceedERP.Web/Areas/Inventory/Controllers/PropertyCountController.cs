using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{

	public class PropertyCountController : Controller
	{
		private ExceedDbContext db = new ExceedDbContext();
		private ApplicationDbContext userDb = new ApplicationDbContext();
		private readonly object countLock = new object();


		[AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
		public ActionResult Index()
		{
			PopulateViewData();
			return View();
		}
		private void PopulateViewData()
		{
			var branches = db.Branch;
			var stores = db.InventoryStores;

			var categories = db.ItemCategorys;
			var items = db.InventoryItems;
			var fiscalYears = db.GLFiscalYears;
			var periods = db.InventoryPeriods;
			//var users = userDb.Users;
			var keepers = db.StoreKeeperAssignments.DistinctByField(x => x.StoreKeeper).Select(x => x.StoreKeeper).ToList();
			var users = userDb.Users.Where(x => keepers.Contains(x.Id)).Select(p => new
			{
				Code = p.Id,
				Name = p.UserName + "(" + p.FirstName + " " + p.MiddleName + " " + p.LastName + ")"
			});

			ViewData["users"] = users;

			ViewData["Years"] = fiscalYears;
			ViewData["periods"] = periods;

			ViewData["Items"] = items;
			ViewData["Category"] = categories;
			ViewData["defaultCategory"] = categories.FirstOrDefault();
			ViewData["Stores"] = stores;
			ViewData["branches"] = branches;
			ViewData["defaultCategory"] = branches.FirstOrDefault();

			
		}


		[AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
		public ActionResult PropertyCounts_Read([DataSourceRequest]DataSourceRequest request)
		{
			//List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
			List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
			IQueryable<PropertyCount> propertycounts = db.PropertyCounts.Where(x => userStores.Contains(x.StoreCode));

			DataSourceResult result = propertycounts.ToDataSourceResult(request, propertyCount => new
			{
				PropertyCountId = propertyCount.PropertyCountId,
				CountDate = propertyCount.CountDate,
				BranchId = propertyCount.BranchId,
				BusinessUnitId = propertyCount.BusinessUnitId,
				StoreCode = propertyCount.StoreCode,
				IsMidOfPeriod = propertyCount.IsMidOfPeriod,
				GlFiscalYearId = propertyCount.GlFiscalYearId,
				GLPeriodId = propertyCount.GLPeriodId,
				StoreKeeperId = propertyCount.StoreKeeperId, //UserInformationHelper.GetUserFullNamebyId(propertyCount.StoreKeeperId),
				IsClosed = propertyCount.IsClosed,
			});

			return Json(result);
		}

		[AuthorizeRoles(InventoryConstants.PropertyCount)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult PropertyCounts_Create([DataSourceRequest]DataSourceRequest request, PropertyCount propertyCount)
		{
			//var exist = db.PropertyCounts.Where(x => x.BranchId == propertyCount.BranchId)
			//    .Where(x => x.StoreCode == propertyCount.StoreCode)
			//    .Where(x => x.CountDate ==propertyCount.CountDate.Date).AsQueryable();

			//once per year
			var exist = db.PropertyCounts
			   .Where(x => x.StoreCode == propertyCount.StoreCode)
			   .Where(x => x.GLPeriodId == propertyCount.GLPeriodId)
			   .Where(x => x.GlFiscalYearId == propertyCount.GlFiscalYearId).AsNoTracking().ToList();

			if (exist.Any())
				ModelState.AddModelError("exist", "Count for selected fiscal year, period and store " + propertyCount.StoreCode + " already exists");

			if (String.IsNullOrEmpty(propertyCount.StoreKeeperId))
				propertyCount.StoreKeeperId =ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreKeeper(propertyCount.StoreCode);

			if (ModelState.IsValid)
			{
				var entity = new PropertyCount
				{
					CountDate =propertyCount.CountDate.Date,
					BranchId = propertyCount.BranchId,
					BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
					StoreCode = propertyCount.StoreCode,
					GlFiscalYearId = propertyCount.GlFiscalYearId,
					GLPeriodId = propertyCount.GLPeriodId,
					StoreKeeperId = propertyCount.StoreKeeperId,
					CreatedBy = User.Identity.Name,
					DateCreated = DateTime.Now,
					OrgBranchName = propertyCount.BranchId.ToString()
				};

				db.PropertyCounts.Add(entity);
				db.SaveChanges();
				propertyCount.PropertyCountId = entity.PropertyCountId;
			   // GetItemsInStore(entity);
			}

			#region disable store

			var store = db.InventoryStores.Find(propertyCount.StoreCode);
			if (store != null)
			{
				store.Enable = false;
				db.Entry(store).State = EntityState.Modified;
				db.SaveChanges();
			}

			#endregion
			return Json(new[] { propertyCount }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(InventoryConstants.PropertyCount)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult PropertyCounts_Update([DataSourceRequest]DataSourceRequest request, PropertyCount propertyCount)
		{
			//var exist = db.PropertyCounts.Where(x => x.PropertyCountId != propertyCount.PropertyCountId)
			//                .Where(x => x.BranchId == propertyCount.BranchId)
			//                .Where(x => x.StoreCode == propertyCount.StoreCode)
			//                .Where(x => x.CountDate == propertyCount.CountDate.Date).AsQueryable();
			//once per year
			var exist = db.PropertyCounts.Where(x => x.PropertyCountId != propertyCount.PropertyCountId)
			   .Where(x => x.StoreCode == propertyCount.StoreCode)
			   .Where(x => x.GLPeriodId == propertyCount.GLPeriodId)
			   .Where(x => x.GlFiscalYearId == propertyCount.GlFiscalYearId).AsNoTracking().ToList();

			if (exist.Any())
				ModelState.AddModelError("exist", "Count for selected fiscal year, period and store " + propertyCount.StoreCode + " already exists");

			if (String.IsNullOrEmpty(propertyCount.StoreKeeperId))
				propertyCount.StoreKeeperId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreKeeper(propertyCount.StoreCode);

			if (ModelState.IsValid)
			{
				var entity = new PropertyCount
				{
					PropertyCountId = propertyCount.PropertyCountId,
					CountDate = propertyCount.CountDate.Date,
					BranchId = propertyCount.BranchId,
					BusinessUnitId = propertyCount.BusinessUnitId,
					StoreCode = propertyCount.StoreCode,
					GlFiscalYearId = propertyCount.GlFiscalYearId,
					GLPeriodId = propertyCount.GLPeriodId,
					StoreKeeperId = propertyCount.StoreKeeperId,
					ModifiedBy = User.Identity.Name,
					LastModified = DateTime.Now,
				};

				db.PropertyCounts.Attach(entity);
				db.Entry(entity).State = EntityState.Modified;
				db.SaveChanges();

				#region disable store

				var store = db.InventoryStores.Find(propertyCount.StoreCode);
				if (store != null)
				{
					store.Enable = false;
					db.Entry(store).State = EntityState.Modified;
					db.SaveChanges();
				}

				#endregion

			}

			return Json(new[] { propertyCount }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(InventoryConstants.PropertyCount)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult PropertyCounts_Destroy([DataSourceRequest]DataSourceRequest request, PropertyCount propertyCount)
		{
			if (ModelState.IsValid)
			{
				var entity = new PropertyCount
				{
					PropertyCountId = propertyCount.PropertyCountId,
					CountDate = propertyCount.CountDate,
					BranchId = propertyCount.BranchId,
					StoreCode = propertyCount.StoreCode,

				};

				db.PropertyCounts.Attach(entity);
				db.PropertyCounts.Remove(entity);
				db.SaveChanges();
				#region delete log
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted property count number " + propertyCount.PropertyCountId + " " + propertyCount.Remark);
				#endregion
			}

			return Json(new[] { propertyCount }.ToDataSourceResult(request, ModelState));
		}

		public JsonResult RefreshStoreItems(int id)
		{
			var countRecord = db.PropertyCounts.Find(id);
			var storeitems = db.StoreItemAssignments
				.Where(x => x.StoreCode == countRecord.StoreCode)
				.Where(x => x.InventoryItem.Enable)
				//.Where(x=>x.ItemCode== "13106-000-695") //test
				.ToList();
			var countItems = db.PropertyCountItems.Where(i => i.PropertyCountId == id).ToList();

			string sortOrder = InventoryConfigurationHelper.GetBinCardRSortOrder();

			foreach (var item in storeitems)
			{
				decimal balanceBeforeCount = item.Balance;
				// temporary change
				//any record just before the current or on the current date
				#region get balance with respect to property count date instead of CURRENT BALANCE
				var binCardLines =
				   db.BinCardLines.Where(b => b.Date <= countRecord.CountDate &&//added =
				   b.StoreItemAssignmentID == item.StoreItemAssignmentID &&
				   //b.ReferenceDoc != null &&
				   !b.ReferenceDoc.StartsWith("Count on date") // exclude count
				   ).AsQueryable();
				if (sortOrder == BincardSortOrderAttributes.DateOfTransaction.ToString())
				{
					var sorted = binCardLines.OrderByDescending(d => d.Date).ThenByDescending(d => d.BinCardLineID).Take(1).ToList();
					if (sorted.Any())
					{
						var targetBinTransaction = sorted[0];
						balanceBeforeCount = targetBinTransaction.BalanceQTY;
					}
				}
				else if (sortOrder == BincardSortOrderAttributes.SequencialId.ToString())
				{
					var sorted = binCardLines.OrderByDescending(d => d.BinCardLineID).Take(1).ToList();
					if (sorted.Any())
					{
						var targetBinTransaction = sorted[0];
						balanceBeforeCount = targetBinTransaction.BalanceQTY;
					}

				}
				
				#endregion


				var countItem = countItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
				if (countItem == null)
				{
					var inventoryitem = db.InventoryItems.Find(item.ItemCode);
					#region missing item

					if (countRecord != null)
					{
						if (inventoryitem != null)
						{
							var propertyCountItem = new PropertyCountItem
							{
								ItemCategoryCode = item.ItemCategoryCode,
								ItemName=inventoryitem.Name,
								ItemCode = item.ItemCode,
								ItemCodeReadOnly = item.ItemCode.Trim(),
								PropertyCountId = id,
								Count = 0,
								//PreviousBalance = item.Balance,
								PreviousBalance = balanceBeforeCount,
								Price = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(item.ItemCode, countRecord.StoreCode),
								Status = false,
								Location=item.ItemLocation,
								Unit = item.InventoryItem.UoMName,
								CreatedBy = "Update missing item and balance"
							};

							db.PropertyCountItems.Add(propertyCountItem);
						}

						db.SaveChanges();
					}

					#endregion
				}
				else
				{
					//if (!countItem.Updated)
					if (!countRecord.IsClosed)
					{
						#region update item item

						//countItem.PreviousBalance = item.Balance;
						countItem.PreviousBalance = balanceBeforeCount;
						db.Entry(countItem).State = EntityState.Modified;
						db.SaveChanges();

						#endregion
					}
				}
			}

			var formattedData1 = new
			{
				Message = "Completed.",
				Status = "OK"
			};
			return Json(formattedData1, JsonRequestBehavior.AllowGet);

		}

		public JsonResult RecalculateAllStoreItems(string id)
		{
			string errorMessage = string.Empty;

			if (string.IsNullOrEmpty(id))
			{
				var formattedData = new
				{
					Message = "There is no valid count selected.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

		   
			var countRecord = db.PropertyCounts.Find(int.Parse(id));
			var storeitems = db.StoreItemAssignments
				//.Where(x=>x.ItemCode== "11411-90-20-0001")
				.Where(x => x.StoreCode == countRecord.StoreCode)
				.ToList();

			foreach (var item in storeitems)
			{

				var binCardLines = db.BinCardLines
					.Where(b => b.Date <= countRecord.CountDate &&
					b.StoreItemAssignmentID == item.StoreItemAssignmentID)//&&
					//b.Reference != countRecord.PropertyCountId.ToString())
				   .AsQueryable();

				if (binCardLines.Any())
				{
					var previousCountBinCard = binCardLines.Where(x => x.ReferenceDoc.Contains("Count")).AsQueryable();

					if (previousCountBinCard.Any())
					{
						DateTime recentCountDate = previousCountBinCard.Where(x => x.ReferenceDoc.Contains("Count")).Max(x => x.Date);
						int previousCountBinCardId = previousCountBinCard.Where(x => x.Date == recentCountDate).Select(x => x.BinCardLineID).FirstOrDefault();

						errorMessage=InventoryBalanceHelper.RecalculateTransactionBincardData(item.StoreItemAssignmentID, previousCountBinCardId, true);

						#region  log
						UserHelper.OperationLog(Request.UserHostAddress, "Bin Card recalculation", "Success", User.Identity.Name, $"Bin Card recalculation done starting for item" +
						   $" {item?.InventoryItem?.ItemCode} {item?.InventoryItem?.Name} from ID {previousCountBinCardId}-count", Modules.Inventory);
						#endregion
					}
					else
					{
						//get fiscal year
						var fiscalyear = db.GLFiscalYears.Find(countRecord.GlFiscalYearId);
						var inyearBincards = binCardLines.Where(x => x.Date >= fiscalyear.DateFrom).AsQueryable();

						if (inyearBincards.Any())
						{
							DateTime firstTransactionDate = inyearBincards.Min(x => x.Date);
							int fisrstBincardId = inyearBincards.Where(x => x.Date == firstTransactionDate).Select(x => x.BinCardLineID).FirstOrDefault();

							errorMessage=InventoryBalanceHelper.RecalculateTransactionBincardData(item.StoreItemAssignmentID, fisrstBincardId);

							#region  log
							UserHelper.OperationLog(Request.UserHostAddress, "Bin Card recalculation", "Success", User.Identity.Name, $"Bin Card recalculation done starting for item" +
							   $" {item?.InventoryItem?.ItemCode} {item?.InventoryItem?.Name} from ID {fisrstBincardId}-count", Modules.Inventory);
							#endregion
						}

						//else manually correct the data if not transaction in the year

					}
				}


			}

			#region operation log
			UserHelper.OperationLog(Request.UserHostAddress, "Count recalculate", "Success", User.Identity.Name, $"recalculated  {countRecord.StoreCode} ", Modules.Inventory);
			#endregion
			if (String.IsNullOrEmpty(errorMessage))
			{
				var formattedData = new
				{
					Message = "Completed.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			else
			{
				var formattedData = new
				{
					Message =errorMessage,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

		}
		public JsonResult RecalculateAllStoreItemsFromBeginning(string id)
		{
			var storeitems = db.StoreItemAssignments
				//.Where(x=>x.ItemCode=="14202-000-00044")
				.Where(x => x.StoreCode == id)
				.ToList();

			DateTime beginningDate = db.BinCardLines.Where(x => x.ReferenceDoc.ToUpper() == "BEGINNING").Min(x => x.Date);

			foreach (var item in storeitems)
			{
				int beginningBinCardId = db.BinCardLines
										.Where(x => x.StoreItemAssignmentID == item.StoreItemAssignmentID)
										.Where(x => x.ReferenceDoc.ToUpper() == "BEGINNING").Where(x => x.Date == beginningDate).Select(x => x.BinCardLineID).FirstOrDefault();

				InventoryBalanceHelper.RecalculateTransactionBincardData(item.StoreItemAssignmentID, beginningBinCardId);

				#region  log
				UserHelper.OperationLog(Request.UserHostAddress, "Bin Card recalculation from beginning", "Success", User.Identity.Name, $"Bin Card recalculation done starting for item" +
				   $" {item?.InventoryItem?.ItemCode} {item?.InventoryItem?.Name} from ID {beginningBinCardId}-count", Modules.Inventory);
				#endregion     

			}

			var formattedData1 = new
			{
				Message = "Completed.",
				Status = "OK"
			};
			return Json(formattedData1, JsonRequestBehavior.AllowGet);

		}
		public JsonResult LoadItemsInStore(string id)
		{
			var countRecord = db.PropertyCounts.Find(int.Parse(id));
			//List<PropertyCountItem> list = new List<PropertyCountItem>();
			lock (countLock)
			{
				var itmeList = db.StoreItemAssignments
					.Where(x=>x.InventoryItem.Enable)
					.Where(x => x.StoreCode == countRecord.StoreCode).ToList();

			 
				foreach (var item in itmeList)
				{
					var inventoryitem = db.InventoryItems.Find(item.ItemCode);                  
					var exist = db.PropertyCountItems
						.Where(x => x.PropertyCountId == countRecord.PropertyCountId)
						.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();

					if (inventoryitem != null && exist == null)
					{
						var propertyCountItem = new PropertyCountItem
						{
							ItemCategoryCode = item.ItemCategoryCode,
							ItemCode = item.ItemCode,
							ItemCodeReadOnly = item.ItemCode.Trim(),
							ItemName = inventoryitem.Name,
							PropertyCountId = countRecord.PropertyCountId,
							Count = 0,
							PreviousBalance = item.Balance,
							Location = item.ItemLocation,
							Unit = inventoryitem.UoMName,
							Price = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(item.ItemCode, countRecord.StoreCode),
							Status = false,
							CreatedBy = "Loading Store items"
						};

						//list.Add(propertyCountItem);
						db.PropertyCountItems.Add(propertyCountItem);
						db.SaveChanges();
					}


				}
				// db.PropertyCountItems.AddRange(list);
				// db.SaveChanges();
				#region remove duplicate items
				db.Database.CommandTimeout = 7200;
				var duplicateItems = db.PropertyCountItems
					.Where(x => x.PropertyCountId == countRecord.PropertyCountId)
					.GroupBy(x => new { x.ItemCode, x.PropertyCountId })
					.Where(g => g.Count() > 1)
					.Select(y => new { Element = y.Key, Counter = y.Count() })
					.AsNoTracking()
					.ToList();

				foreach (var duplicate in duplicateItems)
				{
					var duplicateledgerlines = db.PropertyCountItems
						.Where(x => x.ItemCode == duplicate.Element.ItemCode)
						.Where(x => x.PropertyCountId == duplicate.Element.PropertyCountId).ToList();
					if (duplicateledgerlines.Any())
					{
						var minline = duplicateledgerlines.Min(x => x.PropertyCountItemId);
						var toberemoved = duplicateledgerlines.Where(x => x.PropertyCountItemId != minline).ToList();
						db.PropertyCountItems.RemoveRange(toberemoved);
						db.SaveChanges();
					}

				}

				#endregion
			}
			var formattedData1 = new
			{
				Message = "Completed.",
				Status = "OK"
			};
			return Json(formattedData1, JsonRequestBehavior.AllowGet);
		}

		private string GetStatus(int id)
		{
			var data = db.PropertyCountItems.Where(a => a.PropertyCountId.Equals(id)).Where(x => x.Status == false);
			if (data.Any())
			{
				return "There are items with status not updated.Please update them first";
			}

			var price = db.PropertyCountItems
				.Where(x => x.PropertyCountId==id)
				.Where(x=>x.Count>0)
				.Where(x => x.Price == 0);
			if (price.Any())
			{
				return "There are items with zero price.Please update them first";
			}
			return null;
		}

		public ActionResult PropertyCountPrint(int id)
		{
			ViewBag.Id = id;
			return View();
		}

		[AuthorizeRoles(InventoryConstants.PropertyCountApprover)]
		public JsonResult UpdateZeroBalance(int id)
		{            
			var record = db.PropertyCounts.Find(id);
			var propertyCountItems = db.PropertyCountItems.Where(x => x.PropertyCountId == id)
				.Where(x=>x.Count==0 && x.Status==false).ToList();
			if (record != null)
			{
				foreach (var item in propertyCountItems)
				{
					item.Status = true;
					db.Entry(item).State = EntityState.Modified;
					db.SaveChanges();                

				}

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Count zero count status update", "Success", User.Identity.Name, $"updated  {record.StoreCode} ", Modules.Inventory);
				#endregion
				var formattedData = new
				{
					Message = "Updating zero count items completed.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			var formattedData2 = new
			{
				Message = "The count is not found.",
				Status = "NOT_OK"
			};
			return Json(formattedData2, JsonRequestBehavior.AllowGet);
		}

		[AuthorizeRoles(InventoryConstants.PropertyCountApprover)]
		public JsonResult UpdateBalance(int id)
		{
			string errorMessage = GetStatus(id);
			if (!String.IsNullOrEmpty(errorMessage))
			{
				var formattedData0 = new
				{
					Message = errorMessage,
					Status = "NOT_OK"
				};
				return Json(formattedData0, JsonRequestBehavior.AllowGet);
			}
			var record = db.PropertyCounts.Find(id);
			var propertyCountItems = db.PropertyCountItems.Where(x => x.PropertyCountId == id).ToList();
			if (record != null)
			{
				foreach (var item in propertyCountItems)
				{
					if (!item.Updated)
					{
						var storeItem = db.StoreItemAssignments.FirstOrDefault(x =>
							x.StoreCode == record.StoreCode && x.ItemCode == item.ItemCode);
						if (storeItem != null)
						{
							storeItem.Balance = item.Count;
							storeItem.AvgPrice = item.Price;//added 
							storeItem.ItemLocation = !String.IsNullOrWhiteSpace(item.Location)? item.Location:storeItem.ItemLocation;
							item.Updated = true;
							db.Entry(item).State = EntityState.Modified;
							db.Entry(storeItem).State = EntityState.Modified;
							db.SaveChanges();

							#region bin card line
							var binCardLine = new BinCardLine
							{
								StoreItemAssignment = storeItem,
								ReferenceDoc = "Count on date " + record.CountDate.ToShortDateString(),
								Reference = record.PropertyCountId.ToString(),
								Date = record.CountDate,
								BalanceQTY = storeItem.Balance,
								BalanceUPrice = storeItem.AvgPrice,
								BalanceTPrice = storeItem.AvgPrice * storeItem.Balance,
								TimeStamp = DateTime.Now.Date,
								TransactionType= BinCardTransactionTypes.Count,
								StatusType= BinCardTransactionStatus.Regular,
								TransactionOrder=BinCardTransactionOrder.Count
							};
							db.BinCardLines.Add(binCardLine);
							db.SaveChanges();
							#endregion
						}
						else
						{
							#region create store item

							storeItem = new StoreItemAssignment
							{
								StoreCode = record.StoreCode,
								ItemCategoryCode = item.ItemCategoryCode,
								ItemCode = item.ItemCode,
								Balance = item.Count,
								AvgPrice = item.Price,
								ItemLocation = item.Location,
								CreatedBy = User.Identity.Name,
								DateCreated = DateTime.Now
							};
							item.Updated = true;
							db.StoreItemAssignments.Add(storeItem);
							db.SaveChanges();

							#endregion
							#region bin card line


							var binCardLine = new BinCardLine
							{
								StoreItemAssignment = storeItem,
								ReferenceDoc = "Count on date " + record.CountDate.ToShortDateString(),
								Reference = record.PropertyCountId.ToString(),
								Date = record.CountDate,
								BalanceQTY = storeItem.Balance,
								BalanceUPrice = storeItem.AvgPrice,
								BalanceTPrice = storeItem.AvgPrice * storeItem.Balance,
								TimeStamp = DateTime.Now
							};
							db.BinCardLines.Add(binCardLine);
							db.SaveChanges();



							#endregion

						}

					}

				}

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Count update", "Success", User.Identity.Name, $"updated  {record.StoreCode} ", Modules.Inventory);
				#endregion
				var formattedData = new
				{
					Message = "Updating items completed.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			var formattedData2 = new
			{
				Message = "The count is not found.",
				Status = "NOT_OK"
			};
			return Json(formattedData2, JsonRequestBehavior.AllowGet);
		}

		[AuthorizeRoles(InventoryConstants.PropertyCountApprover)]
		public JsonResult ReverseUpdate(int id)
		{
			var record = db.PropertyCounts.Find(id);
			var propertyCountItems = db.PropertyCountItems.Where(x => x.PropertyCountId == id).ToList();

			#region validate if all are updated

			foreach (var item in propertyCountItems)
			{
				if (!item.Updated)
				{
					var formattedData = new
					{
						Message = "There are items not update, please update before reversing.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

			}

			#endregion

			foreach (var item in propertyCountItems)
			{
				var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == record.StoreCode && x.ItemCode == item.ItemCode);
				if (storeItem != null)
				{
					#region reverse store balance
				   // storeItem.Balance = item.PreviousBalance; if the reverse is after many transaction, current balance will be wrong
					item.Updated = false;
					db.Entry(item).State = EntityState.Modified;
					db.Entry(storeItem).State = EntityState.Modified;
					db.SaveChanges();
					#endregion

				}

			}
			if (record != null)
			{
				var referenceDoc = "Count on date " + record.CountDate.ToShortDateString();
				var reference = record.PropertyCountId.ToString();
				#region remove bin card
				var bincardLines = db.BinCardLines
					.Where(x => x.ReferenceDoc == referenceDoc && x.Reference == reference).ToList();
				db.BinCardLines.RemoveRange(bincardLines);
				db.SaveChanges();

				#endregion
			}
			#region operation log
			UserHelper.OperationLog(Request.UserHostAddress, "Count reverse", "Success", User.Identity.Name, $"reversed  {record.StoreCode} ", Modules.Inventory);
			#endregion
			var formattedData1 = new
			{
				Message = "Reversed Successfully.",
				Status = "OK"
			};
			return Json(formattedData1, JsonRequestBehavior.AllowGet);
		}
		[AuthorizeRoles(InventoryConstants.PropertyCountApprover)]
		public JsonResult CloseCount(int id)
		{
			
			var record = db.PropertyCounts.Find(id);
			
			if (record != null)
			{
				var propertyCountItems = db.PropertyCountItems.Where(x => x.PropertyCountId == id).ToList();
				#region validate if all are updated

				foreach (var item in propertyCountItems)
				{
					if (!item.Updated)
					{
						var formattedData = new
						{
							Message = "There are items not update, please update before closing.\n " + item.ItemCode + "-" + item.InventoryItem.Name,
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

				}
				#endregion

				record.IsClosed = true;
				db.Entry(record).State = EntityState.Modified;
				db.SaveChanges();
			}

			#region operation log
			UserHelper.OperationLog(Request.UserHostAddress, "Count close", "Success", User.Identity.Name, $"closed  {record.StoreCode} ", Modules.Inventory);
			#endregion
			var formattedData1 = new
			{
				Message = "Count is closed Sucessfully.",
				Status = "OK"
			};
			return Json(formattedData1, JsonRequestBehavior.AllowGet);
		}
		[AuthorizeRoles(InventoryConstants.PropertyCount)]
		[HttpPost]
		public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
		{
			var fileContents = Convert.FromBase64String(base64);

			return File(fileContents, contentType, fileName);
		}

		[AuthorizeRoles(InventoryConstants.PropertyCount)]
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
