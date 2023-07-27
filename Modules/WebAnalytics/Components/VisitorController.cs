using System;
using System.Web;
using System.Web.Configuration;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using WebMatrix.Data;
using DotNetNuke.Entities.Portals;
using System.Collections;
using DotNetNuke.Data;
using DeviceDetectorNET;
using MaxMind.GeoIP2;
using DeviceDetectorNET.Parser;

namespace HCC.WebAnalytics
{
    public class VisitorController
    {
        public static string UserAgentFilter = "bot|crawl|spider|sbider|ask|slurp|larbin|search|indexer|archiver|nutch|capture|scanalert";

        public dynamic GetVisitor(int VisitorId)
        {
            dynamic objVisitor;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objVisitor = db.QuerySingle("exec hccm_GetVisitor @0", VisitorId);
            }
            return objVisitor;
        }

        public int AddVisitor(int PortalId)
        {
            int intVisitorId = -1;
            using (var db = Database.Open("SiteSqlServer"))
            {
                intVisitorId = db.QuerySingle("exec hccm_AddVisitor @0", PortalId).VisitorId;
            }
            return intVisitorId;
        }

        public void UpdateVisitor(int VisitorId, object UserId)
        {
            if (UserId != DBNull.Value)
            {
                using (var db = Database.Open("SiteSqlServer"))
                {
                    db.Execute("exec hccm_UpdateVisitor @0, @1", VisitorId, UserId);
                }
            }
        }

