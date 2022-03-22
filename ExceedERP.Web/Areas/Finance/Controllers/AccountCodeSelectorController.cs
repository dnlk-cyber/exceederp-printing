using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Repository;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
  
    public class AccountCodeSelectorController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        

        public JsonResult GetSegment(int index,string setup="")
        {
            List<SelectorVm> list = new List<SelectorVm>();
            var segmentSequence = _db.GlSegmentSetups.Where(x => x.Enabled).ToList();
            if (segmentSequence.Any())
            {
                var segment = segmentSequence.FirstOrDefault(x => x.Sequence == index);

                if (segment != null)
                {
                    if (segment.SegmentType== DynamicSegmentTypes.NaturalAccount)
                    {
                        #region old
                        var accounts = _db.GLChartOfAccountAccounts
                            .Where(x => x.Enable)
                            .Where(x => x.Parent == false)
                            .OrderBy(x => x.Values)

                            .ToList();

                        foreach (var acc in accounts)
                        {
                            var listitem = new SelectorVm
                            {
                                Code = acc.Values,
                                Name = acc.Values + "_" + acc.Description
                            };
                            list.Add(listitem);
                        }
                        #endregion
                        #region new
                        //var naturalAccounts = (List<SelectorVm>)System.Runtime.Caching.MemoryCache.Default["Finance_GL_CoA_Natural"];
                        //if (naturalAccounts == null)
                        //{
                        //    naturalAccounts = _db.GLChartOfAccountAccounts
                        //    .Where(x => x.Enable)
                        //    .Where(x => x.Parent == false)
                        //    .OrderBy(x => x.Values)


                        //         .Select(x => new SelectorVm
                        //         {
                        //             Code = x.Values,
                        //             Name = x.Values + "_" +x.Description
                        //         })
                        //         .ToList();
                        //    System.Runtime.Caching.MemoryCache.Default["Finance_GL_CoA_Natural"] = naturalAccounts;
                        //}
                        //foreach (var acc in naturalAccounts)
                        //{
                        //    var listitem = new SelectorVm
                        //    {
                        //        Code = acc.Code,
                        //        Name = acc.Code + "_" + acc.Name
                        //    };
                        //    list.Add(listitem);
                        //}

                        #endregion

                        #region if for setup picker add zzz

                        if (setup.ToUpper() == "TRUE")
                        {
                            string zzz = "";
                            int codeLength = list.FirstOrDefault().Code.Length;
                            var newZzz = zzz.PadLeft(codeLength, 'Z');
                            var listitem = new SelectorVm
                            {
                                Code =newZzz,
                                Name = newZzz
                            };
                            list.Add(listitem);
                        }

                        #endregion

                        return Json(list.Select(c => new
                        {
                            value = c.Code, 
                            desc = c.Name
                        }), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        #region old
                        var segments = _db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == segment.GLSegmentSetupId)
                            .Where(x => x.Enable).Where(x => x.Parent == false)
                            .OrderBy(x => x.Values)
                            .ToList();

                        if (segment.SegmentType == DynamicSegmentTypes.Location)
                        {
                            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

                            List<string> locationCodes = _db.Branch.Where(x => mybraches.Contains(x.BranchId.ToString())).Select(x => x.Code).ToList();

                            segments = segments.Where(x => locationCodes.Contains(x.Values)).ToList();
                        }

                        if (segment.SegmentType == DynamicSegmentTypes.BusinessUnit)
                        {
                            var userid = UserInformationHelper.GetUserId(User.Identity.Name);
                            List<string> bizmap = _db.UserBusinessUnitMaps
                                .Where(x => x.UserId == userid)
                                .Select(x => x.BusinessUnitId).ToList();

                            List<string> bizunits = _db.BusinessUnits.Where(x => bizmap.Contains(x.BusinessUnitId.ToString())).Select(x => x.Accountcode).ToList();

                            segments = segments.Where(x => bizunits.Contains(x.Values)).ToList();
                        }

                        foreach (var acc in segments)
                        {
                            var listitem = new SelectorVm
                            {
                                Code = acc.Values,
                                Name = acc.Values + "_" + acc.Description
                            };
                            list.Add(listitem);
                        }
                        #endregion

                        //#region new 


                        //var segments = (List<COASegmentSelectorVm>)System.Runtime.Caching.MemoryCache.Default["Finance_GL_CoA_Segments"];
                        //if (segments == null)
                        //{
                        //    var allSegments = _db.GlChartOfAccountSegments
                        //        .Where(x => x.GLSegmentSetupId == segment.GLSegmentSetupId)
                        //        .Where(x => x.Enable).Where(x => x.Parent == false)
                        //        .OrderBy(x => x.Values)                            
                        //         .Select(x => new COASegmentSelectorVm
                        //         {
                        //             Code = x.Values,
                        //             Name = x.Values + "_" + x.Description,
                        //             GLSegmentSetupId=x.GLSegmentSetupId
                        //         })
                        //         .ToList();
                        //    System.Runtime.Caching.MemoryCache.Default["Finance_GL_CoA_Segments"] = allSegments; //cache all segments but use the sepecific ones below

                        //    segments = allSegments.Where(x => x.GLSegmentSetupId == segment.GLSegmentSetupId).ToList();
                        //}
                        //else
                        //{
                        //    segments = segments.Where(x => x.GLSegmentSetupId == segment.GLSegmentSetupId).ToList();
                        //}

                        //if (segment.SegmentType == DynamicSegmentTypes.BusinessUnit)
                        //{
                        //    var userid = UserInformationHelper.GetUserId(User.Identity.Name);
                        //    List<string> bizmap = _db.UserBusinessUnitMaps
                        //        .Where(x => x.UserId == userid)
                        //        .Select(x => x.BusinessUnitId).ToList();

                        //    List<string> bizunits = _db.BusinessUnits.Where(x => bizmap.Contains(x.BusinessUnitId.ToString())).Select(x => x.Accountcode).ToList();

                        //    segments = segments.Where(x => bizunits.Contains(x.Code)).ToList();
                        //}

                        //foreach (var acc in segments)
                        //{
                        //    var listitem = new SelectorVm
                        //    {
                        //        Code = acc.Code,
                        //        Name = acc.Name
                        //    };
                        //    list.Add(listitem);
                        //}
                        //#endregion

                        #region if for setup picker add zzz

                        if (setup.ToUpper() == "TRUE")
                        {
                            string zzz = "";
                            int codeLength = list.FirstOrDefault().Code.Length;
                            var newZzz = zzz.PadLeft(codeLength, 'Z');
                            var listitem = new SelectorVm
                            {
                                Code = newZzz,
                                Name = newZzz
                            };
                            list.Add(listitem);
                        }

                        #endregion

                        return Json(list.Select(c => new
                        {
                            value = c.Code,
                            desc = c.Name
                        }), JsonRequestBehavior.AllowGet);

                    }

                }
               
            }

            return null;

        }

        //public JsonResult GetSegment_old(int index)
        //{
        //    var segmentSequence = _db.GlSegmentSetups.Where(x => x.Enabled).ToList();
        //    if (segmentSequence.Any())
        //    {
        //        var segment = segmentSequence.FirstOrDefault(x => x.Sequence == index);

        //        if (segment != null)
        //        {
        //            if (segment.SegmentType == DynamicSegmentTypes.NaturalAccount)
        //            {
        //                var accounts = _db.GLChartOfAccountAccounts
        //                    .Where(x => x.Enable)
        //                    .Where(x => x.Parent == false)
        //                    .OrderBy(x => x.Values)
        //                    .ToList();

        //                return Json(accounts.Select(c => new
        //                    {
        //                        value = c.Values,
        //                        desc = c.Values + "_" + c.Description
        //                    }),
        //                    JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                var segments = _db.GlChartOfAccountSegments
        //                    .Where(x => x.GLSegmentSetupId == segment.GLSegmentSetupId)
        //                    .Where(x => x.Enable).Where(x => x.Parent == false)
        //                    .OrderBy(x => x.Values)
        //                    .ToList();

        //                return Json(segments.Select(c => new
        //                    {
        //                        value = c.Values,
        //                        desc = c.Values + "_" + c.Description
        //                    }),
        //                    JsonRequestBehavior.AllowGet);

        //            }

        //        }

        //    }

        //    return null;

        //}


        public ActionResult GlAccountSelector(string s,string q)
        {
            ViewBag.PreviousAccount = q;
            ViewBag.Setup = s;
            ViewBag.ErrorMsg = "";
            var numberOfSegment = _db.GlRuleValues.FirstOrDefault();
            if(numberOfSegment!=null)
            {
                var segmentNum=numberOfSegment.NumberOfSegment;
                ViewBag.SegNum = segmentNum;
                var segmentSequence = _db.GlSegmentSetups.Where(x => x.Enabled).ToList();
                if(segmentSequence.Any())
                {
                    var segmentCount = segmentSequence.Count;
                    if (segmentNum != segmentCount)
                    {
                        ViewBag.ErrorMsg ="The number of segments configured and the list of enabled segments is not equal.";
                        return View();
                    }

                    ViewBag.segment1 = segmentSequence.FirstOrDefault(x => x.Sequence == 1)?.Name;
                    ViewBag.segment2 = segmentSequence.FirstOrDefault(x => x.Sequence == 2)?.Name;
                    ViewBag.segment3 = segmentSequence.FirstOrDefault(x => x.Sequence == 3)?.Name;
                    ViewBag.segment4 = segmentSequence.FirstOrDefault(x => x.Sequence == 4)?.Name;
                    ViewBag.segment5 = segmentSequence.FirstOrDefault(x => x.Sequence == 5)?.Name;

                }
                else
                {
                    ViewBag.ErrorMsg = "Segment Sequence is not defined.";
                }
               
                
            }
            else
            {
                ViewBag.ErrorMsg = "Number of Segment is not defined.";
            }
            
           
            return View();
        }

        public ActionResult ArAccountSelector()
        {
            return View();
        }

        public ActionResult ApAccountSelector()
        {
            return View();
        }

        public ActionResult WithholdingAccountSelector()
        {
            return View();
        }

        public ActionResult ApCustomerAccount()
        {
            return View();
        }

        public ActionResult GlAccountFromSelector()
        {
            return View();
        }

        public ActionResult GlAccountToSelector()
        {
            return View();
        }
    }
}