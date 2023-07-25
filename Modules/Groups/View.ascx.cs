using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;
using System.Web;
using MaxMind.GeoIP2;
using System;

namespace HCC.Groups
{
    public partial class View : PortalModuleBase
	{

        protected ActionLink cmdManage;
        protected Label lblLogin;

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (!Page.IsPostBack)
            {
                if (Request.IsAuthenticated)
                {
                    if (Settings["google"] == null)
                    {
                        lblLogin.Text = "Please Configure The Module In Settings";
                        cmdManage.Visible = false;
                    }
                    else
                    {
                        lblLogin.Visible = false;
                        cmdManage.Visible = true;
                    }
                }
                else
                {
					lblLogin.Visible = true;
					cmdManage.Visible = false;
				}
			}
		}

        public string GetGoogleAPIKey()
        {
            if (Settings["google"] != null)
            {
                return Settings["google"].ToString();
            }
            else
            {
                return "";
            }
        }

        public string GetLocations()
        {
            string strLocations = "";
            GroupController objGroups = new GroupController();
            foreach (var objGroup in objGroups.GetGroups(PortalId))
            {
                if (strLocations != "")
                {
                    strLocations += ",";
                }
                strLocations += "[";
                strLocations += "'<a href=\"" + objGroup.URL + "\" target=\"_new\">" + HttpUtility.HtmlEncode(objGroup.GroupName) + "</a>', ";
                strLocations += objGroup.Latitude + ", ";
                strLocations += objGroup.Longitude;
                strLocations += "]";
            }
            return strLocations;
        }
        public string GetMapCenter()
        {
            string strLatLong = "37.09024, -95.712891"; // default to center of USA 
            using (var objGeoIP2DB = new DatabaseReader(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\GeoIP2-City.mmdb")))
            {
                try
                {
                    var objGeoIP2 = objGeoIP2DB.City(Request.UserHostAddress);
                    if (objGeoIP2.Country.Name != "N/A")
                    {
                        strLatLong = objGeoIP2.Location.Latitude.ToString() + ", " + objGeoIP2.Location.Longitude.ToString();
                    }
                }
                catch
                {
                    // IP address cannot be resolved
                }
            }
            return strLatLong;
        }

        public string GetMapHeight()
        {
            if (Settings["height"] != null)
            {
                return Settings["height"].ToString();
            }
            else
            {
                return "500";
            }
        }

    }

}