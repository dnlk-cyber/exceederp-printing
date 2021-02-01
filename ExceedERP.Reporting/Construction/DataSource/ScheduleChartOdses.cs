using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Construction.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    //class ScheduleChartOdses
    //{
    //}

    [DataObject]
    public class GanttChartOds : List<GanttChartViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GanttChartViewModel> GetScheduleTasks(string projectId, string revisionId)
        {
            List<GanttChartViewModel> list = new List<GanttChartViewModel>();

            if (projectId == null)
            {
                return list;
            }
            int id = Convert.ToInt32(projectId);
            int revId = Convert.ToInt32(revisionId);
            var project = _db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();
            var currentVersion = _db.ScheduleRevisions.Where(v => v.ScheduleRevisionId == revId).FirstOrDefault();

            if (project != null && currentVersion != null)
            {
                //var tasks = _db.Tasks.Where(t => t.ProjectWorkBreakDown.ProjectInformationId == project.ProjectInformationId && t.Version == currentVersion.Version).ToList();
                //if (tasks.Any())
                //{
                //    foreach (ExceedERP.Core.Domain.Construction.Task task in tasks)
                //    {
                //        var record = new GanttChartViewModel
                //        {
                //            Title = task.Title,
                //            Start = task.Start,
                //            End = task.End
                //        };
                //        list.Add(record);
                //    }
                //}
            }

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ScheduleVerionViewModel> GetProjectScheduleVersions(string projectId)
        {

            List<ScheduleVerionViewModel> list = new List<ScheduleVerionViewModel>();

            if (projectId == null)
            {
                return list;
            }
            int id = Convert.ToInt32(projectId);
            var project = _db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if(project != null)
            {
                var availableVerions = _db.ScheduleRevisions.Where(sv => sv.ProjectInformationId == id).ToList();
                if (availableVerions.Any())
                {
                    foreach(ScheduleRevision revision in availableVerions)
                    {
                        var record = new ScheduleVerionViewModel {
                             Id = revision.ScheduleRevisionId,
                             Version = "Ver. " + revision.Version
                        };
                        list.Add(record);
                    }
                }
            }
            return list;
        }
    }
}
