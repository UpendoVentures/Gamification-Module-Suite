using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Web.UI.WebControls;
using System.Web;

namespace HCC.Partners
{

    public partial class Settings : ModuleSettingsBase
	{

        protected TextBox txtDays;
        protected TextBox txtRows;
        protected TextBox txtColumns;
        protected TextBox txtServices;
        protected TextBox txtTemplate;
        protected TextBox txtLogoWidth;
        protected TextBox txtLogoHeight;
        protected TextBox txtEmployees;
        protected TextBox txtSites;
        protected DropDownList cboShowcaseModule;
        protected TextBox txtPartnerRole;
        protected DataGrid grdPartners;

        public override void LoadSettings()
		{
			try
            {
				if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty((string)ModuleSettings["days"]))
                    {
                        txtDays.Text = (string)ModuleSettings["days"];
                    }
                    else
                    {
                        txtDays.Text = "30";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["rows"]))
                    {
                        txtRows.Text = (string)ModuleSettings["rows"];
                    }
                    else
                    {
                        txtRows.Text = "3";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["columns"]))
                    {
                        txtColumns.Text = (string)ModuleSettings["columns"];
                    }
                    else
                    {
                        txtColumns.Text = "3";
                    }
                    if (ModuleSettings["services"] != null)
                    {
                        txtServices.Text = (string)ModuleSettings["services"];
                    }
                    else
                    {
                        txtServices.Text = "Consulting,Hosting,Training,Design,Support";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["template"]))
                    {
                        txtTemplate.Text = (string)ModuleSettings["template"];
                    }
                    else
                    {
                        txtTemplate.Text = "<a href=\"[WEBSITE]\" title=\"[NAME]\" target=\"_new\">[NAME]</a>";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["logowidth"]))
                    {
                        txtLogoWidth.Text = (string)ModuleSettings["logowidth"];
                    }
                    else
                    {
                        txtLogoWidth.Text = "200";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["logoheight"]))
                    {
                        txtLogoHeight.Text = (string)ModuleSettings["logoheight"];
                    }
                    else
                    {
                        txtLogoHeight.Text = "200";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["employees"]))
                    {
                        txtEmployees.Text = (string)ModuleSettings["employees"];
                    }
                    else
                    {
                        txtEmployees.Text = "3";
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["sites"]))
                    {
                        txtSites.Text = (string)ModuleSettings["sites"];
                    }
                    else
                    {
                        txtSites.Text = "1";
                    }
                    ModuleController objModules = new ModuleController();
                    foreach (ModuleInfo objModule in objModules.GetModules(PortalId))
                    {
                        if (objModule.ModuleDefinition.DefinitionName == "Showcase")
                        {
                            cboShowcaseModule.Items.Add(new ListItem(objModule.ModuleTitle, objModule.ModuleID.ToString()));
                        }
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["showcasemodule"]))
                    {
                        if (cboShowcaseModule.Items.FindByValue((string)ModuleSettings["showcasemodule"]) != null)
                        {
                            cboShowcaseModule.Items.FindByValue((string)ModuleSettings["showcasemodule"]).Selected = true;
                        }
                    }
                    if (!string.IsNullOrEmpty((string)ModuleSettings["partnerrole"]))
                    {
                        txtPartnerRole.Text = (string)ModuleSettings["partnerrole"];
                    }
                    PartnerController objPartners = new PartnerController();
                    grdPartners.DataSource = objPartners.GetPartnerActivity(PortalId, DBNull.Value, DBNull.Value, DBNull.Value, DateTime.Now.AddDays(-int.Parse(txtDays.Text)), DateTime.Now, 10, 1, true);
                    grdPartners.DataBind();
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
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "days", txtDays.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "rows", txtRows.Text);
				ModuleController.Instance.UpdateModuleSetting(ModuleId, "columns", txtColumns.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "services", txtServices.Text.Replace(", ", ","));
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "template", txtTemplate.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "logowidth", txtLogoWidth.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "logoheight", txtLogoHeight.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "employees", txtEmployees.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "sites", txtSites.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "showcasemodule", cboShowcaseModule.SelectedValue);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "partnerrole", txtPartnerRole.Text);
            }
            catch (Exception exc)
            {
				// Module failed to load
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

        public string DisplayEmail(object Email, object Contact, object PartnerName, object Summary)
        {
            return "<a href=\"mailto:&cc=" + Email.ToString() + "&subject=" + ("Partner Introduction").Replace(" ","%20") + "&body=" + ("Dear ,%0A%0APlease allow me to introduce you to " + Contact.ToString() + " from " + PartnerName.ToString() + ". " + Summary.ToString() + ". I encourage you to connect with one another and discuss your specific project needs.%0A%0AThank You,%0A").Replace(" ", "%20") + "\">" + Email.ToString() + "</a>";
        }

    }

}

