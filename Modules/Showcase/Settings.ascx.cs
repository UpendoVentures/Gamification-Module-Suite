using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using System.Web.UI.WebControls;

namespace HCC.Showcase
{
    public partial class Settings : ModuleSettingsBase
	{
        protected DropDownList cboFolder;
        protected TextBox txtRows;
        protected TextBox txtColumns;
        protected TextBox txtCategories;
        protected TextBox txtTemplate;
        protected TextBox txtURL;
        protected TextBox txtWidth;
        protected TextBox txtHeight;
        protected TextBox txtRefresh;
        protected CheckBox chkHistory;
        protected CheckBox chkUnique;
        protected TextBox txtValidation;
        protected TextBox txtInstructions;

        public override void LoadSettings()
		{
			try
            {
				if (!Page.IsPostBack)
                {
					foreach (FolderInfo objFolder in FolderManager.Instance.GetFolders(PortalId))
                    {
						cboFolder.Items.Add(new ListItem("/" + objFolder.FolderPath));
					}

					if (ModuleSettings["rows"] != null)
                    {
						txtRows.Text = (string)ModuleSettings["rows"];
					} else {
						txtRows.Text = "3";
					}
					if (ModuleSettings["columns"] != null)
                    {
						txtColumns.Text = (string)ModuleSettings["columns"];
					} else {
						txtColumns.Text = "3";
					}
                    if (ModuleSettings["categories"] != null)
                    {
                        txtCategories.Text = (string)ModuleSettings["categories"];
                    }
                    else
                    {
                        txtCategories.Text = "";
                    }
                    if (ModuleSettings["template"] != null)
                    {
						txtTemplate.Text = (string)ModuleSettings["template"];
					} else {
						txtTemplate.Text = "<a href=\"[URL]\" title=\"[TITLE]\" target=\"_new\"><img src=\"[THUMBNAIL]\" alt=\"[TITLE]\" width=\"341\" height=\"256\"></a>";
					}
					if (ModuleSettings["url"] != null)
                    {
						txtURL.Text = (string)ModuleSettings["url"];
					} else {
						txtURL.Text = "<Use Internal Thumbnail Generator>";
					}
					if (ModuleSettings["width"] != null)
                    {
						txtWidth.Text = (string)ModuleSettings["width"];
					} else {
						txtWidth.Text = "1024";
					}
					if (ModuleSettings["height"] != null)
                    {
						txtHeight.Text = (string)ModuleSettings["height"];
					} else {
						txtHeight.Text = "768";
					}
                    if (ModuleSettings["folder"] != null)
                    {
                        if (cboFolder.Items.FindByValue("/" + ModuleSettings["folder"].ToString()) != null) {
                            cboFolder.Items.FindByValue("/" + ModuleSettings["folder"].ToString()).Selected = true;
                        }
                    }
                    if (ModuleSettings["refresh"] != null)
                    {
						txtRefresh.Text = (string)ModuleSettings["refresh"];
					} else {
						txtRefresh.Text = "7";
					}
					if (ModuleSettings["history"] != null)
                    {
						chkHistory.Checked = Convert.ToBoolean(ModuleSettings["history"]);
					} else {
						chkHistory.Checked = false;
					}
                    if (ModuleSettings["unique"] != null)
                    {
                        chkUnique.Checked = Convert.ToBoolean(ModuleSettings["unique"]);
                    } else {
                        chkUnique.Checked = true;
                    }
                    if (ModuleSettings["validation"] != null)
                    {
                        txtValidation.Text = (string)ModuleSettings["validation"];
                    }
                    if (ModuleSettings["instructions"] != null)
                    {
                        txtInstructions.Text = (string)ModuleSettings["instructions"];
                    }
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
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "rows", txtRows.Text);
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "columns", txtColumns.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "categories", txtCategories.Text.Replace(", ",","));
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "template", txtTemplate.Text);

				if (txtURL.Text != "<Use Internal Thumbnail Generator>")
                {
					ModuleController.Instance.UpdateModuleSetting(ModuleId, "url", DotNetNuke.Common.Globals.AddHTTP(txtURL.Text));
				}
                else
                {
					ModuleController.Instance.UpdateModuleSetting(ModuleId, "url", "");
				}
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "width", txtWidth.Text);
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "height", txtHeight.Text);
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "folder", cboFolder.SelectedValue.Substring(1));
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "refresh", txtRefresh.Text);
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "history", chkHistory.Checked.ToString());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "unique", chkUnique.Checked.ToString());
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "validation", txtValidation.Text);
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "instructions", txtInstructions.Text);
			}
            catch (Exception exc)
            {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

	}

}

