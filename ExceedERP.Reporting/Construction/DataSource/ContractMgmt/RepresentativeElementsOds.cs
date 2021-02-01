using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class RepresentativeElementsOds : List<RepresentativeElementProjectViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RepresentativeElementProjectViewModel> GetRepresentativeElements(string projectId)
        {
            List<RepresentativeElementProjectViewModel> list = new List<RepresentativeElementProjectViewModel>();

            var id = Convert.ToInt32(projectId);

            var projects = _db.ProjectInformation.ToList();
            if (projectId != null)
            {
                projects = projects.Where(p => p.ProjectInformationId == id).ToList();
            }
            if (projects.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var record = new RepresentativeElementProjectViewModel
                    {
                        Name = project.Name,
                        Id = project.ProjectInformationId
                    };
                    var elements = _db.ProjectRepresentativeElements.Where(p => p.ProjectInformationId == project.ProjectInformationId).ToList();
                    foreach(ProjectRepresentativeElement element in elements)
                    {
                        var elementRecord = new RepresentativeElementViewModel
                        {
                            IndexCode = element.IndexCode,
                            Description = element.Description,
                            RepresentedItems = element.RepresentedItems,
                            Source = element.Source,
                            BasicPrice = element.BasicPrice
                        };
                        record.RepresentativeElementsList.Add(elementRecord);
                    }
                    
                    if ( record.RepresentativeElementsList.Any())
                    {
                        list.Add(record);
                    }
                }
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = _db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = _db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }
    }

}

