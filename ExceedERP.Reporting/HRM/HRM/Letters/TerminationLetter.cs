namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TerminationLetter.
    /// </summary>
    public partial class TerminationLetter : Telerik.Reporting.Report
    {
        public TerminationLetter()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

           // reportFooterSection1.Visible = false;
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = TerminationLetterObjectDataSource;
            if(this.DataSource !=null)
            {

                //reportFooterSection1.Visible = true;
            }
        }
    }
}