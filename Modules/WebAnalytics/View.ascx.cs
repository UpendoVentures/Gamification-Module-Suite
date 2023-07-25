using System;
using System.Data;
using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DotNetNuke.Entities.Portals;

namespace HCC.WebAnalytics
{
    public partial class View : PortalModuleBase
    {
        protected TextBox txtStartDate;
        protected HyperLink cmdStartDate;
        protected TextBox txtEndDate;
        protected HyperLink cmdEndDate;

        protected Label lblTotals;
        protected RadHtmlChart radGraph;
        protected DropDownList cboReport;
        protected DropDownList cboRows;
        protected DropDownList cboType;
        protected DataGrid grdReport;
        protected RadHtmlChart pieReport;

        protected RadioButtonList optLoggingMethod;
        protected TextBox txtRetentionHistory;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdStartDate.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStartDate);
            cmdEndDate.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtEndDate);

            if (!Page.IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.AddDays(-14).ToString("d");
                txtEndDate.Text = DateTime.Now.ToString("d");

                cboReport.Items.Add(new ListItem("Dates", "convert(varchar,Date,101) AS Date"));
                cboReport.Items.Add(new ListItem("Week Days", "datepart(weekday,Date) AS WeekDay"));
                cboReport.Items.Add(new ListItem("Weeks", "datepart(week,Date) AS Week"));
                cboReport.Items.Add(new ListItem("Months", "datepart(month,Date) AS Month"));
                cboReport.Items.Add(new ListItem("Hours", "datepart(hour,Date) AS Hour"));
                cboReport.Items.Add(new ListItem("Users", "Username"));
                cboReport.Items.Add(new ListItem("Pages", "TabPath"));
                cboReport.Items.Add(new ListItem("Referring Domains", "ReferrerDomain"));
                cboReport.Items.Add(new ListItem("Referring URLs", "ReferrerURL"));
                cboReport.Items.Add(new ListItem("Device Types", "DeviceType"));
                cboReport.Items.Add(new ListItem("Visitors", "hccm_Visits.VisitorId AS VisitorId"));
                cboReport.Items.Add(new ListItem("Domains", "Domain"));
                cboReport.Items.Add(new ListItem("URLs", "hccm_Visits.URL AS URL"));
                cboReport.Items.Add(new ListItem("IP Addresses", "IP"));
                cboReport.Items.Add(new ListItem("Countries", "Country"));
                cboReport.Items.Add(new ListItem("Regions", "Region"));
                cboReport.Items.Add(new ListItem("Cities", "City"));
                cboReport.Items.Add(new ListItem("Languages", "Language"));
                cboReport.Items.Add(new ListItem("User Agents", "UserAgent"));
                cboReport.Items.Add(new ListItem("Devices", "Device"));
                cboReport.Items.Add(new ListItem("Platforms", "Platform"));
                cboReport.Items.Add(new ListItem("Browsers", "Browser"));
                cboReport.Items.Add(new ListItem("Campaigns", "Campaign"));
                cboReport.Items.Add(new ListItem("Servers", "Server"));
                cboReport.SelectedIndex = 0;

                cboRows.Items.Add(new ListItem("Top 10", "10"));
                cboRows.Items.Add(new ListItem("Top 20", "20"));
                cboRows.Items.Add(new ListItem("Top 50", "50"));
                cboRows.Items.Add(new ListItem("Top 100", "100"));
                cboReport.SelectedIndex = 0;

                cboType.SelectedIndex = 0;

                DisplayDashboard();
                DisplayReport();

                optLoggingMethod.Items.FindByValue(PortalController.GetPortalSetting("LoggingMethod", PortalId, "B")).Selected = true;
                txtRetentionHistory.Text = PortalController.GetPortalSetting("RetentionHistory", PortalId, "90");
            }

        }

        protected void cmdDisplay_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStartDate.Text) & !string.IsNullOrEmpty(txtEndDate.Text))
            {
                DisplayDashboard();
                DisplayReport();
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "You Must Provide A Start And End Date", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
        }

        protected void cmdUpdate_Click(object sender, System.EventArgs e)
        {
            PortalController.UpdatePortalSetting(PortalId, "LoggingMethod", optLoggingMethod.SelectedValue);
            PortalController.UpdatePortalSetting(PortalId, "RetentionHistory", txtRetentionHistory.Text);
        }

        private void DisplayDashboard()
        {
            object objStartDate = DBNull.Value;
            if (!string.IsNullOrEmpty(txtStartDate.Text))
            {
                objStartDate = System.DateTime.Parse(txtStartDate.Text);
            }
            object objEndDate = DBNull.Value;
            if (!string.IsNullOrEmpty(txtEndDate.Text))
            {
                objEndDate = System.DateTime.Parse(txtEndDate.Text).AddDays(1);
            }

            int Views = 0;
            int Visits = 0;
            int Visitors = 0;
            int Users = 0;

            VisitorController objVisitors = new VisitorController();
            IDataReader dr = objVisitors.GetVisitsDashboard(PortalId, objStartDate, objEndDate);

            if (dr.Read())
            {
                Views = Convert.ToInt32(dr["Views"]);
                Visits = Convert.ToInt32(dr["Visits"]);
                Visitors = Convert.ToInt32(dr["Visitors"]);
                Users = Convert.ToInt32(dr["Users"]);
            }

            string strTotals = "<b>Views:</b> " + Views.ToString();
            strTotals += "&nbsp;&nbsp;<b>Visits:</b> " + Visits.ToString();
            strTotals += "&nbsp;&nbsp;<b>Visitors:</b> " + Visitors.ToString();
            strTotals += "&nbsp;&nbsp;<b>Users:</b> " + Users.ToString();
            lblTotals.Text = strTotals;

            radGraph.PlotArea.XAxis.LabelsAppearance.RotationAngle = 270;
            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            DateTime StartDate = DateTime.Parse(objStartDate.ToString(), culture, System.Globalization.DateTimeStyles.AssumeLocal);
            DateTime EndDate = DateTime.Parse(objEndDate.ToString(), culture, System.Globalization.DateTimeStyles.AssumeLocal);
            DateTime CurrentDate = StartDate;
            while (CurrentDate < EndDate)
            {
                radGraph.PlotArea.XAxis.Items.Add(new AxisItem(CurrentDate.ToString("d")));
                CurrentDate += TimeSpan.FromDays(1);
            }

            LineSeries ViewsSeries = new LineSeries();
            ViewsSeries.Name = "Views";
            ViewsSeries.MissingValues = Telerik.Web.UI.HtmlChart.MissingValuesBehavior.Zero;
            ViewsSeries.LabelsAppearance.Visible = false;
            ViewsSeries.TooltipsAppearance.DataFormatString = "{0} Views";

            LineSeries VisitsSeries = new LineSeries();
            VisitsSeries.Name = "Visits";
            VisitsSeries.MissingValues = Telerik.Web.UI.HtmlChart.MissingValuesBehavior.Zero;
            VisitsSeries.LabelsAppearance.Visible = false;
            VisitsSeries.TooltipsAppearance.DataFormatString = "{0} Visits";

            LineSeries VisitorsSeries = new LineSeries();
            VisitorsSeries.Name = "Visitors";
            VisitorsSeries.MissingValues = Telerik.Web.UI.HtmlChart.MissingValuesBehavior.Zero;
            VisitorsSeries.LabelsAppearance.Visible = false;
            VisitorsSeries.TooltipsAppearance.DataFormatString = "{0} Visitors";

            LineSeries UsersSeries = new LineSeries();
            UsersSeries.Name = "Users";
            UsersSeries.MissingValues = Telerik.Web.UI.HtmlChart.MissingValuesBehavior.Zero;
            UsersSeries.LabelsAppearance.Visible = false;
            UsersSeries.TooltipsAppearance.DataFormatString = "{0} Users";

            dr.NextResult();
            for (int intResult = 1; intResult <= 4; intResult++)
            {
                bool HasRows = dr.Read();
                CurrentDate = StartDate;
                while (CurrentDate < EndDate)
                {
                    if (HasRows == false || (Convert.ToDateTime(dr["Date"]).ToString("d")) != CurrentDate.ToString("d"))
                    {
                        switch (intResult)
                        {
                            case 1:
                                ViewsSeries.Items.Add(new SeriesItem(0));
                                break;
                            case 2:
                                VisitsSeries.Items.Add(new SeriesItem(0));
                                break;
                            case 3:
                                VisitorsSeries.Items.Add(new SeriesItem(0));
                                break;
                            case 4:
                                UsersSeries.Items.Add(new SeriesItem(0));
                                break;
                        }
                    }
                    else
                    {
                        switch (intResult)
                        {
                            case 1:
                                ViewsSeries.Items.Add(new SeriesItem(Convert.ToInt32(dr["Views"])));
                                break;
                            case 2:
                                VisitsSeries.Items.Add(new SeriesItem(Convert.ToInt32(dr["Visits"])));
                                break;
                            case 3:
                                VisitorsSeries.Items.Add(new SeriesItem(Convert.ToInt32(dr["Visitors"])));
                                break;
                            case 4:
                                UsersSeries.Items.Add(new SeriesItem(Convert.ToInt32(dr["Users"])));
                                break;
                        }
                        if (HasRows)
                        {
                            HasRows = dr.Read();
                        }
                    }
                    CurrentDate += TimeSpan.FromDays(1);
                }
                dr.NextResult();
            }
            radGraph.PlotArea.Series.Add(ViewsSeries);
            radGraph.PlotArea.Series.Add(VisitsSeries);
            radGraph.PlotArea.Series.Add(VisitorsSeries);
            radGraph.PlotArea.Series.Add(UsersSeries);

            dr.Close();
        }

        private void DisplayReport()
        {
            object objStartDate = DBNull.Value;
            if (!string.IsNullOrEmpty(txtStartDate.Text))
            {
                objStartDate = System.DateTime.Parse(txtStartDate.Text);
            }
            object objEndDate = DBNull.Value;
            if (!string.IsNullOrEmpty(txtEndDate.Text))
            {
                objEndDate = System.DateTime.Parse(txtEndDate.Text).AddDays(1);
            }

            VisitorController objVisitors = new VisitorController();
            IDataReader dr = objVisitors.GetVisitsReport(PortalId, objStartDate, objEndDate, cboReport.SelectedValue, int.Parse(cboRows.SelectedValue));

            switch (cboType.SelectedValue)
            {
                case "R":
                    grdReport.DataSource = dr;
                    grdReport.DataBind();
                    dr.Close();
                    grdReport.Visible = true;
                    pieReport.Visible = false;
                    break;
                case "C":
                    PieSeries objPieSeries = new PieSeries();
                    objPieSeries.StartAngle = 90;
                    objPieSeries.LabelsAppearance.Position = Telerik.Web.UI.HtmlChart.PieLabelsPosition.Circle;
                    objPieSeries.LabelsAppearance.DataFormatString = "{0} %";
                    objPieSeries.TooltipsAppearance.DataFormatString = "{0} %";

                    bool blnFirstItem = true;
                    string strColumn = cboReport.SelectedValue;
                    if (strColumn.IndexOf("AS") != -1)
                    {
                        strColumn = strColumn.Substring(strColumn.IndexOf("AS") + 3);
                    }
                    if (strColumn.IndexOf(":") != -1)
                    {
                        strColumn = strColumn.Substring(0, strColumn.IndexOf(":"));
                    }
                    while (dr.Read())
                    {
                        SeriesItem objSeriesItem = new SeriesItem();
                        objSeriesItem.Name = (string.IsNullOrEmpty(dr[strColumn].ToString()) ? "Unknown" : dr[strColumn].ToString()) + " (" + dr["Count"].ToString() + ")";
                        objSeriesItem.YValue = (decimal)dr["Percent"];
                        if (blnFirstItem)
                        {
                            objSeriesItem.Exploded = true;
                            blnFirstItem = false;
                        }
                        objPieSeries.Items.Add(objSeriesItem);
                    }
                    dr.Close();

                    pieReport.PlotArea.Series.Add(objPieSeries);
                    pieReport.Visible = true;
                    grdReport.Visible = false;
                    break;
            }
        }

    }

}
