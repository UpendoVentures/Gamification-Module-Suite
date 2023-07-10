using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using WebMatrix.Data;
using System.Drawing;
using System.Web;
using System.IO;
using DotNetNuke.Common.Utilities;

namespace HCC.Showcase
{
    public class SiteController
	{
        public IEnumerable<dynamic> GetSites(int ModuleId, int Rows, int Page, string Category)
		{
            string cacheKey = "HCC.Showcase." + ModuleId.ToString() + "." + Rows.ToString() + "." + Page.ToString() + "." + Category.ToString();
            var args = new CacheItemArgs(cacheKey);
            IEnumerable<dynamic> objSites = CBO.GetCachedObject<IEnumerable<dynamic>>(args,
                delegate
                {
                    var objModuleId = (ModuleId == -1) ? (object)DBNull.Value : ModuleId;
                    var objCategory = (Category == "") ? (object)DBNull.Value : Category;
                    using (var db = Database.Open("SiteSqlServer"))
                    {
                        objSites = db.Query("exec hccm_GetSites @0, @1, @2, @3", objModuleId, Rows, Page, objCategory);
                    }
                    return objSites;
                }
            );
            return objSites;
        }

        public IEnumerable<dynamic> GetSiteOwners(int ModuleId)
        {
            IEnumerable<dynamic> objUsers;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objUsers = db.Query("exec hccm_GetSiteOwners @0", ModuleId);
            }
            return objUsers;
        }

        public IEnumerable<dynamic> GetSitesByUserId(int ModuleId, int UserId)
		{
            IEnumerable<dynamic> objSites;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objSites = db.Query("exec hccm_GetSitesByUserId @0, @1", ModuleId, UserId);
            }
            return objSites;
		}

		public IEnumerable<dynamic> GetSitesByURL(int ModuleId, string URL)
		{
            IEnumerable<dynamic> objSites;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objSites = db.Query("exec hccm_GetSitesByURL @0, @1", ModuleId, URL);
            }
            return objSites;
		}

        public dynamic GetSite(int SiteId)
		{
            dynamic objSite;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objSite = db.QuerySingle("exec hccm_GetSite @0", SiteId);
            }
            return objSite;
		}

		public int UpdateSite(int SiteId, int ModuleId, string URL, string Title, string Description, string Categories, bool IsActive, string Thumbnail, int UserId)
		{
            int intSiteId;
            using (var db = Database.Open("SiteSqlServer"))
            {
                intSiteId = db.QuerySingle("exec hccm_UpdateSite @0, @1, @2, @3, @4, @5, @6, @7, @8", SiteId, ModuleId, URL, Title, Description, Categories, IsActive, Thumbnail, UserId).SiteId;
            }
            return intSiteId;
		}

		public bool ValidateSite(Hashtable objModuleSettings, int SiteID, string URL)
		{
			bool blnValid = true;

            // first validate if the URL exists
            blnValid = ValidateURL(URL);
            if (!blnValid && URL.IndexOf("https://") != -1)
            {
                // if it was a secure URL that failed, try the unsecure version
                blnValid = ValidateURL(URL.Replace("https://", "http://"));
            }

            // now validate using any extra validation rules
			if (blnValid && objModuleSettings["validation"] != null)
            {
                if (objModuleSettings["validation"].ToString().StartsWith("/"))
                {
                    // validate if a specific file exists
                    Uri objUri = new Uri(URL);
                    string strValidation = objUri.Scheme + "://" + objUri.Host + objModuleSettings["validation"];
                    blnValid = ValidateURL(strValidation);
                    if (!blnValid && strValidation.IndexOf("https://") != -1)
                    {
                        // if it was a secure URL that failed, try the unsecure version
                        blnValid = ValidateURL(strValidation.Replace("https://", "http://"));
                    }
                }
                else
                {
                    // validate if response contains a phrase
                    blnValid = ValidateURL(URL, objModuleSettings["validation"].ToString());
                    if (!blnValid && URL.IndexOf("https://") != -1)
                    {
                        // if it was a secure URL that failed, try the unsecure version
                        blnValid = ValidateURL(URL.Replace("https://", "http://"), objModuleSettings["validation"].ToString());
                    }
                }
            }

            return blnValid;
		}

        private bool ValidateURL(string URL)
        {
            return ValidateURL(URL, "");
        }

        private bool ValidateURL(string URL, string Phrase)
		{
			bool blnValid = false;
			try
            {
				HttpWebRequest objHttpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
				objHttpWebRequest.Method = WebRequestMethods.Http.Get;
				objHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
				objHttpWebRequest.Referer = URL;
				objHttpWebRequest.Timeout = 10000;
				objHttpWebRequest.KeepAlive = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpWebResponse objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();
				if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    if (Phrase != "")
                    {
                        StreamReader reader = new StreamReader(objHttpWebResponse.GetResponseStream());
                        string content = reader.ReadToEnd();
                        reader.Close();
                        if (content.ToLower().IndexOf(Phrase.ToLower()) != -1)
                        {
                            blnValid = true;
                        }
                    }
                    else
                    {
                        blnValid = true;
                    }
                }
				objHttpWebResponse.Close();
			}
            catch
            {
				// invalid
			}
			return blnValid;
		}

        public string CreateThumbnail(Hashtable objModuleSettings, int SiteID, string URL, string HomeDirectoryMapPath, string HomeDirectory)
		{
			string strThumbnail = "";

			int intWidth = 1024;
			int intHeight = 768;
			string strVersion = "";
            bool blnHistory = false;
			if (objModuleSettings["width"] != null) {
				intWidth = int.Parse(objModuleSettings["width"].ToString());
			}
			if (objModuleSettings["height"] != null) {
				intHeight = int.Parse(objModuleSettings["height"].ToString());
			}
            strVersion = "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
            if (objModuleSettings["history"] != null)
            {
                blnHistory = bool.Parse(objModuleSettings["history"].ToString());
            }
            if (!blnHistory)
            {
                try {
                    foreach (string strFile in Directory.GetFiles(HomeDirectoryMapPath + objModuleSettings["folder"].ToString().Replace("/", "\\"), "Site" + SiteID.ToString("00000") + "*.*"))
                    {
                        File.Delete(strFile);
                    }
                }
                catch {
                    // error
                }
            }

			try {
				strThumbnail = HomeDirectoryMapPath + objModuleSettings["folder"].ToString().Replace("/", "\\") + "Site" + SiteID.ToString("00000") + strVersion + ".jpg";
				if (objModuleSettings["url"] != null && objModuleSettings["url"].ToString() != "") {
					string strURL = objModuleSettings["url"].ToString();
					strURL = strURL.Replace("[URL]", HttpUtility.UrlEncode(URL));
					strURL = strURL.Replace("[WIDTH]", intWidth.ToString());
					strURL = strURL.Replace("[HEIGHT]", intHeight.ToString());
					WebClient objWebClient = new WebClient();
					objWebClient.DownloadFile(strURL, strThumbnail);
				} else {
					Bitmap objBitmap = WebsiteThumbnail.GetThumbnail(URL, 1024, 768, intWidth, intHeight);
					objBitmap.Save(strThumbnail, System.Drawing.Imaging.ImageFormat.Jpeg);
					objBitmap.Dispose();
				}
				strThumbnail = HomeDirectory + objModuleSettings["folder"] + "Site" + SiteID.ToString("00000") + strVersion + ".jpg";
			} catch {
				// error
			}

			return strThumbnail;
		}
	}
}


