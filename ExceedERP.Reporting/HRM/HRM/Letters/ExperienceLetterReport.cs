using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    public partial class ExperienceLetterReport : Telerik.Reporting.Report
    {
        public ExperienceLetterReport()
        {
            // 1. Safe Culture Handling
            var activeCulture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture;
            var activeUiCulture = CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = activeCulture;
            Thread.CurrentThread.CurrentUICulture = activeUiCulture;

            // 2. Initialize Visual Components
            InitializeComponent();

            // Keep the greeting but leave signer details blank for manual completion.
            this.htmlSigner.Value = "ከሠላምታ ጋር / With Regards,";
            this.htmlContentEn.Value = this.htmlContentEn.Value + " + \"<br /><br /><b>Internal Work Experience:</b><br />\" + Fields.InternalExperienceEnglish";
            this.htmlContentAm.Value = this.htmlContentAm.Value + " + \"<br /><br /><b>የውስጥ የሥራ ልምድ፡</b><br />\" + Fields.InternalExperienceAmharic";

            // 3. Header Image Setup
            Image headerImage = null;
            bool usingUploadedHeader = false;
            var httpContext = System.Web.HttpContext.Current;
            byte[] companyLogoBytes = null;
            string uploadedHeaderFileName = null;

            try
            {
                using (var db = new ExceedDbContext())
                {
                    //uploadedHeaderFileName = db.TemplateHeaderImages
                    //    .Where(x => x.IsNeened && x.HeaderImageFileName != null && x.HeaderImageFileName != "")
                    //    .Select(x => x.HeaderImageFileName)
                    //    .FirstOrDefault();

                    var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
                    if (organization != null)
                    {
                        var organizationId = organization.MainCompanyId.ToString();
                        companyLogoBytes = db.Attachments
                            .Where(x => x.Reference == organizationId && x.ReferenceType == AttachmentType.LOGO)
                            .Select(x => x.BinaryFile)
                            .FirstOrDefault();
                    }
                }
            }
            catch
            {
                // Continue to the available file-based fallbacks if the database is unavailable.
            }

            if (httpContext != null && !string.IsNullOrWhiteSpace(uploadedHeaderFileName))
            {
                try
                {
                    var safeFileName = Path.GetFileName(uploadedHeaderFileName);
                    var uploadedHeaderPath = httpContext.Server.MapPath("~/Content/Image/WaterMark/" + safeFileName);
                    if (File.Exists(uploadedHeaderPath))
                    {
                        using (var sourceImage = Image.FromFile(uploadedHeaderPath))
                        {
                            headerImage = new Bitmap(sourceImage);
                        }
                        usingUploadedHeader = true;
                    }
                }
                catch
                {
                    // An invalid/missing upload should not prevent the logo fallback.
                }
            }

            if (headerImage == null && companyLogoBytes != null && companyLogoBytes.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream(companyLogoBytes))
                    using (var sourceImage = Image.FromStream(stream))
                    {
                        headerImage = new Bitmap(sourceImage);
                    }
                }
                catch
                {
                    // Continue to the bundled logo if the stored logo cannot be decoded.
                }
            }

            if (headerImage == null && httpContext != null)
            {
                try
                {
                    var fallbackLogoPath = httpContext.Server.MapPath("~/Content/Image/WaterMark/mcg-logo-header.png");
                    if (File.Exists(fallbackLogoPath))
                    {
                        using (var sourceImage = Image.FromFile(fallbackLogoPath))
                        {
                            headerImage = new Bitmap(sourceImage);
                        }
                    }
                }
                catch
                {
                    // Fallback gracefully if the bundled logo is unavailable.
                }
            }

            if (headerImage != null)
            {
                this.imgHeaderLogo.Value = headerImage;
                if (usingUploadedHeader)
                {
                    this.imgHeaderLogo.Location = new Telerik.Reporting.Drawing.PointU(
                        Telerik.Reporting.Drawing.Unit.Inch(0D),
                        Telerik.Reporting.Drawing.Unit.Inch(0.0799999982D));
                    this.imgHeaderLogo.Size = new Telerik.Reporting.Drawing.SizeU(
                        Telerik.Reporting.Drawing.Unit.Inch(7.0999999046D),
                        Telerik.Reporting.Drawing.Unit.Inch(0.9D));
                    this.txtHeaderCompName.Visible = false;
                    this.txtHeaderCompSlogan.Visible = false;
                }
            }

            // 4. Watermark Setup
            try
            {
                var watermarkImage = WaterMarkHelper.GetImageForWaterMark();
                if (watermarkImage != null)
                {
                    var pictureWatermark1 = new Telerik.Reporting.Drawing.PictureWatermark
                    {
                        Image = watermarkImage,
                        Position = Telerik.Reporting.Drawing.WatermarkPosition.Behind,
                        PrintOnFirstPage = true,
                        PrintOnLastPage = true,
                        Sizing = Telerik.Reporting.Drawing.WatermarkSizeMode.ScaleProportional,
                        Opacity = 0.5D
                    };

                    this.PageSettings.Watermarks.Add(pictureWatermark1);
                }
            }
            catch
            {
                // Fallback gracefully if watermark image path is unavailable
            }
        }
    }
}