using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Web.UI.WebControls;

namespace HCC.Partners
{
    public partial class DetailSettings : ModuleSettingsBase
	{

        protected TextBox txtTemplate;

        public override void LoadSettings()
		{
			try
            {
				if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty((string)ModuleSettings["template"]))
                    {
                        txtTemplate.Text = (string)ModuleSettings["template"];
                    }
                    else
                    {
                        txtTemplate.Text = "<a href=\"[WEBSITE]\" title=\"[NAME]\" target=\"_new\">[NAME]</a>";
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
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "template", txtTemplate.Text);
            }
            catch (Exception exc)
            {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

	}

}

