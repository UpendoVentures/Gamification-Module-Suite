using System;
using System.Collections;
using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;
using System.Collections.Generic;
using DotNetNuke.Entities.Users;

namespace HCC.CommunityMetrics
{
    public partial class Edit : PortalModuleBase
	{
        protected DropDownList cboActivity;
        protected DropDownList cboTypeName;
        protected TextBox txtName;
        protected TextBox txtDescription;
        protected TextBox txtLastExecution;
        protected TextBox txtFactor;
        protected TextBox txtMinDaily;
        protected TextBox txtMaxDaily;
        protected RadioButtonList optMetricType;
        protected TextBox txtUserFilter;
        protected CheckBox chkIsActive;
        protected Repeater rptSettings;
        protected LinkButton cmdTest;
        protected LinkButton cmdCopy;
        protected TextBox txtStartDate;
        protected HyperLink cmdStartDate;
        protected TextBox txtEndDate;
        protected HyperLink cmdEndDate;
        protected RadHtmlChart radGraph;
        protected Label lblGraph;

        protected DropDownList cboActivities;
        protected TextBox txtDate;
        protected HyperLink cmdDate;
        protected DropDownList cboUser;
        protected TextBox txtUser;
        protected TextBox txtCount;
        protected TextBox txtNotes;

        private void ClearActivity()
		{
            ActivityController objActivities = new ActivityController();

			cboActivity.Items.Clear();
			cboActivity.DataSource = objActivities.GetActivities();
			cboActivity.DataBind();
			cboActivity.Items.Insert(0, new ListItem("<Create New Activity>", "-1"));
			cboActivity.SelectedIndex = 0;

			cboTypeName.Items.Clear();
			foreach (Type objType in objActivities.GetActivityTypes())
            {
				cboTypeName.Items.Add(new ListItem(objType.Name, objType.FullName));
			}
			cboTypeName.Items.Insert(0, new ListItem("<Not Specified>"));

			txtName.Text = "";
			txtDescription.Text = "";
			txtLastExecution.Text = DateTime.Now.AddDays(-1).ToString("MMM d, yyyy");
			txtFactor.Text = "1";
            txtMinDaily.Text = "0";
            txtMaxDaily.Text = "0";
            optMetricType.Enabled = true;
            txtUserFilter.Text = "";
			chkIsActive.Checked = true;

			rptSettings.DataSource = null;
			rptSettings.DataBind();

			cmdTest.Visible = false;
            cmdCopy.Visible = false;

            DisplayGraph();

            cboActivities.Items.Clear();
            cboActivities.DataSource = objActivities.GetActivities();
            cboActivities.DataBind();
            cboActivities.Items.Insert(0, new ListItem("<Select Activity>", "-1"));
            cboActivities.SelectedIndex = 0;
            cboUser.Items.Clear();
            cboUser.Items.Insert(0, new ListItem("<Select User>", "-1"));
            cboUser.Visible = true;
            txtUser.Visible = false;
            txtCount.Text = "";
            txtNotes.Text = "";
        }

        private void LoadActivity()
		{
			ActivityController objActivities = new ActivityController();

			ActivityInfo objActivity = objActivities.GetActivity(int.Parse(cboActivity.SelectedValue));
			if (objActivity != null)
            {
				txtName.Text = objActivity.ActivityName;
				txtDescription.Text = objActivity.Description;
				cboTypeName.ClearSelection();
				if (cboTypeName.Items.FindByValue(objActivity.TypeName) != null)
                {
					cboTypeName.Items.FindByValue(objActivity.TypeName).Selected = true;
					LoadSettings(objActivity.Settings, objActivity.MetricType);
				}
				txtLastExecution.Text = objActivity.LastExecutionDate.ToString("MMM d, yyyy");
				txtFactor.Text = objActivity.Factor.ToString();
                txtMinDaily.Text = objActivity.MinDaily.ToString();
                txtMaxDaily.Text = objActivity.MaxDaily.ToString();
                txtUserFilter.Text = objActivity.UserFilter;
                chkIsActive.Checked = objActivity.IsActive;
				cmdTest.Visible = true;
                cmdCopy.Visible = true;
                DisplayGraph();
            }
		}

