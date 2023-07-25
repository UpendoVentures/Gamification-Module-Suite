using System;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace HCC.WebAnalytics
{
	public class VisitorTracker : IHttpModule
	{
		private System.Text.RegularExpressions.Regex UserAgentFilter = new System.Text.RegularExpressions.Regex(VisitorController.UserAgentFilter, System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

		public string ModuleName {
			get { return "VisitorTracker"; }
		}

		public void Init(HttpApplication application)
		{
			application.EndRequest += this.OnEndRequest;
		}

		public void OnEndRequest(object s, EventArgs e)
		{
			HttpContext Context = ((HttpApplication)s).Context;
			HttpRequest Request = Context.Request;
			HttpResponse Response = Context.Response;

			HttpCookie objVisitorCookie = null;
			HttpCookie objSessionCookie = null;
			HttpCookie objRequestCookie = null;

            VisitorController objVisitors = new VisitorController();
            int VisitorId = -1;
			int UserId = -1;
			Guid SessionId = Guid.Empty;
			Guid RequestId = Guid.Empty;
			Guid LastRequestId = Guid.Empty;

			PortalSettings _portalSettings = (PortalSettings)Context.Items["PortalSettings"];

			// only process requests for content pages
			if (((_portalSettings != null)) && Request.Url.LocalPath.ToLower().EndsWith("default.aspx")) {
				// filter web crawlers and other bots
				if (String.IsNullOrEmpty(Request.UserAgent) == false && UserAgentFilter.Match(Request.UserAgent).Success == false) {                     
					// get last request cookie value
					objRequestCookie = Request.Cookies["HCCREQUEST"];
					if ((objRequestCookie != null)) {
						LastRequestId = new Guid(objRequestCookie.Value);
					}

					// create new request cookie
					RequestId = Guid.NewGuid();
					objRequestCookie = new HttpCookie("HCCREQUEST");
					objRequestCookie.Value = RequestId.ToString();
					Response.Cookies.Add(objRequestCookie);

					// get last session cookie value
					objSessionCookie = Request.Cookies["HCCSESSION"];
					if ((objSessionCookie != null)) {
						SessionId = new Guid(objSessionCookie.Value);
					} else {
						// create a new session id
						SessionId = Guid.NewGuid();
						objSessionCookie = new HttpCookie("HCCSESSION");
						objSessionCookie.Value = SessionId.ToString();
						objSessionCookie.Expires = DateTime.Now.AddMinutes(30);
						Response.Cookies.Add(objSessionCookie);
					}

					// get/set cookie if visitor tracking is enabled
					objVisitorCookie = Request.Cookies["HCCVISITOR"];

					if ((objVisitorCookie != null)) {
						VisitorId = Convert.ToInt32(objVisitorCookie.Value);
					}

					if (VisitorId == -1) {
						// create Visitor record 
						VisitorId = objVisitors.AddVisitor(_portalSettings.PortalId);

						// create Visitor cookie
						objVisitorCookie = new HttpCookie("HCCVISITOR");
						objVisitorCookie.Value = VisitorId.ToString();
						objVisitorCookie.Expires = DateTime.MaxValue;
						Response.Cookies.Add(objVisitorCookie);
					}

					// get User if authenticated
					if (Request.IsAuthenticated) {
						UserInfo objUser = UserController.Instance.GetCurrentUserInfo();
						if ((objUser != null)) {
                            UserId = objUser.UserID;
						}
					}

					// Visitor activity
					string Campaign = "";
					if ((Request.QueryString["campaign"] != null)) {
						Campaign = Request.QueryString["campaign"];
					}

					// add visit
					objVisitors.AddVisit(_portalSettings.PortalId, VisitorId, _portalSettings.ActiveTab.TabID, UserId, 
                        Request.UserHostAddress, Request.UserLanguages, Request.Url.Host + Request.ApplicationPath, Request.RawUrl, 
                        Request.UserAgent,Request.UrlReferrer, "click", Campaign, SessionId, RequestId, LastRequestId);
				}
			}

		}

		public void Dispose()
		{

		}

	}

}