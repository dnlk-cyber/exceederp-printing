
namespace ExceedERP.Reporting.HRM.HRM
{
    partial class TerminationSummaryByProjectReport
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for Telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            this.headerSubReport1 = new ExceedERP.Reporting.SharedDesigners.HeaderSubReport();
            this.thSN = new Telerik.Reporting.TextBox();
            this.thProjectName = new Telerik.Reporting.TextBox();
            this.thReason = new Telerik.Reporting.TextBox();
            this.thMale = new Telerik.Reporting.TextBox();
            this.thFemale = new Telerik.Reporting.TextBox();
            this.thTotal = new Telerik.Reporting.TextBox();
            this.BranchDataSource = new Telerik.Reporting.ObjectDataSource();
            this.TerminationSummaryDataSource = new Telerik.Reporting.ObjectDataSource();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.table1 = new Telerik.Reporting.Table();
            this.tbSN = new Telerik.Reporting.TextBox();
            this.tbProjectName = new Telerik.Reporting.TextBox();
            this.tbReason = new Telerik.Reporting.TextBox();
            this.tbMale = new Telerik.Reporting.TextBox();
            this.tbFemale = new Telerik.Reporting.TextBox();
            this.tbTotal = new Telerik.Reporting.TextBox();
            this.tbGrandTotalLabel = new Telerik.Reporting.TextBox();
            this.tbGrandTotalMale = new Telerik.Reporting.TextBox();
            this.tbGrandTotalFemale = new Telerik.Reporting.TextBox();
            this.tbGrandTotalAll = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.txtPageNumber = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // headerSubReport1
            // 
            this.headerSubReport1.Name = "HeaderSubReport";
            // 
            // thSN
            // 
            this.thSN.Name = "thSN";
            this.thSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.thSN.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(77)))), ((int)(((byte)(140)))));
            this.thSN.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thSN.Style.Color = System.Drawing.Color.White;
            this.thSN.Style.Font.Bold = true;
            this.thSN.Style.Font.Name = "Segoe UI";
            this.thSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thSN.Value = "S/N";
            // 
            // thProjectName
            // 
            this.thProjectName.Name = "thProjectName";
            this.thProjectName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.thProjectName.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(77)))), ((int)(((byte)(140)))));
            this.thProjectName.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thProjectName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thProjectName.Style.Color = System.Drawing.Color.White;
            this.thProjectName.Style.Font.Bold = true;
            this.thProjectName.Style.Font.Name = "Segoe UI";
            this.thProjectName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thProjectName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thProjectName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thProjectName.Value = "Project Name";
            // 
            // thReason
            // 
            this.thReason.Name = "thReason";
            this.thReason.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.thReason.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(77)))), ((int)(((byte)(140)))));
            this.thReason.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thReason.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thReason.Style.Color = System.Drawing.Color.White;
            this.thReason.Style.Font.Bold = true;
            this.thReason.Style.Font.Name = "Segoe UI";
            this.thReason.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thReason.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thReason.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thReason.Value = "Termination Reason / Type";
            // 
            // thMale
            // 
            this.thMale.Name = "thMale";
            this.thMale.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.thMale.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(77)))), ((int)(((byte)(140)))));
            this.thMale.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thMale.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thMale.Style.Color = System.Drawing.Color.White;
            this.thMale.Style.Font.Bold = true;
            this.thMale.Style.Font.Name = "Segoe UI";
            this.thMale.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thMale.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thMale.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thMale.Value = "Male";
            // 
            // thFemale
            // 
            this.thFemale.Name = "thFemale";
            this.thFemale.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.thFemale.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(77)))), ((int)(((byte)(140)))));
            this.thFemale.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thFemale.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thFemale.Style.Color = System.Drawing.Color.White;
            this.thFemale.Style.Font.Bold = true;
            this.thFemale.Style.Font.Name = "Segoe UI";
            this.thFemale.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thFemale.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thFemale.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thFemale.Value = "Female";
            // 
            // thTotal
            // 
            this.thTotal.Name = "thTotal";
            this.thTotal.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1180000305175781D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.thTotal.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(77)))), ((int)(((byte)(140)))));
            this.thTotal.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thTotal.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thTotal.Style.Color = System.Drawing.Color.White;
            this.thTotal.Style.Font.Bold = true;
            this.thTotal.Style.Font.Name = "Segoe UI";
            this.thTotal.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thTotal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thTotal.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thTotal.Value = "Total";
            // 
            // BranchDataSource
            // 
            this.BranchDataSource.DataMember = "GetBranch";
            this.BranchDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);
            this.BranchDataSource.Name = "BranchDataSource";
            // 
            // TerminationSummaryDataSource
            // 
            this.TerminationSummaryDataSource.DataMember = "GetTerminationSummaryByProject";
            this.TerminationSummaryDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.TerminationSummaryByProjectObjectDataSource);
            this.TerminationSummaryDataSource.Name = "TerminationSummaryDataSource";
            this.TerminationSummaryDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(object[]), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("fromDate", typeof(System.Nullable<System.DateTime>), "= Parameters.FromDate.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("toDate", typeof(System.Nullable<System.DateTime>), "= Parameters.ToDate.Value")});
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.64999997615814209D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport1});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.subReport1.Name = "subReport1";
            instanceReportSource1.ReportDocument = this.headerSubReport1;
            this.subReport1.ReportSource = instanceReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9D), Telerik.Reporting.Drawing.Unit.Inch(0.550000011920929D));
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detailSection1.Name = "detailSection1";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.39999997615814209D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8000000715255737D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.1999995708465576D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.1180000305175781D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.25D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2800000011920929D)));
            this.table1.Body.SetCellContent(0, 0, this.tbSN);
            this.table1.Body.SetCellContent(0, 1, this.tbProjectName);
            this.table1.Body.SetCellContent(0, 2, this.tbReason);
            this.table1.Body.SetCellContent(0, 3, this.tbMale);
            this.table1.Body.SetCellContent(0, 4, this.tbFemale);
            this.table1.Body.SetCellContent(0, 5, this.tbTotal);
            this.table1.Body.SetCellContent(1, 3, this.tbGrandTotalMale);
            this.table1.Body.SetCellContent(1, 4, this.tbGrandTotalFemale);
            this.table1.Body.SetCellContent(1, 5, this.tbGrandTotalAll);
            this.table1.Body.SetCellContent(1, 0, this.tbGrandTotalLabel, 1, 3);
            tableGroup1.ReportItem = this.thSN;
            tableGroup2.ReportItem = this.thProjectName;
            tableGroup3.ReportItem = this.thReason;
            tableGroup4.ReportItem = this.thMale;
            tableGroup5.ReportItem = this.thFemale;
            tableGroup6.ReportItem = this.thTotal;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.DataSource = this.TerminationSummaryDataSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tbSN,
            this.tbProjectName,
            this.tbReason,
            this.tbMale,
            this.tbFemale,
            this.tbTotal,
            this.tbGrandTotalLabel,
            this.tbGrandTotalMale,
            this.tbGrandTotalFemale,
            this.tbGrandTotalAll,
            this.thSN,
            this.thProjectName,
            this.thReason,
            this.thMale,
            this.thFemale,
            this.thTotal});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.10000000149011612D));
            this.table1.Name = "table1";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detailRow";
            this.table1.RowGroups.Add(tableGroup7);
            this.table1.RowGroups.Add(tableGroup8);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.9180002212524414D), Telerik.Reporting.Drawing.Unit.Inch(0.93000000715255737D));
            // 
            // tbSN
            // 
            this.tbSN.Name = "tbSN";
            this.tbSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.39999997615814209D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbSN.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbSN.Style.Font.Name = "Segoe UI";
            this.tbSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tbSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbSN.Value = "= RowNumber(\"table1\")";
            // 
            // tbProjectName
            // 
            this.tbProjectName.Name = "tbProjectName";
            this.tbProjectName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbProjectName.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbProjectName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbProjectName.Style.Font.Name = "Segoe UI";
            this.tbProjectName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbProjectName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.tbProjectName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbProjectName.Value = "= Fields.ProjectName";
            // 
            // tbReason
            // 
            this.tbReason.Name = "tbReason";
            this.tbReason.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1999998092651367D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbReason.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbReason.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbReason.Style.Font.Name = "Segoe UI";
            this.tbReason.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbReason.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.tbReason.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbReason.Value = "= Fields.TerminationReason";
            // 
            // tbMale
            // 
            this.tbMale.Name = "tbMale";
            this.tbMale.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbMale.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbMale.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbMale.Style.Font.Name = "Segoe UI";
            this.tbMale.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbMale.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbMale.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbMale.Value = "= Fields.MaleCount";
            // 
            // tbFemale
            // 
            this.tbFemale.Name = "tbFemale";
            this.tbFemale.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbFemale.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbFemale.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbFemale.Style.Font.Name = "Segoe UI";
            this.tbFemale.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbFemale.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbFemale.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbFemale.Value = "= Fields.FemaleCount";
            // 
            // tbTotal
            // 
            this.tbTotal.Name = "tbTotal";
            this.tbTotal.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1180000305175781D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbTotal.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbTotal.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbTotal.Style.Font.Name = "Segoe UI";
            this.tbTotal.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbTotal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbTotal.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbTotal.Value = "= Fields.TotalEmployee";
            // 
            // tbGrandTotalLabel
            // 
            this.tbGrandTotalLabel.Name = "tbGrandTotalLabel";
            this.tbGrandTotalLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.3999996185302734D), Telerik.Reporting.Drawing.Unit.Inch(0.2800000011920929D));
            this.tbGrandTotalLabel.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.tbGrandTotalLabel.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.tbGrandTotalLabel.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbGrandTotalLabel.Style.Font.Bold = true;
            this.tbGrandTotalLabel.Style.Font.Name = "Segoe UI";
            this.tbGrandTotalLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.tbGrandTotalLabel.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbGrandTotalLabel.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbGrandTotalLabel.Value = "Grand Total";
            // 
            // tbGrandTotalMale
            // 
            this.tbGrandTotalMale.Name = "tbGrandTotalMale";
            this.tbGrandTotalMale.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.2800000011920929D));
            this.tbGrandTotalMale.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.tbGrandTotalMale.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.tbGrandTotalMale.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbGrandTotalMale.Style.Font.Bold = true;
            this.tbGrandTotalMale.Style.Font.Name = "Segoe UI";
            this.tbGrandTotalMale.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.tbGrandTotalMale.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbGrandTotalMale.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbGrandTotalMale.Value = "= Sum(Fields.MaleCount)";
            // 
            // tbGrandTotalFemale
            // 
            this.tbGrandTotalFemale.Name = "tbGrandTotalFemale";
            this.tbGrandTotalFemale.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.2800000011920929D));
            this.tbGrandTotalFemale.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.tbGrandTotalFemale.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.tbGrandTotalFemale.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbGrandTotalFemale.Style.Font.Bold = true;
            this.tbGrandTotalFemale.Style.Font.Name = "Segoe UI";
            this.tbGrandTotalFemale.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.tbGrandTotalFemale.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbGrandTotalFemale.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbGrandTotalFemale.Value = "= Sum(Fields.FemaleCount)";
            // 
            // tbGrandTotalAll
            // 
            this.tbGrandTotalAll.Name = "tbGrandTotalAll";
            this.tbGrandTotalAll.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1180000305175781D), Telerik.Reporting.Drawing.Unit.Inch(0.2800000011920929D));
            this.tbGrandTotalAll.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.tbGrandTotalAll.Style.BorderColor.Default = System.Drawing.Color.DarkGray;
            this.tbGrandTotalAll.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbGrandTotalAll.Style.Font.Bold = true;
            this.tbGrandTotalAll.Style.Font.Name = "Segoe UI";
            this.tbGrandTotalAll.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.tbGrandTotalAll.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbGrandTotalAll.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbGrandTotalAll.Value = "= Sum(Fields.TotalEmployee)";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.34999999403953552D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtPageNumber});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // txtPageNumber
            // 
            this.txtPageNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6D), Telerik.Reporting.Drawing.Unit.Inch(0.05000000074505806D));
            this.txtPageNumber.Name = "txtPageNumber";
            this.txtPageNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtPageNumber.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtPageNumber.Value = "Page: {PageNumber}";
            // 
            // TerminationSummaryByProjectReport
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.reportHeaderSection1,
            this.detailSection1,
            this.pageFooterSection1});
            this.Name = "TerminationSummaryByProjectReport";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.BranchDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter1.MultiValue = true;
            reportParameter1.Name = "branchId";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.Name = "FromDate";
            reportParameter2.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.Name = "ToDate";
            reportParameter3.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter3.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(9D);
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.SubReport subReport1;
        private SharedDesigners.HeaderSubReport headerSubReport1;
        private Telerik.Reporting.ObjectDataSource BranchDataSource;
        private Telerik.Reporting.ObjectDataSource TerminationSummaryDataSource;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox thSN, thProjectName, thReason, thMale, thFemale, thTotal;
        private Telerik.Reporting.TextBox tbSN, tbProjectName, tbReason, tbMale, tbFemale, tbTotal;
        private Telerik.Reporting.TextBox tbGrandTotalLabel, tbGrandTotalMale, tbGrandTotalFemale, tbGrandTotalAll;
        private Telerik.Reporting.TextBox txtPageNumber;
    }
}