		private void SaveActivity()
		{
			ActivityController objActivities = new ActivityController();

			bool blnValid = true;
			if (string.IsNullOrEmpty(txtName.Text) | cboTypeName.SelectedIndex < 1)
            {
				blnValid = false;
			}

			if (blnValid)
            {
				ActivityInfo objActivity = new ActivityInfo();
				objActivity.ActivityId = int.Parse(cboActivity.SelectedValue);
				objActivity.ActivityName = txtName.Text;
				objActivity.Description = txtDescription.Text;
				objActivity.TypeName = cboTypeName.SelectedValue;
				objActivity.LastExecutionDate = DateTime.Now.AddDays(-1);
				if (!string.IsNullOrEmpty(txtLastExecution.Text))
                {
					objActivity.LastExecutionDate = System.DateTime.Parse(txtLastExecution.Text);
				}
				objActivity.Factor = 1;
				if (!string.IsNullOrEmpty(txtFactor.Text))
                {
					objActivity.Factor = Double.Parse(txtFactor.Text);
				}
                objActivity.MinDaily = 0;
                if (!string.IsNullOrEmpty(txtMinDaily.Text))
                {
                    objActivity.MinDaily = int.Parse(txtMinDaily.Text);
                }
                objActivity.MaxDaily = 0;
                if (!string.IsNullOrEmpty(txtMaxDaily.Text))
                {
                    objActivity.MaxDaily = int.Parse(txtMaxDaily.Text);
                }
                objActivity.MetricType = (MetricTypeEnum)int.Parse(optMetricType.SelectedValue);
                objActivity.UserFilter = txtUserFilter.Text;
                objActivity.IsActive = chkIsActive.Checked;

				objActivity.ActivityId = objActivities.UpdateActivity(objActivity);

                for (int intItem = 0; intItem <= rptSettings.Items.Count - 1; intItem++)
                {
                    string strKey = ((Label)rptSettings.Items[intItem].FindControl("lblSetting")).Text;
                    string strValue = ((TextBox)rptSettings.Items[intItem].FindControl("txtSetting")).Text;
                    objActivities.UpdateActivitySetting(objActivity.ActivityId, strKey, strValue);
                }

                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Update Successful", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                ClearActivity();
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Activity Name, Activity Type, and Settings Are All Required", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
			}
		}

		private void LoadSettings(Hashtable Settings, MetricTypeEnum intMetricType)
		{
			if (cboTypeName.SelectedIndex > 0)
            {
				ActivityController objActivities = new ActivityController();
				Hashtable objSettings = objActivities.GetSettings(cboTypeName.SelectedValue);
				if (Settings != null)
                {
					foreach (DictionaryEntry objValue in Settings)
                    {
						if (objSettings.ContainsKey(objValue.Key))
                        {
							objSettings[objValue.Key] = objSettings[objValue.Key] + "|" + objValue.Value;
						}
					}
				}
                else
                {
                    for (int intItem = 0; intItem <= rptSettings.Items.Count - 1; intItem++)
                    {
                        string strKey = ((Label)rptSettings.Items[intItem].FindControl("lblSetting")).Text;
                        if (objSettings.ContainsKey(strKey))
                        {
                            string strValue = ((TextBox)rptSettings.Items[intItem].FindControl("txtSetting")).Text;
                            objSettings[strKey] = objSettings[strKey] + "|" + strValue;
                        }
                    }
                }
                rptSettings.DataSource = objSettings;
				rptSettings.DataBind();

				if (intMetricType == MetricTypeEnum.Undefined)
                {
					intMetricType = objActivities.GetMetricType(cboTypeName.SelectedValue);
				}
                if (optMetricType.Items.FindByText(intMetricType.ToString()) != null)
                {
                    optMetricType.Items.FindByText(intMetricType.ToString()).Selected = true;
                }
                else
                {
                    optMetricType.SelectedIndex = 0;
                }
                optMetricType.Enabled = true;
                if (intMetricType != MetricTypeEnum.Undefined)
                {
                    optMetricType.Enabled = false;
                }
			}
            else
            {
				rptSettings.DataSource = null;
				rptSettings.DataBind();
				optMetricType.SelectedIndex = 0;
				optMetricType.Enabled = true;
			}
		}

