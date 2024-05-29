using System.Globalization;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for RetirementNote.
    /// </summary>
    public partial class IncomOutcommingLetter : Telerik.Reporting.Report
    {
        public IncomOutcommingLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
          
            this.DataSource = Organization;
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.DateHead.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Type.Value = Resources.ResourceManager.GetString("Rpt_Type", cultures);
            this.EmpInstName.Value = Resources.ResourceManager.GetString("Rpt_EmpInstName", cultures);
            this.EmpInst.Value = Resources.ResourceManager.GetString("Rpt_EmpInst", cultures);
            this.RefNo.Value = Resources.ResourceManager.GetString("Rpt_RefNo", cultures);
            this.Shelve.Value = Resources.ResourceManager.GetString("Rpt_Shelve", cultures);
            this.Subject.Value = Resources.ResourceManager.GetString("Rpt_Subject", cultures);
            this.Row.Value = Resources.ResourceManager.GetString("Rpt_Row", cultures);
            this.Folder.Value = Resources.ResourceManager.GetString("Rpt_Folder", cultures);
            this.NoOfAtt.Value = Resources.ResourceManager.GetString("Rpt_NoOfAtt", cultures);
            this.LetterReport.Value = Resources.ResourceManager.GetString("Rpt_LetterReport", cultures);
        }
    }
}