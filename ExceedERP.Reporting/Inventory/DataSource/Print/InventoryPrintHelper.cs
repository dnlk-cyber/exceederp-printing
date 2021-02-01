using System;
using System.Linq;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    public class InventoryPrintHelper
    {
        public static string GetId(int id)
        {
            string code = "";
            if (id < 10)
            {
                code = "0000" + id;

            }
            else if (id >= 10 && id < 100)
            {
                code = "000" + id; 
            }
            else if (id >= 100 && id < 1000)
            {
                code = "00" + id; 
            }
            else if (id >= 1000 && id < 10000)
            {
                code = "0" + id; 
            }
            else if (id >= 10000)
            {
                code =  id.ToString(); 
            }
             
            return code; 
        }

        public static string GetStoreKeeper(string code)
        {
            ExceedDbContext dbContext = new ExceedDbContext();
            var storeKeeperAssignment = dbContext.StoreKeeperAssignments.FirstOrDefault(x => x.StoreCode == code);
            if (storeKeeperAssignment != null)
            {
                string keeper =  storeKeeperAssignment.StoreKeeper;
                //return EmployeeName(keeper);
            }
            return "";
        }
        public static string StoreName(string code)
        {
            var storeName = "";
            ExceedDbContext dboContext = new ExceedDbContext();
            var store = dboContext.InventoryStores.Find(code);
            if (store != null)
            {
                storeName = store.Name;
                
            }
            return storeName;
        }

        public static string WorkUnitName(int code)
        {
            string structureName = "";
            try
            {
                ExceedDbContext dbContext = new ExceedDbContext();
                var companyStructure = dbContext.MainCompanyStructures.Find(code);
                if (companyStructure != null) structureName = companyStructure.Name;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
            }

            return structureName;
        }
        public static string BranchName(int code)
        {
            string branchname = "";
            ExceedDbContext dbContext = new ExceedDbContext();
            var branch = dbContext.Branch.Find(code);
            if (branch != null) branchname= branch.Name;

            return branchname;
        }
        public static string EmployeeName(int empid)
        {
          
            ExceedDbContext dboContext = new ExceedDbContext();
            var record = dboContext.Person_Employee.Find(empid);
            if (record != null)
            {
                string fullname = record.FirstNameEnglish + " " + record.MiddleNameEnglish + " " + record.LastNameEnglish;
                return fullname;
            }
            return null;
        }

    }
}