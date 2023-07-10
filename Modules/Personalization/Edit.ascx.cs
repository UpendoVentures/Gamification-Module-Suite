using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using System.Collections;
using System.Web.UI.WebControls;

namespace HCC.Personalization
{
    public partial class Edit : PortalModuleBase
	{
        protected DropDownList cboModule;
        protected DataGrid grdRules;
        protected DropDownList cboAttribute;
        protected DropDownList cboCondition;
        protected TextBox txtValue;
        protected LinkButton cmdAdd;

        public string GetAttributeName(object strAttributeValue)
        {
            string strAttributeName = "";
            switch (strAttributeValue.ToString())
            {
                case "DEVICE:HardwareVendor":
                    strAttributeName = "Hardware Vendor";
                    break;
                case "DEVICE:HardwareModel":
                    strAttributeName = "Hardware Model";
                    break;
                case "DEVICE:HardwarePlatform":
                    strAttributeName = "Hardware Platform";
                    break;
                case "DEVICE:PlatformVendor":
                    strAttributeName = "Platform Vendor";
                    break;
                case "DEVICE:PlatformName":
                    strAttributeName = "Platform Name";
                    break;
                case "DEVICE:PlatformVersion":
                    strAttributeName = "Platform Version";
                    break;
                case "DEVICE:BrowserVendor":
                    strAttributeName = "Browser Vendor";
                    break;
                case "DEVICE:BrowserName":
                    strAttributeName = "Browser Name";
                    break;
                case "DEVICE:BrowserVersion":
                    strAttributeName = "Browser Version";
                    break;
                case "DEVICE:IsMobile":
                    strAttributeName = "Is Mobile Device?";
                    break;
                case "USERAGENT":
                    strAttributeName = "Device User Agent";
                    break;
                case "LANGUAGE":
                    strAttributeName = "Language";
                    break;
                case "LOCATION:COUNTRY":
                    strAttributeName = "Country";
                    break;
                case "LOCATION:REGION":
                    strAttributeName = "Region";
                    break;
                case "LOCATION:CITY":
                    strAttributeName = "City";
                    break;
                case "IP":
                    strAttributeName = "IP Address";
                    break;
                case "CAMPAIGN":
                    strAttributeName = "Campaign";
                    break;
                case "PAGE":
                    strAttributeName = "Page";
                    break;
                case "REFERRER":
                    strAttributeName = "Referrer";
                    break;
            }
            return strAttributeName;
        }

        public string GetConditionName(object strConditionValue)
        {
            string strConditionName = "";
            switch (strConditionValue.ToString())
            {
                case "=":
                    strConditionName = "Equals";
                    break;
                case "!=":
                    strConditionName = "Does Not Equal";
                    break;
                case "<":
                    strConditionName = "Is Less Than";
                    break;
                case ">":
                    strConditionName = "Is Greater Than";
                    break;
                case "~":
                    strConditionName = "Contains";
                    break;
                case "!~":
                    strConditionName = "Does Not Contain";
                    break;
                case "[]":
                    strConditionName = "Is In";
                    break;
                case "[!]":
                    strConditionName = "Is Not In";
                    break;
            }
            return strConditionName;
        }

        private void LoadRules()
		{
            ArrayList objRules = new ArrayList();
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule(int.Parse(cboModule.SelectedValue));
            if (objModule.ModuleSettings["personalization"] != null)
            {
                string[] arrRules = objModule.ModuleSettings["personalization"].ToString().Split('|');
                foreach (string strRule in arrRules)
                {
                    if (strRule != "")
                    {
                        string[] arrAttributes = strRule.Split(',');
                        if (arrAttributes.Length == 3)
                        {
                            Rule objRule = new Rule();
                            objRule.Attribute = arrAttributes[0];
                            objRule.Condition = arrAttributes[1];
                            objRule.Value = arrAttributes[2];
                            objRules.Add(objRule);
                        }
                    }
                }
            }
            grdRules.DataSource = objRules;
            grdRules.DataBind();
        }

        private void SaveRule()
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule(int.Parse(cboModule.SelectedValue));
            string strRules = "";
            if (objModule.ModuleSettings["personalization"] != null)
            {
                strRules = objModule.ModuleSettings["personalization"].ToString();
            }
            strRules += cboAttribute.SelectedValue + "," + cboCondition.SelectedValue + "," + txtValue.Text + "|";
            objModules.UpdateModuleSetting(int.Parse(cboModule.SelectedValue), "personalization", strRules);
            cboAttribute.SelectedIndex = 0;
            cboCondition.SelectedIndex = 0;
            txtValue.Text = "";
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Rule Added", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
        }

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (!Page.IsPostBack)
            {
                cboModule.Items.Add(new ListItem("<Select Module>", "-1"));
                foreach (ModuleInfo objModule in PortalSettings.ActiveTab.Modules)
                {
                    if (ModulePermissionController.CanViewModule(objModule) && !objModule.IsDeleted)
                    {
                        cboModule.Items.Add(new ListItem(objModule.ModuleTitle, objModule.ModuleID.ToString()));
                    }
                }
			}
		}


        protected void cmdAdd_Click(System.Object sender, System.EventArgs e)
		{
            if (cboModule.SelectedIndex > 0 && cboAttribute.SelectedIndex > 0 && cboCondition.SelectedIndex > 0 && txtValue.Text != "")
            {
                SaveRule();
                LoadRules();
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Please Specify All Required Fields", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }

        protected void cmdCancel_Click(System.Object sender, System.EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
        }

        protected void cboModule_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            LoadRules();
        }

        protected void grdRules_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "cmdDelete")
            {
                ArrayList objRules = new ArrayList();
                ModuleController objModules = new ModuleController();
                ModuleInfo objModule = objModules.GetModule(int.Parse(cboModule.SelectedValue));
                if (objModule.ModuleSettings["personalization"] != null)
                {
                    string strRules = "";
                    string[] arrRules = objModule.ModuleSettings["personalization"].ToString().Split('|');
                    foreach (string strRule in arrRules)
                    {
                        if (strRule != "")
                        {
                            string[] arrAttributes = strRule.Split(',');
                            if (arrAttributes[0] != e.CommandArgument.ToString())
                            {
                                strRules += strRule + "|";
                            }
                        }
                    }
                    objModules.UpdateModuleSetting(int.Parse(cboModule.SelectedValue), "personalization", strRules);
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Rule Removed", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                    LoadRules();
                }
            }
        }
	}

    public class Rule
    {
        public string Attribute { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }

    }
}
