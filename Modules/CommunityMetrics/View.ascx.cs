using System;
using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Security;

namespace HCC.CommunityMetrics
{
    public partial class View : PortalModuleBase
	{
        protected ActionLink cmdManage;
        protected DropDownList cboDays;
        protected DropDownList cboActivity;
        protected DropDownList cboRows;
        protected Label lblFilter;
        protected DataGrid grdUserActivity;
        protected LinkButton cmdBack;
        protected Label lblActivities;

        private void BindData()
        {
            object intUserId = DBNull.Value;
            if (cmdBack.CommandArgument != "")
            {
                intUserId = int.Parse(cmdBack.CommandArgument);
            }

            DateTime datEnd = DateTime.Now.AddDays(-1);
            DateTime datStart = datEnd;
            if (string.IsNullOrEmpty((string)Settings["range"]) || (string)Settings["range"] == "D")
            {
                string strDays = "M";
                if (cboDays.Visible)
                {
                    strDays = cboDays.SelectedValue;
                }
                else
                {
                    if (Settings["days"] != null)
                    {
                        strDays = Settings["days"].ToString();
                    }
                }

                switch (strDays)
                {
                    case "D":
                        datStart = datEnd.AddDays(-1);
                        break;
                    case "W":
                        datStart = datEnd.AddDays(-7);
                        break;
                    case "M":
                        datStart = datEnd.AddMonths(-1);
                        break;
                    case "Q":
                        datStart = datEnd.AddMonths(-3);
                        break;
                    case "Y":
                        datStart = datEnd.AddYears(-1);
                        break;
                    case "*":
                        datStart = datEnd.AddYears(-99);
                        break;
                    default:
                        datStart = datEnd.AddMonths(-1);
                        break;
                }
                datStart = datStart.AddDays(1);
            }                
            else
            {
                if (!string.IsNullOrEmpty((string)Settings["start"]))
                {
                    datStart = DateTime.Parse(Settings["start"].ToString());
                }
                if (!string.IsNullOrEmpty((string)Settings["end"]))
                {
                    datEnd = DateTime.Parse(Settings["end"].ToString());
                }
            }

            object intActivityId = -1;
            if (cboActivity.Visible)
            {
                intActivityId = int.Parse(cboActivity.SelectedValue);
            }
            else
            {
                if (Settings["activity"] != null)
                {
                    intActivityId = int.Parse(Settings["activity"].ToString());
                }
            }
            if ((int)intActivityId == -1)
            {
                intActivityId = DBNull.Value;
            }

            int intRows = 10;
            if (cboRows.Visible)
            {
                intRows = int.Parse(cboRows.SelectedValue);
            }
            else
            {
                if (Settings["rows"] != null)
                {
                    intRows = int.Parse(Settings["rows"].ToString());
                }
            }

            bool blnSummary = true;
            if (cmdBack.CommandArgument != "") {
                blnSummary = false;
            }

            ActivityController objActivities = new ActivityController();
            grdUserActivity.DataSource = objActivities.GetUserActivities(intUserId, intActivityId, datStart, datEnd, blnSummary, intRows);
            grdUserActivity.DataBind();

            if ((string)Settings["days"] != "*")
            {
                lblFilter.Text = "<br /><b>" + datStart.ToString("MMM d, yyyy") + "</b> to <b>" + datEnd.ToString("MMM d, yyyy") + "</b>";
            }

            if (cmdBack.CommandArgument != "")
            {
                cmdBack.Visible = true;
            }
            else
            {
                cmdBack.Visible = false;
            }

        }

