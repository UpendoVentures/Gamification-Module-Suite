using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using MaxMind.GeoIP2;
using System;

namespace HCC.VisitorsOnline
{
    public partial class View : PortalModuleBase
	{
        protected Label lblMessage;
        private string strLocations = "";

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (!Page.IsPostBack)
            {
                if (Request.IsAuthenticated)
                {
                    if (Settings["google"] == null)
                    {
                        lblMessage.Text = "Please Configure The Module In Settings";
                    }
                }
                BindData();
			}
		}

        private void BindData()
        {
            if (Settings["onlinetime"] != null)
            {
                int intUsers = 0;
                int intVisitors = 0;

                VisitorsOnlineController objVisitors = new VisitorsOnlineController();
                foreach (var objVisitor in objVisitors.GetVisitorsOnline(PortalId, int.Parse(Settings["onlinetime"].ToString())))
                {
                    if (objVisitor.Latitude != "" && objVisitor.Longitude != "")
                    {
                        if (strLocations != "")
                        {
                            strLocations += ",";
                        }
                        strLocations += "[";
                        if (string.IsNullOrEmpty(Convert.ToString(objVisitor.UserId)))
                        {
                            strLocations += "'red', ";
                            intVisitors += 1;
                        }
                        else
                        {
                            strLocations += "'blue', ";
                            intUsers += 1;
                        }
                        strLocations += objVisitor.Latitude + ", ";
                        strLocations += objVisitor.Longitude;
                        strLocations += "]";
                    }
                }
                lblMessage.Text = "<img src=\"http://maps.google.com/mapfiles/ms/icons/red.png\">&nbsp;<b>Visitors Online:</b> " + intVisitors.ToString() + "&nbsp;&nbsp;<img src=\"http://maps.google.com/mapfiles/ms/icons/blue.png\">&nbsp;<b>Users Online:</b> " + intUsers.ToString();
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