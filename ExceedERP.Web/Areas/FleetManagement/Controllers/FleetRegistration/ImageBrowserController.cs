using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
 using ExceedERP.Core.Domain.Common;
using System.IO;
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class ImageBrowserController : EditorImageBrowserController
    {
        private const string contentFolderRoot = "~/Content/Images/FLEETPHOTO/";
        private const string prettyName = "Fleet/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/Images/FLEETPHOTO/" };

        public ImageBrowserController()
        { }

        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath
        {
            get
            {
                return CreateUserFolder();
            }
        }
        
   
        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot);

            var path = Server.MapPath(virtualPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    CopyFolder(Server.MapPath(sourceFolder), path);
                }
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }
    }
}