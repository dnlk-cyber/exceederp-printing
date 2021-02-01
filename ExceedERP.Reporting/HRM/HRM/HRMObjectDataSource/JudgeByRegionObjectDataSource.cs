using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using Structure = ExceedERP.Reporting.HRM.HRM.HRMViewModel.Structure;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeByRegionObjectDataSource : List<JudgeByRegion>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JudgeByRegion> GetJudgeByRegion()
        {
            List<JudgeByRegion> InfoList = new List<JudgeByRegion>();
            var lookups = db.Lookups;

            var regionslist= Enum.GetValues(typeof(Region)).OfType<Region>().ToList();
            foreach (var nation in regionslist)
            {
                InfoList.Add(GetByRegion(nation));
            }


           

            //#region GAMBELLA

            //regionList = regions.Where(x => x.Region == Region.GAMBELLA);
            //if (regionList.Any())
            //{
            //    Info = new JudgeByRegion();
            //    Info.RegionName = Region.GAMBELLA;
            //    Info.Nations = new List<Nation>();
            //    foreach (var n in regionList)
            //    {
            //        var r = nationss.Where(x => x.LookupId == n.LookupId).FirstOrDefault();
            //        Nations = new Nation();
            //        Nations.Structure = new List<Structure>();
            //        Nations.NationName = r.Description;
            //        if (branch.Any())
            //        {
            //            foreach (var b in branch)
            //            {
            //                Structure = new Structure();

            //                Structure.Name = b.Name;
            //                var empNation = allemployee.Where(x => x.Nation == n.LookupId.ToString());
            //                foreach (var e in empNation)
            //                {
            //                    Structure.Sum += employees
            //                        .Where(x => x.BranchId == b.BranchId && x.EmployeeId == e.EmployeeId).Count();
            //                    Info.GTotal += Structure.Sum;
            //                }

            //                Nations.Structure.Add(Structure);
            //            }
            //        }

            //        Info.Nations.Add(Nations);


            //    }

            //    InfoList.Add(Info);

            //}

            //#endregion


            //#region HARRARI

            //GetByRegion(Region.HARRARI);

            //#endregion

            //#region OROMO

            //GetByRegion(Region.OROMO);


            //#endregion

            //#region SOMALIA

            //GetByRegion(Region.SOMALIA);

            //#endregion

            //#region SOUTH_NATION_AND_NATIONALITY

            //GetByRegion(Region.SOUTH_NATION_AND_NATIONALITIES);

            //#endregion

            //#region TIGRAY

            //GetByRegion(Region.TIGRAY);

            //#endregion

            return InfoList;
        }

        public JudgeByRegion GetByRegion(Region regionNames)
        {
            JudgeByRegion info = new JudgeByRegion();
            Structure structure = new Structure();
            Nation Nations = new Nation();
            var allemployee = db.Person_Employee;
            var employees = db.Placements.ToList();
            var regions = db.RegionNation.ToList();
            var lookups = db.Lookups;
            var branch = db.Branch;
            var nationss = lookups.Where(x => x.LookupType == LookUpTypes.NATION);

            var regionList = regions.Where(x => x.Region == regionNames);
            if (regionList.Any())
            {
                info = new JudgeByRegion();
                info.RegionName = regionNames;
                info.Nations = new List<Nation>();
                foreach (var n in regionList)
                {
                    var r = nationss.FirstOrDefault(x => x.LookupId == n.LookupId);
                    Nations = new Nation();
                    Nations.Structure = new List<Structure>();
                    Nations.NationName = r.Description;
                    if (branch.Any())
                    {
                        foreach (var b in branch)
                        {
                            structure = new Structure();

                            structure.Name = b.Name;
                            var empNation = allemployee.Where(x => x.Nation == n.LookupId.ToString());
                            foreach (var e in empNation)
                            {
                                structure.Sum += employees.Count(x => x.BranchId == b.BranchId && x.EmployeeId == e.EmployeeId);
                                info.GTotal += structure.Sum;
                            }

                            Nations.Structure.Add(structure);
                        }
                    }

                    info.Nations.Add(Nations);


                }
            }

            return info;

        }
    
}
}