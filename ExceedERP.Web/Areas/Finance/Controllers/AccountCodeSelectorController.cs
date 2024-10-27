using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Finance.Controllers
{

    public class AccountCodeSelectorController : Controller
    {
        private readonly ExceedDbContext _db ;
        public AccountCodeSelectorController()
        {
            _db = new ExceedDbContext();
        }
        public JsonResult GetSegment(int index, string setup = "")
        {
            List<SelectorVm> list = new List<SelectorVm>();

            // Use cached GlSegmentSetups
            var segmentSequence = MemoryCache.Default[CacheHelper.GlSegmentSetups] as List<GlSegmentSetupCache>;
            if (segmentSequence == null)
            {
                segmentSequence = _db.GlSegmentSetups
                                    .Where(x => x.Enabled)
                                    .Select(x => new GlSegmentSetupCache
                                    {
                                        Sequence = x.Sequence,
                                        SegmentType = x.SegmentType,
                                        GLSegmentSetupId = x.GLSegmentSetupId,
                                        Name = x.Name,
                                    })
                                    .ToList();
                MemoryCache.Default.Add(CacheHelper.GlSegmentSetups, segmentSequence, ObjectCache.InfiniteAbsoluteExpiration);
            }

            if (segmentSequence.Any())
            {
                var segment = segmentSequence.FirstOrDefault(x => x.Sequence == index);

                if (segment != null)
                {
                    if (segment.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        // Use cached GLChartOfAccountAccounts
                        var accounts = MemoryCache.Default[CacheHelper.GLChartOfAccountAccounts] as List<GLChartOfAccountAccountCache>;
                        if (accounts == null)
                        {
                            accounts = _db.GLChartOfAccountAccounts
                                .Where(x => x.Enable && x.Parent == false)
                                .OrderBy(x => x.Values)
                                .Select(x => new GLChartOfAccountAccountCache
                                {
                                    Values = x.Values,
                                    Description = x.Description
                                })
                                .ToList();
                            MemoryCache.Default.Add(CacheHelper.GLChartOfAccountAccounts, accounts, ObjectCache.InfiniteAbsoluteExpiration);
                        }

                        foreach (var acc in accounts)
                        {
                            var listitem = new SelectorVm
                            {
                                Code = acc.Values,
                                Name = acc.Values + "_" + acc.Description
                            };
                            list.Add(listitem);
                        }

                        // If for setup picker, add zzz
                        if (setup.ToUpper() == "TRUE")
                        {
                            string zzz = "";
                            int codeLength = list.FirstOrDefault()?.Code.Length ?? 0;
                            var newZzz = zzz.PadLeft(codeLength, 'Z');
                            var listitem = new SelectorVm
                            {
                                Code = newZzz,
                                Name = newZzz
                            };
                            list.Add(listitem);
                        }

                        return Json(list.Select(c => new { value = c.Code, desc = c.Name }), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // Use cached GlChartOfAccountSegments
                        var segments = MemoryCache.Default[CacheHelper.GlChartOfAccountSegments] as List<GlChartOfAccountSegmentCache>;
                        if (segments == null)
                        {
                            segments = _db.GlChartOfAccountSegments
                                .Where(x => x.Enable && x.Parent == false)
                                .OrderBy(x => x.Values)
                                .Select(x => new GlChartOfAccountSegmentCache
                                {
                                    GLSegmentSetupId = x.GLSegmentSetupId,
                                    Values = x.Values,
                                    Description = x.Description
                                })
                                .ToList();
                            MemoryCache.Default.Add(CacheHelper.GlChartOfAccountSegments, segments, ObjectCache.InfiniteAbsoluteExpiration);
                        }

                        var filteredSegments = segments.Where(x => x.GLSegmentSetupId == segment.GLSegmentSetupId).ToList();

                        // Apply location or business unit filters if applicable
                        if (segment.SegmentType == DynamicSegmentTypes.Location)
                        {
                            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
                            List<string> locationCodes = _db.Branch.Where(x => myBranches.Contains(x.BranchId))
                                                                   .Select(x => x.LocationAccount)
                                                                   .ToList();
                            filteredSegments = filteredSegments.Where(x => locationCodes.Contains(x.Values)).ToList();
                        }

                        if (segment.SegmentType == DynamicSegmentTypes.BusinessUnit)
                        {
                            var userId = UserInformationHelper.GetUserId(User.Identity.Name);
                            List<int> bizMap = _db.UserBusinessUnitMaps
                                                  .Where(x => x.UserId == userId)
                                                  .Select(x => x.BusinessUnitId)
                                                  .ToList();

                            List<string> bizUnits = _db.BusinessUnits
                                                      .Where(x => bizMap.Contains(x.BusinessUnitId))
                                                      .Select(x => x.Accountcode)
                                                      .ToList();

                            filteredSegments = filteredSegments.Where(x => bizUnits.Contains(x.Values)).ToList();
                        }

                        foreach (var acc in filteredSegments)
                        {
                            var listitem = new SelectorVm
                            {
                                Code = acc.Values,
                                Name = acc.Values + "_" + acc.Description
                            };
                            list.Add(listitem);
                        }

                        // If for setup picker, add zzz
                        if (setup.ToUpper() == "TRUE")
                        {
                            string zzz = "";
                            int codeLength = list.FirstOrDefault()?.Code.Length ?? 0;
                            var newZzz = zzz.PadLeft(codeLength, 'Z');
                            var listitem = new SelectorVm
                            {
                                Code = newZzz,
                                Name = newZzz
                            };
                            list.Add(listitem);
                        }

                        return Json(list.Select(c => new { value = c.Code, desc = c.Name }), JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return null;
        }

        public JsonResult GetSegment_old(int index,string setup="")
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
                            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

                            List<string> locationCodes = _db.Branch.Where(x => mybraches.Contains(x.BranchId)).Select(x => x.LocationAccount).ToList();

                            segments = segments.Where(x => locationCodes.Contains(x.Values)).ToList();
                        }

                        if (segment.SegmentType == DynamicSegmentTypes.BusinessUnit)
                        {
                            var userid = UserInformationHelper.GetUserId(User.Identity.Name);
                            List<int> bizmap = _db.UserBusinessUnitMaps
                                .Where(x => x.UserId == userid)
                                .Select(x => x.BusinessUnitId).ToList();

                            List<string> bizunits = _db.BusinessUnits.Where(x => bizmap.Contains(x.BusinessUnitId)).Select(x => x.Accountcode).ToList();

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



        public ActionResult GlAccountSelector(string s, string q)
        {
            ViewBag.PreviousAccount = q;
            ViewBag.Setup = s;
            ViewBag.ErrorMsg = "";

            // Fetch the number of segments
            var numberOfSegment = _db.GlRuleValues.FirstOrDefault();
            if (numberOfSegment != null)
            {
                var segmentNum = numberOfSegment.NumberOfSegment;
                ViewBag.SegNum = segmentNum;

                // Use cached GlSegmentSetups if available
                var segmentSequence = MemoryCache.Default[CacheHelper.GlSegmentSetups] as List<GlSegmentSetupCache>;
                if (segmentSequence == null)
                {
                    segmentSequence = _db.GlSegmentSetups
                                        .Where(x => x.Enabled)
                                        .Select(x => new GlSegmentSetupCache
                                        {
                                            Sequence = x.Sequence,
                                            SegmentType = x.SegmentType,
                                            GLSegmentSetupId = x.GLSegmentSetupId,
                                            Name = x.Name,
                                        })
                                        .ToList();
                    MemoryCache.Default.Add(CacheHelper.GlSegmentSetups, segmentSequence, ObjectCache.InfiniteAbsoluteExpiration);
                }

                if (segmentSequence.Any())
                {
                    var segmentCount = segmentSequence.Count;
                    if (segmentNum != segmentCount)
                    {
                        ViewBag.ErrorMsg = "The number of segments configured and the list of enabled segments is not equal.";
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

        public ActionResult GlAccountSelector_old(string s,string q)
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