using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml.Linq;
using System.IO;

namespace HCC.Groups
{
    public partial class Edit : PortalModuleBase
	{
        protected TextBox txtName;
        protected TextBox txtURL;
        protected TextBox txtCity;
        protected TextBox txtRegion;
        protected CheckBox chkActive;

        private void LoadGroup()
		{
            GroupController objGroups = new GroupController();
			var objGroup = objGroups.GetGroupByUserId(PortalId, UserId);
			if ((objGroup != null))
            {
				txtName.Text = objGroup.GroupName;
                txtURL.Text = objGroup.URL;
				txtCity.Text = objGroup.City;
				txtRegion.Text = objGroup.Region;
                chkActive.Checked = bool.Parse(objGroup.IsActive.ToString());
			} 
		}

		private void SaveGroup()
		{
            string Latitude = "";
            string Longitude = "";

            string url = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}", System.Uri.EscapeDataString(txtCity.Text + ", " + txtRegion.Text));
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            XDocument doc = XDocument.Load(response.GetResponseStream());
            if (doc.Element("GeocodeResponse").Element("status").Value == "OK")
            {
                XElement location = doc.Element("GeocodeResponse").Element("result").Element("geometry").Element("location");
                Latitude = location.Element("lat").Value;
                Longitude = location.Element("lng").Value;
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "City/Region Specified Is Not A Valid Location", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }

            string URL = DotNetNuke.Common.Globals.AddHTTP(txtURL.Text);
            if (Settings["format"].ToString() == "" || URL.ToLower().IndexOf(Settings["format"].ToString().ToLower()) != -1)
            {
                try
                {
                    request = WebRequest.Create(URL);
                    response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string content = reader.ReadToEnd();
                    reader.Close();
                    if (Settings["validation"].ToString() != "" && content.ToLower().IndexOf(Settings["validation"].ToString().ToLower()) == -1)
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "URL Specified Does Not Contain Validation Expression ( " + Settings["validation"].ToString() + " )", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                        URL = "";
                    }
                }
                catch
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "URL Specified Does Not Exist", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                    URL = "";
                }
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "URL Specified Is Not In Correct Format ( " + Settings["format"].ToString() + " )", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                URL = "";
            }

            if (Latitude != "" && Longitude != "" && URL != "")
            {
                GroupController objGroups = new GroupController();
                objGroups.UpdateGroup(PortalId, txtName.Text, URL, txtCity.Text, txtRegion.Text, Latitude, Longitude, chkActive.Checked, UserId);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Update Successful", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
            }
        }

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (!Page.IsPostBack) {
                if (Settings["instructions"].ToString() != "")
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Settings["instructions"].ToString(), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo);
                }
                LoadGroup();
			}
		}

        protected void cmdSave_Click(System.Object sender, System.EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtName.Text) & !string.IsNullOrEmpty(txtURL.Text) & !string.IsNullOrEmpty(txtCity.Text) & !string.IsNullOrEmpty(txtRegion.Text))
            {
                SaveGroup();
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Please Provide All Required Information", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
			}
		}

        protected void cmdCancel_Click(System.Object sender, System.EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
        }

    }

}
