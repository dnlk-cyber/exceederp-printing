using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public partial class DiagramController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public virtual JsonResult ReadDiagramTasks([DataSourceRequest] DataSourceRequest request, int revisionId)
        {
            var db = new ExceedDbContext();

            var currentRevion = db.ScheduleRevisions.Find(revisionId);
            int projectId = 0;
            if (currentRevion != null)
            {
                var project = db.ProjectInformation.Find(currentRevion.ProjectInformationId);
                if (project != null)
                {
                    projectId = project.ProjectInformationId;
                }
            }


            var items = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == projectId && t.Version == currentRevion.Version);
            var rawList = items.Select(t => new
            {
                TaskID = t.ID,
                ProjectSpecificId = t.ProjectSpecificId,
                Title = t.Title,
                Start = t.Start,
                End = t.End,
                //ProjectWorkBreakDownId = t.ProjectWorkBreakDownId,
                WorkItem = t.WorkItemDuration
            });
            List<DiagramViewModel> dvm = new List<DiagramViewModel>();
            foreach (var itm in rawList)
            {
                var newDVM = new DiagramViewModel
                {
                    TaskID = itm.TaskID,
                    ProjectSpecificId = itm.ProjectSpecificId,
                    Title = itm.Title,
                   // ProjectWorkBreakDownId = itm.ProjectWorkBreakDownId,
                    ProjectInformationId = itm.WorkItem.ProjectInformationId,
                    Duration = BusinessDaysCalculator.Calculate(itm.Start, itm.End),
                    Start = FormatDates(itm.Start),
                    End = FormatDates(itm.End),
                    Successors = GetTaskSuccessors(itm.TaskID),
                    //Successors = GetLongestSuccessor(itm.TaskID),
                    Resources = GetTaskResources(itm.TaskID)
                };
                if (newDVM.Successors.Any())
                {

                    if (!HasAPredecessor(newDVM.TaskID) && IsProjectKickOffTask(newDVM.TaskID, revisionId))
                    {
                        newDVM.Successors = GetFilteredSuccessors(newDVM);
                        newDVM.Successors = GetCriticalPathTasks(newDVM);
                        dvm.Add(newDVM);
                    }
                }

            }
            return Json(dvm, JsonRequestBehavior.AllowGet);
        }
        public bool HasAPredecessor(int id)
        {
            var predecessors = db.Dependencies.Where(dp => dp.SuccessorID == id);
            return predecessors.Count() > 0;
        }
        private bool IsProjectKickOffTask(int taskId, int revisionId)
        {
            var currentTask = db.Tasks.Find(taskId);
            if (currentTask == null)
            {
            }
            return GetScheduleVersionStartAndEndDates(currentTask.WorkItemDuration.ProjectInformationId, revisionId)[0] == currentTask.Start;
        }
        private bool IsProjectEndTask(int taskId, int revisionId)
        {
            var currentTask = db.Tasks.Find(taskId);
            if (currentTask == null)
            {
            }
            return GetScheduleVersionStartAndEndDates(currentTask.WorkItemDuration.ProjectInformationId, revisionId)[1] == currentTask.End;
        }

        // calculate the path weight of all possible paths
        private DiagramViewModel DetermineLongestPath(DiagramViewModel model)
        {
            var filteredSuccessor = new DiagramViewModel();
            if (model.Successors.Any())
            {
                var allSuccessors = model.Successors;
                filteredSuccessor = allSuccessors.FirstOrDefault();
                foreach (var succeesor in allSuccessors)
                {
                    if (!succeesor.Successors.Any())
                    {
                        if (succeesor.PathWeight >= filteredSuccessor.PathWeight)
                        {
                            filteredSuccessor = succeesor;

                        }
                    }
                    else
                    {
                        var successorOfSuccessor = DetermineLongestPath(succeesor);
                        if (successorOfSuccessor != null)
                        {
                            var combinedWieght = succeesor.PathWeight + successorOfSuccessor.PathWeight;
                            if (combinedWieght > filteredSuccessor.PathWeight)
                            {
                                //succeesor.Duration = combinedWieght;
                                succeesor.PathWeight = combinedWieght;
                                filteredSuccessor = succeesor;
                            }
                        }
                    }
                }
            }
            return filteredSuccessor;
        }
        private List<DiagramViewModel> GetFilteredSuccessors(DiagramViewModel model)
        {
            var targetModel = DetermineLongestPath(model);
            var list = new List<DiagramViewModel>();
            if (targetModel != null)
            {
                list.Add(targetModel);
            }
            return list;
        }

        // refinine the paths by comparing their path weight, which is the sum of the successor
        // and its consecutive succssors 
        private DiagramViewModel DetermineCriticalPath(DiagramViewModel model)
        {
            var longestTask = new DiagramViewModel();
            if (model.Successors.Any())
            {
                var successors = model.Successors;
                longestTask = successors.FirstOrDefault();

                if (longestTask.Successors.Any())
                {
                    longestTask.Successors = GetCriticalPathTasks(longestTask);
                }

                if (successors.Count > 1)
                {
                    var innerLongest = successors.FirstOrDefault();
                    foreach (var sc in successors)
                    {
                        if (sc.PathWeight > innerLongest.PathWeight)
                        {
                            innerLongest = sc;
                        }
                    }
                    if (innerLongest.Successors.Any())
                    {
                        innerLongest.Successors = GetCriticalPathTasks(innerLongest);
                    }
                    longestTask = innerLongest;
                }

            }
            return longestTask;
        }

        // get the tasks that are on the critical path
        private List<DiagramViewModel> GetCriticalPathTasks(DiagramViewModel model)
        {
            var targetModel = DetermineCriticalPath(model);
            var list = new List<DiagramViewModel>();
            if (targetModel != null)
            {
                list.Add(targetModel);
            }
            return list;
        }
        private string FormatDates(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        // finding all successor of a given  task
        private List<DiagramViewModel> GetTaskSuccessors(int taskId)
        {
            List<DiagramViewModel> successers = new List<DiagramViewModel>();
            var allSuccessors = from d in db.Dependencies
                                join t in db.Tasks
                                on d.SuccessorID equals t.ID
                                where d.PredecessorID == taskId
                                select new
                                {
                                    ID = t.ID,
                                    ProjectSpecificId = t.ProjectSpecificId,
                                    Title = t.Title,
                                    Start = t.Start,
                                    End = t.End,
                                    ProjectInformationId = t.WorkItemDuration.ProjectInformationId,
                                    //ProjectWorkBreakDownId = t.ProjectWorkBreakDownId
                                };

            foreach (var sc in allSuccessors)
            {
                var sucToAdd = new DiagramViewModel
                {
                    TaskID = sc.ID,
                    ProjectSpecificId = sc.ProjectSpecificId,
                    Title = sc.Title,
                    //ProjectWorkBreakDownId = sc.ProjectWorkBreakDownId,
                    ProjectInformationId = sc.ProjectInformationId,
                    Start = FormatDates(sc.Start),
                    End = FormatDates(sc.End),
                    Duration = BusinessDaysCalculator.Calculate(sc.Start, sc.End),
                    Successors = GetTaskSuccessors(sc.ID),
                    Resources = GetTaskResources(sc.ID)
                };
                sucToAdd.PathWeight = sucToAdd.Duration;
                successers.Add(sucToAdd);
            }

            return successers;
        }

        //private DateTime[] GetProjectStartAndEndDates(int projectId)
        //{
        //    var largestDate = DateTime.Now;
        //    var smallestDate = DateTime.Now;
        //    var currentProj = db.ProjectInformation.Find(projectId);
        //    if (currentProj != null)
        //    {
        //        var projectTasks = db.Tasks.Where(task => task.ProjectWorkBreakDown.Project.ProjectInformationId == projectId).AsQueryable();
        //        if (projectTasks.Count() > 0)
        //        {
        //            var tsk = projectTasks.FirstOrDefault();
        //            if (tsk != null)
        //            {
        //                smallestDate = tsk.Start;
        //                largestDate = tsk.End;

        //                foreach (var t in projectTasks)
        //                {
        //                    if (t.Start < smallestDate)
        //                    {
        //                        smallestDate = t.Start;
        //                    }
        //                }
        //                foreach (var t in projectTasks)
        //                {
        //                    if (t.End > largestDate)
        //                    {
        //                        largestDate = t.End;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return new DateTime[] { smallestDate, largestDate };
        //}
        private DateTime[] GetScheduleVersionStartAndEndDates(int projectId, int revisionId)
        {
            var largestDate = DateTime.Now;
            var smallestDate = DateTime.Now;
            var currentProj = db.ProjectInformation.Find(projectId);
            var currentScheduleVersion = db.ScheduleRevisions.Find(revisionId);
            if (currentProj != null && currentScheduleVersion != null)
            {
                var projectTasks = db.Tasks.Where(task => task.WorkItemDuration.ProjectInformationId == projectId && task.Version == currentScheduleVersion.Version).AsQueryable();
                if (projectTasks.Count() > 0)
                {
                    var tsk = projectTasks.FirstOrDefault();
                    if (tsk != null)
                    {
                        smallestDate = tsk.Start;
                        largestDate = tsk.End;

                        foreach (var t in projectTasks)
                        {
                            if (t.Start < smallestDate)
                            {
                                smallestDate = t.Start;
                            }
                        }
                        foreach (var t in projectTasks)
                        {
                            if (t.End > largestDate)
                            {
                                largestDate = t.End;
                            }
                        }
                    }
                }
            }
            return new DateTime[] { smallestDate, largestDate };
        }
        private string GetTaskResources(int taskId)
        {
            var concatnatedResNames = "";
            var res = (from r in db.Resources
                       join a in db.ResourceAssignments
                       on r.ID equals a.ResourceID
                       where a.TaskID == taskId
                       select new
                       { r.Name, }).ToList();
            var firstRes = res.FirstOrDefault();
            if (firstRes != null)
            {
                if (res.Count > 1)
                {
                    //concatnatedResNames = concatnatedResNames + firstRes.Name + " , ";
                    concatnatedResNames = concatnatedResNames + firstRes.Name;
                    var resArray = res.ToArray();
                    //for (int i = 1; i < 3 ; i++)
                    ////for (int i = 1; i < res.Count() - 1; i++)
                    //{
                    //    concatnatedResNames = concatnatedResNames + resArray[i].Name + " , ";
                    //}
                    //concatnatedResNames = concatnatedResNames + resArray[res.Count() - 1].Name;
                    concatnatedResNames = concatnatedResNames + " ...";
                }
                else if (res.Count == 1)
                {
                    concatnatedResNames = concatnatedResNames + firstRes.Name;
                }
                else
                {
                    concatnatedResNames = " ";
                }
            }
            return concatnatedResNames;

        }
    }
}