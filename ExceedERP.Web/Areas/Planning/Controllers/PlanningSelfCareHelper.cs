using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    internal static class PlanningSelfCareHelper
    {
        public static bool CanViewAllWorkUnits(IPrincipal user)
        {
            return user != null &&
                   (user.IsInRole(PlanningRoles.Planning_Admin) ||
                    user.IsInRole(PlanningRoles.Planning_User));
        }

        public static int GetCurrentDepartmentId(string userName)
        {
            return UserInformationHelper.GetEmployeeDepartmentId(userName);
        }

        public static IQueryable<GlobalIndicator> GetDepartmentGlobalIndicators(ExceedDbContext db, int departmentId, int? planId = null)
        {
            var globalIndicators = db.GlobalIndicators.Where(x => x.OrgStructureId == 0 || x.OrgStructureId == departmentId);

            if (planId.HasValue)
            {
                var planDepartmentIds = db.BSCPlans
                    .Where(x => x.PlanId == planId.Value)
                    .Select(x => x.OrgStructureId);

                globalIndicators = globalIndicators.Where(x => x.OrgStructureId == 0 || x.OrgStructureId == departmentId || planDepartmentIds.Contains(x.OrgStructureId));
            }

            return globalIndicators;
        }

        public static List<object> GetDepartmentGlobalIndicatorList(ExceedDbContext db, string userName)
        {
            var departmentId = GetCurrentDepartmentId(userName);
            return GetDepartmentGlobalIndicators(db, departmentId)
                .Select(x => new
                {
                    x.GlobalIndicatorId,
                    x.Indicator
                })
                .AsEnumerable()
                .Select(x => (object)x)
                .ToList();
        }

        public static List<object> GetDepartmentObjectiveList(ExceedDbContext db, string userName)
        {
            var departmentId = GetCurrentDepartmentId(userName);
            return db.Objectives
                .Where(x => x.OrgStructureId == departmentId)
                .Select(x => new
                {
                    x.ObjectiveId,
                    x.Name,
                    x.Description
                })
                .AsEnumerable()
                .Select(x => (object)x)
                .ToList();
        }

        public static List<object> GetDepartmentIndicatorList(ExceedDbContext db, string userName, bool activeOnly = false)
        {
            var departmentId = GetCurrentDepartmentId(userName);
            var indicators = db.Indicators.Where(x => x.GetObjective.OrgStructureId == departmentId);

            if (activeOnly)
            {
                indicators = indicators.Where(x => x.IsActive);
            }

            return indicators
                .Select(x => new
                {
                    x.IndicatorId,
                    x.Name,
                    x.Description,
                    x.KPIType
                })
                .AsEnumerable()
                .Select(x => (object)x)
                .ToList();
        }
    }
}