        private void DisplayGraph()
        {
            object objActivityId = DBNull.Value;
            if (cboActivity.SelectedIndex > 0)
            {
                objActivityId = int.Parse(cboActivity.SelectedValue);
            }
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

            ActivityController objActivities = new ActivityController();

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

            int ActivityId = -1;
            LineSeries objSeries = null;

            IDataReader dr = objActivities.GetDailyActivity(objActivityId, objStartDate, objEndDate);
            while (dr.Read())
            {
                if (Convert.ToInt32(dr["ActivityId"]) != ActivityId)
                {
                    if (objSeries != null && objSeries.Items.Count != 0)
                    {
                        while (CurrentDate < EndDate)
                        {
                            CurrentDate += TimeSpan.FromDays(1);
                            objSeries.Items.Add(new SeriesItem(0));
                        }
                        radGraph.PlotArea.Series.Add(objSeries);
                    }

                    ActivityId = Convert.ToInt32(dr["ActivityId"]);
                    objSeries = new LineSeries();
                    objSeries.Name = dr["ActivityName"].ToString();
                    objSeries.MissingValues = Telerik.Web.UI.HtmlChart.MissingValuesBehavior.Zero;
                    objSeries.LabelsAppearance.Visible = false;
                    objSeries.TooltipsAppearance.DataFormatString = "{0} " + dr["ActivityName"].ToString();
                    CurrentDate = StartDate;
                }

                while (Convert.ToDateTime(dr["Date"]).ToString("d") != CurrentDate.ToString("d"))
                {
                    objSeries.Items.Add(new SeriesItem(0));
                    CurrentDate += TimeSpan.FromDays(1);
                }
                objSeries.Items.Add(new SeriesItem(Convert.ToInt32(dr["Count"])));
                CurrentDate += TimeSpan.FromDays(1);
            }
            dr.Close();
            if (objSeries != null && objSeries.Items.Count != 0)
            {
                while (CurrentDate < EndDate)
                {
                    CurrentDate += TimeSpan.FromDays(1);
                    objSeries.Items.Add(new SeriesItem(0));
                }
                radGraph.PlotArea.Series.Add(objSeries);
            }

            if (radGraph.PlotArea.Series.Count == 0)
            {
                radGraph.Visible = false;
                lblGraph.Visible = true;
            }
            else
            {
                radGraph.Visible = true;
                lblGraph.Visible = false;
            }
        }

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            cmdStartDate.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStartDate);
            cmdEndDate.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtEndDate);
            cmdDate.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtDate);

            if (!Page.IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.AddDays(-7).ToString("d");
                txtEndDate.Text = DateTime.Now.ToString("d");
                ClearActivity();
            }
        }

        protected void rptSettings_ItemDataBound(object Sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
            {
				((Label)e.Item.FindControl("lblSetting")).Text = ((DictionaryEntry)e.Item.DataItem).Key.ToString();
                string strValue = "";
                string strHelp = ((DictionaryEntry)e.Item.DataItem).Value.ToString();
                if (strHelp.IndexOf("|") != -1 )
                {
                    strValue = strHelp.Substring(strHelp.IndexOf("|") + 1);
                    strHelp = strHelp.Replace("|" + strValue, "");
                }
                ((Label)e.Item.FindControl("lblHelp")).Text = strHelp;
                ((TextBox)e.Item.FindControl("txtSetting")).Text = strValue;
			}
		}

		protected void cboActivity_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cboActivity.SelectedIndex > 0)
            {
				LoadActivity();
			}
            else
            {
				ClearActivity();
			}
		}

		protected void cboTypeName_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			LoadSettings(null, MetricTypeEnum.Undefined);
		}

        protected void cmdUpdate_Click(System.Object sender, System.EventArgs e)
		{
			SaveActivity();
		}

        protected void cmdCancel_Click(System.Object sender, System.EventArgs e)
		{
			Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
		}

        protected void cmdCopy_Click(System.Object sender, System.EventArgs e)
        {
            cboActivity.SelectedIndex = 0;
            cmdCopy.Visible = false;
            cmdTest.Visible = false;
        }

        protected void cmdTest_Click(object sender, System.EventArgs e)
		{
			ActivityController objActivities = new ActivityController();
			ActivityInfo objActivity = objActivities.GetActivity(int.Parse(cboActivity.SelectedValue));
			try
            {
				var objTest = objActivities.GetUserActivity(objActivity, Convert.ToDateTime(txtLastExecution.Text));
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Test Executed Successfully For " + txtName.Text + " On " + txtLastExecution.Text, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
			}
            catch (Exception ex)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, ex.InnerException.ToString(), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
			}
		}

        protected void cboActivities_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ActivityController objActivities = new ActivityController();
            cboUser.DataSource = objActivities.GetUserActivities(DBNull.Value, int.Parse(cboActivities.SelectedValue), DateTime.Parse(txtDate.Text), DateTime.Parse(txtDate.Text), false, 10000);
            cboUser.DataBind();
            cboUser.Items.Insert(0, new ListItem("<Select User>", "-1"));
            cboUser.Items.Insert(1, new ListItem("<Enter User>", ""));
            cboUser.SelectedIndex = 0;
            cboUser.Visible = true;
            txtUser.Visible = false;
            txtCount.Text = "";
            txtNotes.Text = "";
        }

        protected void cboUser_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboUser.SelectedIndex == 1)
            {
                cboUser.Visible = false;
                txtUser.Visible = true;
            }
            else
            {
                ActivityController objActivities = new ActivityController();
                UserActivityInfo objUserActivity = objActivities.GetUserActivity(int.Parse(cboActivities.SelectedValue), int.Parse(cboUser.SelectedValue), DateTime.Parse(txtDate.Text));
                if (objUserActivity != null)
                {
                    txtCount.Text = objUserActivity.Count.ToString();
                    txtNotes.Text = objUserActivity.Notes;
                }
            }
        }

        protected void cmdSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intUserId = -1;
                if (txtUser.Visible)
                {
                    UserInfo objUser = UserController.GetUserByName(txtUser.Text);
                    if (objUser != null)
                    {
                        intUserId = objUser.UserID;
                    }
                }
                else
                {
                    intUserId = int.Parse(cboUser.SelectedValue);
                }
                if (intUserId != -1)
                {
                    ActivityController objActivities = new ActivityController();
                    objActivities.UpdateUserActivity(int.Parse(cboActivities.SelectedValue), intUserId, DateTime.Parse(txtDate.Text), int.Parse(txtCount.Text), txtNotes.Text);
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "User Activity Updated Successfully", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                    ClearActivity();
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Specified User Does Not Exist", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
            catch (Exception ex)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, ex.InnerException.ToString(), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
        }
    }

}
