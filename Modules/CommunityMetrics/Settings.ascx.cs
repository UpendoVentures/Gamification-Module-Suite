using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace HCC.CommunityMetrics
{

    public partial class Settings : ModuleSettingsBase
	{

        protected CheckBox chkFilter;
        protected RadioButtonList optRange;
        protected HtmlGenericControl rowDays;
        protected DropDownList cboDays;
        protected HtmlGenericControl rowStart;
        protected TextBox txtStart;
        protected HtmlGenericControl rowEnd;
        protected TextBox txtEnd;
        protected DropDownList cboActivity;
        protected DropDownList cboRows;

        public override void LoadSettings()
		{
			try
            {
				if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty((string)ModuleSettings["filter"]))
                    {
                        chkFilter.Checked = bool.Parse((string)ModuleSettings["filter"]);
                    }
                    else
                    {
                        chkFilter.Checked = true;
                    }

                    optRange.Items.FindByValue("D").Selected = true;
                    if (!string.IsNullOrEmpty((string)ModuleSettings["range"]))
                    {
                        if (optRange.Items.FindByValue((string)ModuleSettings["range"]) != null)
                        {
                            optRange.ClearSelection();
                            optRange.Items.FindByValue((string)ModuleSettings["range"]).Selected = true;
                        }
                    }

                    cboDays.Items.FindByValue("M").Selected = true;
                    if (!string.IsNullOrEmpty((string)ModuleSettings["days"]))
                    {
                        if (cboDays.Items.FindByValue((string)ModuleSettings["days"]) != null)
                        {
                            cboDays.ClearSelection();
                            cboDays.Items.FindByValue((string)ModuleSettings["days"]).Selected = true;
                        }
                    }

                    if (!string.IsNullOrEmpty((string)ModuleSettings["start"]))
                    {
                        txtStart.Text = (string)ModuleSettings["start"];
                    }

                    if (!string.IsNullOrEmpty((string)ModuleSettings["end"]))
                    {
                        txtEnd.Text = (string)ModuleSettings["end"];
                    }

                    ActivityController objActivities = new ActivityController();
                    cboActivity.DataSource = objActivities.GetActivities();
                    cboActivity.DataBind();
                    cboActivity.Items.Insert(0, new ListItem("All Activities", "-1"));

                    cboActivity.Items.FindByValue("-1").Selected = true;
                    if (!string.IsNullOrEmpty((string)ModuleSettings["activity"]))
                    {
                        if (cboActivity.Items.FindByValue((string)ModuleSettings["activity"]) != null)
                        {
                            cboActivity.ClearSelection();
                            cboActivity.Items.FindByValue((string)ModuleSettings["activity"]).Selected = true;
                        }
                    }

                    cboRows.Items.FindByValue("10").Selected = true;
                    if (!string.IsNullOrEmpty((string)ModuleSettings["rows"]))
                    {
                        if (cboRows.Items.FindByValue((string)ModuleSettings["rows"]) != null)
                        {
                            cboRows.ClearSelection();
                            cboRows.Items.FindByValue((string)ModuleSettings["rows"]).Selected = true;
                        }
					}

                    DisplayRange();
                }
            }
            catch (Exception exc)
            {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		public override void UpdateSettings()
		{
			try
            {
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "filter", chkFilter.Checked.ToString());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "range", optRange.SelectedValue.ToString());
                if (optRange.SelectedValue == "D")
                {
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, "days", cboDays.SelectedValue);
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, "start", "");
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, "end", "");
                }
                else
                {
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, "days", "");
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, "start", txtStart.Text);
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, "end", txtEnd.Text);
                }
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "activity", cboActivity.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "rows", cboRows.SelectedValue);
            }
            catch (Exception exc)
            {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

        private void DisplayRange()
        {
            if (optRange.SelectedValue == "D")
            {
                rowDays.Visible = true;
                rowStart.Visible = false;
                rowEnd.Visible = false;
            }
            else
            {
                rowDays.Visible = false;
                rowStart.Visible = true;
                rowEnd.Visible = true;
            }
        }

        protected void optRange_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DisplayRange();
        }

    }

}

