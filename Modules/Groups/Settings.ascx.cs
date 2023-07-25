using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Web.UI.WebControls;

namespace HCC.Groups
{

    public partial class Settings : ModuleSettingsBase
	{

        protected TextBox txtGoogle;
        protected TextBox txtWidth;
        protected TextBox txtHeight;
        protected TextBox txtInstructions;
        protected TextBox txtFormat;
        protected TextBox txtValidation;

        public override void LoadSettings()
		{
			try {
				if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty((string)ModuleSettings["google"]))
                    {
                        txtGoogle.Text = (string)ModuleSettings["google"];
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["height"]))
                    {
                        txtHeight.Text = (string)ModuleSettings["height"];
                    }
                    else
                    {
                        txtHeight.Text = "500";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["instructions"]))
                    {
                        txtInstructions.Text = (string)ModuleSettings["instructions"];
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["format"]))
                    {
                        txtFormat.Text = (string)ModuleSettings["format"];
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["validation"]))
                    {
                        txtValidation.Text = (string)ModuleSettings["validation"];
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
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "google", txtGoogle.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "height", txtHeight.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "instructions", txtInstructions.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "format", txtFormat.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "validation", txtValidation.Text);
            }
            catch (Exception exc)
            {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

	}

}

