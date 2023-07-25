using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;

namespace HCC.Personalization
{
    public partial class View : PortalModuleBase
	{
        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (!Page.IsPostBack)
            { 
                if (!ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, "EDIT"))
                { 
                    ContainerControl.Visible = false;
                }
            }
        }
    }
}