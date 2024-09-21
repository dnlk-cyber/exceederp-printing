namespace ExceedERP.Reporting.Admin.Views
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for UsersInGroupOrRole.
    /// </summary>
    public partial class UsersInGroupOrRole : Telerik.Reporting.Report
    {
        public UsersInGroupOrRole()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = Organization;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}