using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class LineCheckListController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/LineLineCheckList/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LineCheckList_Read([DataSourceRequest] DataSourceRequest request, int Id)
        {

            var LineCheckList = db.LineCheckLists.Where(x => x.CheckListId == Id).Select(e => new 

            {
                LineCheckListId = e.LineCheckListId,
                CheckListId = e.CheckListId,
                Name = e.Name,
              Code =  e.Code, 
              Remark = e.Remark,
                Description = e.Description,
                IsActive = e.IsActive


            }  ).ToList();

            return Json(LineCheckList.ToDataSourceResult(request));

        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineCheckList_Create([DataSourceRequest] DataSourceRequest request, LineCheckList LineCheckList, int CId)
        {
            CheckList Check = db.CheckLists.Where(x => x.CheckListId == CId).FirstOrDefault();
            List<LineCheckList> List = db.LineCheckLists.Where(x => x.CheckListId == CId).ToList();
            int index = List.Count +1;

            string code = Check.Code + index.ToString();
                if (LineCheckList != null)
                {
                    var entity = new LineCheckList
                    {

                        Name = LineCheckList.Name,
                        CheckListId = CId,
                        Description = LineCheckList.Description,
                        IsActive = LineCheckList.IsActive,
                        Code = code,
                        Remark = LineCheckList.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                       
                    };

                    db.LineCheckLists.Add(entity);
                    db.SaveChanges();
                    LineCheckList.LineCheckListId = entity.LineCheckListId;
 
            }

            return Json(new[] { LineCheckList }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineCheckList_Update([DataSourceRequest] DataSourceRequest request, LineCheckList LineCheckList)
        {
            
                if (LineCheckList != null)
                {
                    
                    LineCheckList.LastModified = DateTime.Now;
                    LineCheckList.ModifiedBy = User.Identity.Name;

                    db.LineCheckLists.Attach(LineCheckList);
                    db.Entry(LineCheckList).State = EntityState.Modified;
                    db.SaveChanges();
 
            }
            return Json(new[] { LineCheckList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineCheckList_Destroy([DataSourceRequest] DataSourceRequest request, LineCheckList LineCheckList)
        {
            var LineCheckListObj = db.LineCheckLists.Where(x => x.LineCheckListId == LineCheckList.LineCheckListId).FirstOrDefault();
            if (LineCheckList != null)
            {

                db.LineCheckLists.Attach(LineCheckListObj);
                db.LineCheckLists.Remove(LineCheckListObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Line Check List number " + LineCheckList.LineCheckListId);

            }

            return Json(new[] { LineCheckList }.ToDataSourceResult(request));
        }



    }



}

