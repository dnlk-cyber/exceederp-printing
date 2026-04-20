using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class ServiceCheckListController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/CheckList/
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult CheckList_Read([DataSourceRequest] DataSourceRequest request)
        {

            var CheckList = db.CheckLists.Where(x=>x.IsMaintenace == true).Select(e => new

            {
                CheckListId = e.CheckListId,

                Description = e.Description,
                Code = e.Code,
                IsMaintenace =  e.IsMaintenace,
                Type = e.Type,
                Remark = e.Remark

            }
        ).ToList();

            return Json(CheckList.ToDataSourceResult(request));

        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckList_Create([DataSourceRequest] DataSourceRequest request, CheckList CheckList)
        {
            List<CheckList> checklist = db.CheckLists.Where(x=>x.IsMaintenace == true).OrderByDescending(p => p.CheckListId).ToList();
            List<CheckList> list = db.CheckLists.Where(p => p.Type == CheckList.Type && p.IsMaintenace == true).ToList();

            if (list.Any())
            {
                ModelState.AddModelError("DuplicateCheckListType", " CheckList Type Already Registered");
            }
            else
            {

                int CId = 0;
                string alphIndex = string.Empty;

                if (checklist.Any())
                {
                    CId = checklist.Count;
                }
                if (checklist.Count + 2 < 25)
                {
                    alphIndex = MaintenanceConst.ABC[CId].ToString();
                }
                else
                {
                    int Newindex = CId - 26;
                    alphIndex = "A";
                    alphIndex += MaintenanceConst.ABC[Newindex].ToString();
                }
                if (CheckList != null)
                {
                    var entity = new CheckList
                    {

                        CheckListId = CheckList.CheckListId,
                        Type = CheckList.Type,
                        Code = alphIndex,
                        Description = CheckList.Description,
                        Remark = CheckList.Remark,
                        IsMaintenace= true,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db = new ExceedDbContext();

                    db.CheckLists.Add(entity);
                    db.SaveChanges();
                    CheckList.CheckListId = entity.CheckListId;
                }
            }

            return Json(new[] { CheckList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckList_Update([DataSourceRequest] DataSourceRequest request, CheckList CheckList)
        {
            List<CheckList> list = db.CheckLists.Where(p => p.Type == CheckList.Type && p.CheckListId != CheckList.CheckListId).ToList();

            if (list.Any())
            {
                ModelState.AddModelError("DuplicateCheckListType", " CheckList Type Already Registered");
            }
            else
            {

                if (CheckList != null)
                {
                    CheckList.LastModified = DateTime.Now;
                    CheckList.ModifiedBy = User.Identity.Name;

                    db.CheckLists.Attach(CheckList);
                    db.Entry(CheckList).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { CheckList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckList_Destroy([DataSourceRequest] DataSourceRequest request, CheckList CheckList)
        {
            var CheckListObj = db.CheckLists.Where(x => x.CheckListId == CheckList.CheckListId).FirstOrDefault();
            if (CheckList != null)
            {

                db.CheckLists.Attach(CheckListObj);
                db.CheckLists.Remove(CheckListObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted CheckList number " + CheckList.CheckListId);

            }

            return Json(new[] { CheckList }.ToDataSourceResult(request));
        }
        
        public ActionResult GetCheckList()
        {
            List<CheckListViewModel> list = new List<CheckListViewModel>();
            List<CheckList> checkLists = db.CheckLists.OrderBy(x => x.Code).ToList();


            foreach (var checkList in checkLists)
            {
                List<LineCheckList> lines = db.LineCheckLists.Where(x => x.CheckListId == checkList.CheckListId).ToList();
                string Category = Enum.GetName(typeof(CheckListType), checkList.Type);
                foreach (var line in lines)
                {
                    list.Add(new CheckListViewModel { LineId = line.LineCheckListId, CategoryName = checkList.Code + " ( " + Category + " ) ", IsActive = "false", IsActiveInspection = false, LineCode = line.Code });
                }

            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        #region Auto Increment Alphabet

        private String Number2String(int number, bool isCaps)
        {
            Char c = (Char)((isCaps ? 65 : 97) + (number - 1));
            return c.ToString();
        }
        private string ExcelColumnIndexToName(int Index)
        {
            string range = string.Empty;
            if (Index < 0) return range;
            int a = 26;
            int x = (int)Math.Floor(Math.Log((Index) * (a - 1) / a + 1, a));
            Index -= (int)(Math.Pow(a, x) - 1) * a / (a - 1);
            for (int i = x + 1; Index + i > 0; i--)
            {
                range = ((char)(65 + Index % a)).ToString() + range;
                Index /= a;
            }
            return range;
        }
        private static string incCol(string col)
        {
            if (col == "") return "A";
            string fPart = col.Substring(0, col.Length - 1);
            char lChar = col[col.Length - 1];
            if (lChar == 'Z') return incCol(fPart) + "A";
            return fPart + ++lChar;
        }


        #endregion
    }
}

