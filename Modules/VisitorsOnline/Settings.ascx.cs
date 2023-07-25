using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Web.UI.WebControls;

namespace HCC.VisitorsOnline
{

    public partial class Settings : ModuleSettingsBase
	{

        protected TextBox txtGoogle;
        protected TextBox txtWidth;
        protected TextBox txtHeight;
        protected TextBox txtOnlineTime;

        public override void LoadSettings()
		{
			try {
				if (!Page.IsPostBack) {
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
                    if (!string.IsNullOrEmpty((string)ModuleSettings["onlinetime"]))
                    {
                        txtOnlineTime.Text = (string)ModuleSettings["onlinetime"];
                    }
                    else
                    {
                        txtOnlineTime.Text = "20";
                    }
                }
            } catch (Exception exc) {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		public override void UpdateSettings()
		{
			try {
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "google", txtGoogle.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "height", txtHeight.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "onlinetime", txtOnlineTime.Text);
            }
            catch (Exception exc) {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

	}

}

