using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class ItemController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }
        private void PopulateCategories()
        {
            var dataContext = new ExceedDbContext();
            var categories = dataContext.ItemCategorys.Select(i => new {
                ItemCategoryCode = i.ItemCategoryCode,
                Name = i.ItemCategoryCode + "" + i.Name
            })
            .AsQueryable();

            ViewData["categories"] = categories;
            //ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        public ActionResult Items_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InventoryItem> items = _db.InventoryItems;
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                ItemCode = item.ItemCode,
                Name = item.Name,
                Type = item.Type,
                ItemCategoryCode = item.ItemCategoryCode,
                SubCategoryCode = item.SubCategoryCode,
                Balance = item.Balance,
                Enable = item.Enable,
                AvgPrice = item.AvgPrice,
                TotalPrice = item.TotalPrice,
                UoMName = item.UoMName,
                PartNo = item.PartNo,
                ItemIndex=item.ItemIndex,
                Prefix = item.Prefix,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        // [ValidateAntiForgeryToken]
        public ActionResult Items_Create([DataSourceRequest]DataSourceRequest request, InventoryItem item)
        {
            ModelState.Clear();

            int itemcodeLength = 4;//default
            bool autogenerateCode = false;//true
            string prefixstring = "";

            #region get inventory setting

            var inventorySetting = _db.Configurations.Where(x => x.Category == "Inventory").ToList();
            if (!inventorySetting.Any())
                ModelState.AddModelError("setting", "Inventory Setting is not set");

            var itemCodeLengthSetting = inventorySetting
                                 .Where(x => x.Reference == "ITEM_CODE_LENGTH")
                                .Where(x => x.Attribute == "Quantity").FirstOrDefault(); 
            if (itemCodeLengthSetting != null)
                try
                {
                    itemcodeLength = int.Parse(itemCodeLengthSetting.CurrentValue);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("ITEM_CODE_LENGTH", "ITEM_CODE_LENGTH Setting is not set");
                }

            var itemCodeGenerateSetting = inventorySetting.Where(x => x.Reference == "ITEM_CODE_AUTO_GENERATE").FirstOrDefault();
            if (itemCodeGenerateSetting != null)
                try
                {
                    autogenerateCode = itemCodeGenerateSetting.Attribute == "TRUE" ? true:false ;
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("ITEM_CODE_AUTO_GENERATE", "ITEM_CODE_AUTO_GENERATE Setting is not set");
                }
            #endregion

            if (autogenerateCode)
            {
                item.ItemCode = "";
                #region get next code

                int index = 0;              


                if (!String.IsNullOrWhiteSpace(item.SubCategoryCode))
                    prefixstring = /*item.ItemCategoryCode + "-" +*/ item.SubCategoryCode;
                else
                {
                    prefixstring = item.ItemCategoryCode + "-" + "0".PadLeft(3, '0');
                }

                var nextIndex = _db.InventoryItems.Where(x => x.Prefix == prefixstring);

                if (nextIndex.Any())
                    index = nextIndex.Max(x => x.ItemIndex);
                item.ItemIndex = index + 1;
                item.ItemCode = prefixstring + "-" + item.ItemIndex.ToString().PadLeft(itemcodeLength, '0');

                if (String.IsNullOrWhiteSpace(item.ItemCode))
                    ModelState.AddModelError("code", "Could not generate item code.");
                #endregion
            }

            #region validation

            if (String.IsNullOrEmpty(item.ItemCategoryCode))
                ModelState.AddModelError("Item Category Code", "Item categorycode can't be empty.");

            if (String.IsNullOrEmpty(item.ItemCode))
                ModelState.AddModelError("Item Code", "Item code can't be empty.");

            if (String.IsNullOrEmpty(item.Name))
                ModelState.AddModelError("Item Name", "Item name can't be empty.");

            var exists = _db.InventoryItems.Where(x => x.ItemCode == item.ItemCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Item code",
                    item.ItemCode + " already exists in the system.");
            }

            if (!String.IsNullOrEmpty(item.PartNo))
            {
                var partnoexists =
                    _db.InventoryItems.Where(x => x.PartNo == item.PartNo).ToList();

                if (partnoexists.Any())
                    ModelState.AddModelError("Part No", item.PartNo + " already exists in the system.");
            }

            var nameexists = _db.InventoryItems.Where(x => x.ItemCode != item.ItemCode).Where(x => x.Name == item.Name).ToList();

            if (nameexists.Any())
            {
                ModelState.AddModelError("Item Name", item.Name + " already exists in the system.");
            }


            #endregion

            if (ModelState.IsValid)
            {
                    var entity = new InventoryItem
                    {
                        ItemCode = item.ItemCode,
                        SubCategoryCode = item.SubCategoryCode,
                        Name = item.Name,
                        Type = item.Type,
                        ItemCategoryCode = item.ItemCategoryCode,
                        Balance = item.Balance,
                        Enable = true,
                        AvgPrice = item.AvgPrice,
                        UoMName = item.UoMName,
                        PartNo = item.PartNo,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ItemIndex = item.ItemIndex,
                        Prefix=prefixstring,
                    };


                    _db.InventoryItems.Add(entity);
                    _db.SaveChanges();
                    item.ItemCode = entity.ItemCode;
                    CacheHelper.ResetCache();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory item", "Success", User.Identity.Name, $"created item {entity.ItemCode}  ", Modules.Inventory);
                #endregion

            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Items_Update([DataSourceRequest]DataSourceRequest request, InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;

                var nameexists = _db.InventoryItems.Where(x => x.ItemCode != item.ItemCode)
                    .Where(x => x.Name == item.Name).ToList();

                if (nameexists.Any())
                {
                    ModelState.AddModelError("Item Name", item.Name + " already exists in the system.");
                    valid = false;
                }


                #endregion

                if (valid)
                {
                    var entity = new InventoryItem
                    {
                        ItemCode = item.ItemCode,
                        ItemCategoryCode = item.ItemCategoryCode,
                        SubCategoryCode = item.SubCategoryCode,
                        Name = item.Name,
                        Type = item.Type,
                        Balance = item.Balance,
                        Enable = item.Enable,
                        AvgPrice = item.AvgPrice,
                        UoMName = item.UoMName,
                        PartNo = item.PartNo,
                        ItemIndex = item.ItemIndex,
                        Prefix = item.Prefix,
                        CreatedBy = item.CreatedBy,
                        DateCreated = item.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    _db.InventoryItems.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    CacheHelper.ResetCache();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "inventory item", "Success", User.Identity.Name, $"updated item {entity.ItemCode}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Items_Destroy([DataSourceRequest]DataSourceRequest request, InventoryItem item)
        {
            var storeitem = _db.StoreItemAssignments.Where(x => x.ItemCode == item.ItemCode).ToList();
            var branchitem = _db.BranchItemAssignments.Where(x => x.ItemCode == item.ItemCode).ToList();


            var sum = storeitem.Sum(x => x.Balance);
            if (sum > 0)
                ModelState.AddModelError("balance", $"The item {item.ItemCode} hast total balance of {sum} ,hence can't delete");

            if (ModelState.IsValid)
            {
                var entity = new InventoryItem
                {
                    ItemCode = item.ItemCode,
                    Name = item.Name,
                    Type = item.Type,
                    ItemCategoryCode = item.ItemCategoryCode,
                    Balance = item.Balance,
                    Enable = item.Enable,
                    AvgPrice = item.AvgPrice,
                    UoMName = item.UoMName,
                    PartNo = item.PartNo
                };

                _db.InventoryItems.Attach(entity);
                _db.InventoryItems.Remove(entity);
                _db.SaveChanges();

                _db.StoreItemAssignments.RemoveRange(storeitem);
                _db.SaveChanges();
                _db.BranchItemAssignments.RemoveRange(branchitem);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item  " + item.ItemCode);
                #endregion
                CacheHelper.ResetCache();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var item = _db.InventoryItems.FirstOrDefault(p => p.ItemCode == id);
                if (item != null && item.Enable)
                {
                    item.Enable = false;
                    #region disable log
                    UserHelper.OperationLog(Request.UserHostAddress, "Disable", "Success", User.Identity.Name, "Disabled item  " + item.Name,Modules.Inventory);
                    #endregion
                }
                else
                {
                    if (item != null)
                    {
                        item.Enable = true;
                        #region enable log
                        UserHelper.OperationLog(Request.UserHostAddress, "Enable", "Success", User.Identity.Name, "Enabled item  " + item.Name,Modules.Inventory);
                        #endregion
                    }
                }
                if (item != null)
                {
                    _db.InventoryItems.Attach(item);
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                CacheHelper.ResetCache();
            }

            var formattedData = new
            {
                Message = "Operation Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Items_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> files, InventoryItem item)
        {
            ExceedDbContext db= new ExceedDbContext();
            db.Configuration.ProxyCreationEnabled = false;
            #region Import

            int itemcodeLength = 4;//default
            bool autogenerateCode = false;//true

            #region get inventory setting

            var inventorySetting = _db.Configurations.Where(x => x.Category == "Inventory").ToList();
            if (!inventorySetting.Any())
                ModelState.AddModelError("setting", "Inventory Setting is not set");

            var itemCodeLengthSetting = inventorySetting
                                 .Where(x => x.Reference == "ITEM_CODE_LENGTH")
                                .Where(x => x.Attribute == "Quantity").FirstOrDefault();
            if (itemCodeLengthSetting != null)
                try
                {
                    itemcodeLength = int.Parse(itemCodeLengthSetting.CurrentValue);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("ITEM_CODE_LENGTH", "ITEM_CODE_LENGTH Setting is not set");
                }

            var itemCodeGenerateSetting = inventorySetting.Where(x => x.Reference == "ITEM_CODE_AUTO_GENERATE").FirstOrDefault();
            if (itemCodeGenerateSetting != null)
                try
                {
                    autogenerateCode = itemCodeGenerateSetting.Attribute == "TRUE" ? true : false;
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("ITEM_CODE_AUTO_GENERATE", "ITEM_CODE_AUTO_GENERATE Setting is not set");
                }
            #endregion
            
            //var item = _db.InventoryItems.FirstOrDefault();
            DataSet ds = new DataSet();
            foreach (HttpPostedFileBase fileBase in files)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["files"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                {
                    string fileLocation = Server.MapPath("~/Content/Import/Inventory") +
                                          DateTime.Now.Year.ToString() +
                                          DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                          DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                          DateTime.Now.Second.ToString() + Request.Files["files"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["files"].SaveAs(fileLocation);
                    var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsm")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = $"Select * from [{excelSheets[0]}]";
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                    #region collect data

                    var defaultunitset = _db.UoMs.FirstOrDefault(x => x.Default);

                    var defaultUnit = defaultunitset ?? _db.UoMs.FirstOrDefault();

                    //List<InventoryItem> imports = new List<InventoryItem>();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                        {
                            string prefixstring = "";
                            int itemIndex = 0;

                            string catCode = ds.Tables[0].Rows[j].ItemArray[0].ToString();
                            string subCatCode = ds.Tables[0].Rows[j].ItemArray[1].ToString();
                            string itemCode = ds.Tables[0].Rows[j].ItemArray[2].ToString().Trim();
                            string name = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                            string partNo = ds.Tables[0].Rows[j].ItemArray[4].ToString();
                            string unit = ds.Tables[0].Rows[j].ItemArray[5].ToString();

                            if (autogenerateCode)
                            {
                                #region get next code   
                                int index = 0;

                               
                                if (!String.IsNullOrWhiteSpace(subCatCode))
                                    prefixstring = catCode + "-" + subCatCode;
                                else
                                {
                                    prefixstring = catCode + "-" + "0".PadLeft(3, '0');
                                }

                                var nextIndex = db.InventoryItems.Where(x => x.Prefix == prefixstring);

                                if (nextIndex.Any())
                                    index = nextIndex.Max(x => x.ItemIndex);
                                 itemIndex = index + 1;
                                itemCode = prefixstring + "-" + itemIndex.ToString().PadLeft(itemcodeLength, '0');

                                #endregion
                            }
                            var category = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == catCode);
                            var subCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == subCatCode);
                            var unitObj = db.UoMs.FirstOrDefault(x => x.UoMName == unit);

                            if (!String.IsNullOrWhiteSpace(itemCode)&&category!=null)
                            {
                                var exisit = db.InventoryItems.Find(itemCode);
                                if (exisit == null)
                                {
                                    var import = new InventoryItem
                                    {
                                        ItemCode = itemCode,
                                        ItemCategoryCode = catCode,
                                        SubCategoryCode = subCatCode,
                                        Name = name,
                                        Type = ItemTypes.StockCommonItem,
                                        UoMName = unitObj != null ? unitObj.UoMName : defaultUnit?.UoMName,
                                        PartNo = partNo,
                                        Balance = 0,
                                        Enable = true,
                                        AvgPrice = 0,
                                        ItemIndex = itemIndex,
                                        Prefix = prefixstring,
                                        CreatedBy = User.Identity.Name,
                                        DateCreated = DateTime.Now
                                    };

                                    // imports.Add(import);
                                    db.InventoryItems.Add(import);
                                    db.SaveChanges();
                                    

                                    #region operation log
                                    UserHelper.OperationLog(Request.UserHostAddress, "inventory item ", "Success", User.Identity.Name, $"importred item {itemCode}  ", Modules.Inventory);
                                    #endregion
                                }
                            }
                        }
                    }

                    //if (ModelState.IsValid)
                    //{
                    //    _db.InventoryItems.AddRange(imports);
                    //    _db.SaveChanges();
                    //}

                    #endregion

                }

            }


            #endregion

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult UpdateBalance()
        {
            var items = _db.InventoryItems
                //.Include(x=>x.StoreItemAssignments)
                .AsNoTracking().ToList();
            foreach(var i in items)
            {
                var assignments = _db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).ToList();
                if (assignments.Any())
                {
                    i.Balance = _db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).Sum(x => x.Balance);
                    _db.InventoryItems.Attach(i);
                    _db.Entry(i).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }  
            var formattedData = new
            {
                Message = "Operation complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