        private void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (DotNetNuke.Security.Permissions.ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", ModuleConfiguration))
            {
                cmdManage.Visible = true;
            }
            else
            {
                cmdManage.Visible = false;
            }

            if (!Page.IsPostBack) {

                if (Settings["days"] != null)
                {
                    if (cboDays.Items.FindByValue(Settings["days"].ToString()) != null)
                    {
                        cboDays.Items.FindByValue(Settings["days"].ToString()).Selected = true;
                    }
                }
                else
                {
                    cboDays.SelectedIndex = 2;
                }

                ActivityController objActivities = new ActivityController();

                cboActivity.Items.Add(new ListItem("All Activities", "-1"));
                foreach (var objActivity in objActivities.GetActivities())
                {
                    if (objActivity.IsActive)
                    {
                        cboActivity.Items.Add(new ListItem(objActivity.ActivityName, objActivity.ActivityId.ToString()));
                    }
                }
                if (Settings["activity"] != null)
                {
                    if (cboActivity.Items.FindByValue(Settings["activity"].ToString()) != null)
                    {
                        cboActivity.Items.FindByValue(Settings["activity"].ToString()).Selected = true;
                    }
                }
                else
                {
                    cboActivity.SelectedIndex = 0;
                }

                if (Settings["rows"] != null)
                {
                    if (cboRows.Items.FindByValue(Settings["rows"].ToString()) != null)
                    {
                        cboRows.Items.FindByValue(Settings["rows"].ToString()).Selected = true;
                    }
                }
                else
                {
                    cboRows.SelectedIndex = 1;
                }

                if (Settings["filter"] != null)
                {
                    cboDays.Visible = bool.Parse((string)Settings["filter"]);
                    cboActivity.Visible = bool.Parse((string)Settings["filter"]);
                    cboRows.Visible = bool.Parse((string)Settings["filter"]);
                }
                if (!string.IsNullOrEmpty((string)Settings["range"]) && (string)Settings["range"] == "S")
                {
                    cboDays.Visible = false;
                }

                string strActivities = "";
                foreach (var objActivity in objActivities.GetActivities())
                {
                    if (objActivity.IsActive)
                    {
                        strActivities += ((strActivities == "") ? "" : ", ") + objActivity.ActivityName + " <a title=\"" + objActivity.Description + "\"><img src=\"" + ControlPath + "help.png\" alt=\"" + objActivity.Description + "\"></a>" + " (" + objActivity.Factor.ToString() + ")";
                    }
                }
                lblActivities.Text = "<b>Scoring Details:</b><br />" + strActivities;

                BindData();
            }
        }

		public string DisplayRank()
		{
			return "#" + (grdUserActivity.Items.Count + 1).ToString();
		}

		public string DisplayPhoto(object intUserId, object strDisplayName)
		{
			string strPhoto = "<img ";
			strPhoto += "style=\"border:1px solid #; -webkit-border-radius: 32px; -moz-border-radius: 32px; border-radius: 32px;\" ";
			strPhoto += "src=\"" + DotNetNuke.Common.Globals.ApplicationPath + "/profilepic.ashx?userid=" + intUserId.ToString() + "&w=32&h=32\" ";
			strPhoto += "alt=\"" + strDisplayName.ToString() + "\" ";
			strPhoto += "width=\"32\" height=\"32\" ";
			strPhoto += ">";
			return strPhoto;
		}

		public string DisplayUser(object intUserId, object strDisplayName)
		{
            return "<a href=\"" + DotNetNuke.Common.Globals.ApplicationPath + "/Activity-Feed/userId/" + intUserId.ToString() + "\">" + strDisplayName + "</a>";
        }

		public string DisplayDate(object datDate)
		{
			return Convert.ToDateTime(datDate).ToString("MMM d, yyyy");
		}

        protected void grdUserActivity_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (cmdBack.CommandArgument != "") {
				e.Item.Cells[0].Text = "";
				e.Item.Cells[3].Text = "";
                e.Item.Cells[6].Text = "";
                if (e.Item.ItemIndex > 0) {
                    e.Item.Cells[1].Text = "";
                    e.Item.Cells[2].Text = "";
                }
            }
            else
            {
                e.Item.Cells[4].Text = "";
            }
        }

        protected void grdUserActivity_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "Details")
            {
                cmdBack.CommandArgument = e.CommandArgument.ToString();
                BindData();
            }
        }

        protected void cboDays_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindData();
        }
        protected void cboActivity_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindData();
        }

        protected void cboRows_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindData();
        }

        protected void cmdBack_Click(System.Object sender, System.EventArgs e)
        {
            cmdBack.CommandArgument = "";
            BindData();
        }

    }

}