        public dynamic GetVisit(int VisitId)
        {
            dynamic objVisit;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objVisit = db.QuerySingle("exec hccm_GetVisit @0", VisitId);
            }
            return objVisit;
        }

        public void AddVisit(int PortalId, int VisitorId, int TabId, int UserId, string IP, string[] UserLanguages, string Domain, string URL, string UserAgent, Uri UrlReferrer, string Activity, string Campaign, Guid SessionId, Guid RequestId, Guid LastRequestId)
        {
            if (Domain.EndsWith("/"))
            {
                Domain = Domain.Substring(0, Domain.Length - 1);
            }

            // get referrer URL
            string ReferrerURL = "";
            if ((UrlReferrer != null))
            {
                ReferrerURL = UrlReferrer.ToString();
            }

            string ReferrerDomain = "";
            if (!string.IsNullOrEmpty(ReferrerURL))
            {
                System.Uri Uri = new System.Uri(ReferrerURL);
                ReferrerDomain = Uri.Host;
            }

            // get browser language
            string Language = "";
            if ((UserLanguages != null))
            {
                if (UserLanguages.Length != 0)
                {
                    Language = UserLanguages[0].ToLowerInvariant().Trim();
                }
            }

            // generate RequestID if not provided
            if (RequestId == Guid.Empty)
            {
                RequestId = System.Guid.NewGuid();
            }

            // create visitor object
            VisitorInfo objVisitor = new VisitorInfo();
            objVisitor.PortalId = PortalId;
            objVisitor.Date = DateTime.Now;
            objVisitor.VisitorId = VisitorId;
            objVisitor.TabId = ((TabId == -1) ? DBNull.Value : (object)TabId); 
            objVisitor.UserId = ((UserId == -1) ? DBNull.Value : (object)UserId);
            objVisitor.IP = IP;
            objVisitor.Country = "";
            objVisitor.Region = "";
            objVisitor.City = "";
            objVisitor.Latitude = "";
            objVisitor.Longitude = "";
            objVisitor.Language = Language;
            objVisitor.Domain = Domain;
            objVisitor.URL = URL;
            objVisitor.UserAgent = UserAgent;
            objVisitor.DeviceType = "Desktop";
            objVisitor.Device = "";
            objVisitor.Platform = "";
            objVisitor.Browser = "";
            objVisitor.ReferrerDomain = ReferrerDomain;
            objVisitor.ReferrerURL = ReferrerURL;
            objVisitor.Server = "";
            objVisitor.Activity = Activity;
            objVisitor.Campaign = Campaign;
            objVisitor.SessionId = SessionId;
            objVisitor.RequestId = RequestId;
            objVisitor.LastRequestId = ((LastRequestId == Guid.Empty) ? DBNull.Value : (object)LastRequestId);

            // if logging method is bulk
            if (PortalController.GetPortalSetting("LoggingMethod", PortalId, "B") == "B")
            {
                // save visitor object to memory
                HttpRuntime.Cache["HCCVISITOR" + RequestId.ToString()] = objVisitor;
            }
            else // direct logging
            {
                // populate visitor fields
                objVisitor = ProcessVisit(objVisitor);

                // add visit
                using (var db = Database.Open("SiteSqlServer"))
                {
                    db.Execute("exec hccm_AddVisit @0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @18, @20, @21, @22, @23, @24, @25",
                    objVisitor.PortalId, objVisitor.Date, objVisitor.VisitorId, objVisitor.TabId, objVisitor.UserId,
                    objVisitor.IP, objVisitor.Country, objVisitor.Region, objVisitor.City, objVisitor.Latitude, objVisitor.Longitude,
                    objVisitor.Language, objVisitor.Domain, objVisitor.URL,
                    objVisitor.UserAgent, objVisitor.DeviceType, objVisitor.Device, objVisitor.Platform, objVisitor.Browser,
                    objVisitor.ReferrerDomain, objVisitor.ReferrerURL, objVisitor.Server, objVisitor.Campaign,
                    objVisitor.SessionId, objVisitor.RequestId, objVisitor.LastRequestId);
                }

                // update visitor
                UpdateVisitor(objVisitor.VisitorId, objVisitor.UserId);
            }
        }

        private VisitorInfo ProcessVisit(VisitorInfo objVisitor)
        {

            // get server
            objVisitor.Server = Dns.GetHostName();

            // get geo info based on IP 
            if (!string.IsNullOrEmpty(objVisitor.IP) && objVisitor.IP != "127.0.0.1")
            {
                using (var objGeoIP2DB = new DatabaseReader(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\GeoIP2-City.mmdb")))
                {
                    try
                    {
                        var objGeoIP2 = objGeoIP2DB.City(objVisitor.IP);
                        if (objGeoIP2.Country.Name != null && objGeoIP2.Country.Name != "N/A") { objVisitor.Country = objGeoIP2.Country.Name; }
                        if (objGeoIP2.MostSpecificSubdivision.Name != null) { objVisitor.Region = objGeoIP2.MostSpecificSubdivision.Name; }
                        if (objGeoIP2.City.Name != null) { objVisitor.City = objGeoIP2.City.Name; }
                        objVisitor.Latitude = objGeoIP2.Location.Latitude.ToString();
                        objVisitor.Longitude = objGeoIP2.Location.Longitude.ToString();
                    }
                    catch
                    {
                        // IP address cannot be resolved
                    }
                }
            }

            // get user agent properties
            if (!string.IsNullOrEmpty(objVisitor.UserAgent))
            {
                DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
                var userAgent = objVisitor.UserAgent; 
                
                var dd = new DeviceDetector(userAgent);
                dd.SkipBotDetection();
                dd.Parse();

                if (dd.IsMobile())
                {
                    objVisitor.DeviceType = "Mobile";
                }

                if (dd.GetBrandName() != null && dd.GetBrandName() != "Unknown")
                {
                    objVisitor.Device += dd.GetBrandName();
                }

                if (dd.GetModel() != null && dd.GetModel() != "Unknown")
                {
                    objVisitor.Device += dd.GetModel();
                }

                if (objVisitor.Device == "")
                {
                    objVisitor.Device = "Unavailable";
                }

                var osInfo = dd.GetOs();

                if (osInfo.Match.ToString() != null && osInfo.Match.ToString() != "Unknown")
                {
                    objVisitor.Platform += osInfo.Match.ToString(); // only available in Premium Data
                }

                if (objVisitor.Platform == "")
                {
                    objVisitor.Platform = "Unavailable";
                }

                var clientInfo = dd.GetClient();

                if (clientInfo.Match != null && clientInfo.Match.ToString() != "Unknown")
                {
                    objVisitor.Browser += clientInfo.Match.ToString(); // only available in Premium Data
                }

                if (objVisitor.Browser == "")
                {
                    objVisitor.Browser = "Unavailable";
                }
            }
            return objVisitor;
        }

        public void WriteVisits()
        {
            // create temporary table to store the visitor data
            DataTable dt = new DataTable();
            dt.Columns.Add("PortalId", typeof(int));
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("VisitorId", typeof(int));
            dt.Columns.Add("TabId", typeof(int)).AllowDBNull = true;
            dt.Columns.Add("UserId", typeof(int)).AllowDBNull = true;
            dt.Columns.Add("IP", typeof(string));
            dt.Columns.Add("Country", typeof(string));
            dt.Columns.Add("Region", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("Latitude", typeof(string));
            dt.Columns.Add("Longitude", typeof(string));
            dt.Columns.Add("Language", typeof(string));
            dt.Columns.Add("Domain", typeof(string));
            dt.Columns.Add("URL", typeof(string));
            dt.Columns.Add("UserAgent", typeof(string));
            dt.Columns.Add("DeviceType", typeof(string));
            dt.Columns.Add("Device", typeof(string));
            dt.Columns.Add("Platform", typeof(string));
            dt.Columns.Add("Browser", typeof(string));
            dt.Columns.Add("ReferrerDomain", typeof(string));
            dt.Columns.Add("ReferrerURL", typeof(string));
            dt.Columns.Add("Server", typeof(string));
            dt.Columns.Add("Campaign", typeof(string));
            dt.Columns.Add("SessionId", typeof(Guid));
            dt.Columns.Add("RequestId", typeof(Guid));
            dt.Columns.Add("LastRequestId", typeof(Guid)).AllowDBNull = true;

            // array to store the items to remove from Cache
            List<string> RemoveItems = new List<string>();

            // dictionary to store visitors to update
            Dictionary<int, object> dicVisitors = new Dictionary<int, object>();

            // get all visitor objects from Cache
            dynamic CacheItems = HttpRuntime.Cache.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).Where(key => key.StartsWith("HCCVISITOR")).ToArray();

            // iterate through visitor items
            foreach (string Key in CacheItems)
            {
                // get visitor object
                VisitorInfo objVisitor = (VisitorInfo)HttpRuntime.Cache.Get(Key);

                // populate visitor fields
                objVisitor = ProcessVisit(objVisitor);

                // save visitor to temp table
                dt.Rows.Add(objVisitor.PortalId, objVisitor.Date, objVisitor.VisitorId, objVisitor.TabId, objVisitor.UserId, 
                    objVisitor.IP, objVisitor.Country, objVisitor.Region, objVisitor.City, objVisitor.Latitude, objVisitor.Longitude, 
                    objVisitor.Language, objVisitor.Domain, objVisitor.URL,
                    objVisitor.UserAgent, objVisitor.DeviceType, objVisitor.Device, objVisitor.Platform, objVisitor.Browser, 
                    objVisitor.ReferrerDomain, objVisitor.ReferrerURL, objVisitor.Server, objVisitor.Campaign, 
                    objVisitor.SessionId, objVisitor.RequestId, objVisitor.LastRequestId);

                // save visitor 
                if (!dicVisitors.ContainsKey(objVisitor.VisitorId))
                {
                    dicVisitors.Add(objVisitor.VisitorId, objVisitor.UserId);
                }
                else
                {
                    dicVisitors[objVisitor.VisitorId] = objVisitor.UserId;
                }

                // add item to removal list
                RemoveItems.Add(Key);
            }

            // remove items from Cache
            foreach (string Key in RemoveItems)
            {
                HttpRuntime.Cache.Remove(Key);
            }

            // connect to database
            using (SqlConnection objConnection = new SqlConnection(WebConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString))
            {
                objConnection.Open();

                // bulk insert all visitor objects into database
                using (SqlBulkCopy objBulkCopy = new SqlBulkCopy(objConnection))
                {
                    for (int intColumn = 0; intColumn <= (dt.Columns.Count - 1); intColumn++)
                    {
                        objBulkCopy.ColumnMappings.Add(dt.Columns[intColumn].ColumnName, dt.Columns[intColumn].ColumnName);
                    }
                    objBulkCopy.BatchSize = 100;
                    objBulkCopy.DestinationTableName = "hccm_Visits";
                    objBulkCopy.WriteToServer(dt);
                }
            }

            // iterate through all visitors that need to be updated
            foreach (KeyValuePair<int, object> kvp in dicVisitors)
            {
                UpdateVisitor(kvp.Key, kvp.Value);
            }

        }

        public IDataReader GetVisitsDashboard(int PortalId, object StartDate, object EndDate)
        {
            return DataProvider.Instance().ExecuteReader("hccm_GetVisitsDashboard", PortalId, StartDate, EndDate);
        }

        public IDataReader GetVisitsReport(int PortalId, object StartDate, object EndDate, string Field, int Rows)
        {
            return DataProvider.Instance().ExecuteReader("hccm_GetVisitsReport", ReportQuery(Field, Rows), PortalId, StartDate, EndDate);
        }

        private string ReportQuery(string Field, int Rows)
        {
            string strField = Field;
            string strTotal = "COUNT(*)";
            if (strField.IndexOf(":") != -1)
            {
                strTotal = strField.Substring(strField.IndexOf(":") + 1);
                strField = strField.Substring(0, strField.IndexOf(":"));
            }
            string strGroup = strField;
            if (strGroup.IndexOf("AS") != -1)
            {
                strGroup = strGroup.Substring(0, strGroup.IndexOf("AS") - 1);
            }

            string strSQL = "";

            strSQL += "create table #Report ";
            strSQL += "( ";
            strSQL += " [ID] int IDENTITY (1, 1) NOT NULL, ";
            strSQL += " [Field] nvarchar(255) NULL, ";
            strSQL += " [Count] int NOT NULL ";
            strSQL += ") ";

            strSQL += "insert into #Report ([Field], [Count]) ";
            strSQL += "select TOP " + Rows.ToString() + " " + strField + ", " + strTotal + " AS [Count] ";
            strSQL += "from dbo.hccm_Visits ";
            strSQL += "left outer join dbo.Users on hccm_Visits.UserId = Users.UserId ";
            strSQL += "left outer join dbo.Tabs on hccm_Visits.TabId = Tabs.TabId ";
            strSQL += "where hccm_Visits.PortalId = @PortalID ";
            strSQL += "and ((hccm_Visits.Date >= @StartDate) or @StartDate is null) ";
            strSQL += "and ((hccm_Visits.Date <= @EndDate) or @EndDate is null) ";
            strSQL += "and (" + strGroup + " is not null and " + strGroup + " <> '') ";
            strSQL += "and hccm_Visits.TabId is not null ";
            strSQL += "group by " + strGroup + " ";
            strSQL += "order by [Count] desc ";

            strSQL += "declare @Total int ";
            strSQL += "select @Total = SUM([Count]) ";
            strSQL += "from #Report ";

            strSQL += "select TOP " + Rows.ToString() + " " + strField + ", " + strTotal + " As [Count], @Total As [Total], CONVERT(DECIMAL(5,2),100.0 * " + strTotal + " / @Total) As [Percent] ";
            strSQL += "from dbo.hccm_Visits ";
            strSQL += "left outer join dbo.Users on hccm_Visits.UserId = Users.UserId ";
            strSQL += "left outer join dbo.Tabs on hccm_Visits.TabId = Tabs.TabId ";
            strSQL += "where hccm_Visits.PortalId = @PortalId ";
            strSQL += "and ((hccm_Visits.Date >= @StartDate) or @StartDate is null) ";
            strSQL += "and ((hccm_Visits.Date <= @EndDate) or @EndDate is null) ";
            strSQL += "and (" + strGroup + " is not null and " + strGroup + " <> '') ";
            strSQL += "and hccm_Visits.TabId is not null ";
            strSQL += "group by " + strGroup + " ";
            strSQL += "order by [Count] desc";

            return strSQL;
        }

        public void PurgeVisits()
        {
            using (var db = Database.Open("SiteSqlServer"))
            {
                db.Execute("exec hccm_PurgeVisits");
            }
        }

    }
}
