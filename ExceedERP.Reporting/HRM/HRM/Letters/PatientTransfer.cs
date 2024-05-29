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
    public partial class PatientTransfer : Telerik.Reporting.Report
    {
        public PatientTransfer()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

          
            //
            // TODO: Add any constructor code after InitializeComponent call
            this.DataSource = TrasnferToClinicObjectDataSource;
           
        }
    }
}