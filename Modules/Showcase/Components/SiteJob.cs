using System;
using System.Collections;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace HCC.Showcase
{
    public class SiteJob : DotNetNuke.Services.Scheduling.SchedulerClient
	{
		public SiteJob(DotNetNuke.Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem) : base()
		{
			ScheduleHistoryItem = objScheduleHistoryItem;
		}

		public override void DoWork()
		{
			try {
				string strMessage = this.Processing();
				ScheduleHistoryItem.Succeeded = true;
				ScheduleHistoryItem.AddLogNote("Succeeded. " + strMessage);
			} catch (Exception exc) {
				ScheduleHistoryItem.Succeeded = false;
				ScheduleHistoryItem.AddLogNote("Failed. " + exc.Message);
				Errored(ref exc);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
        }

		public string Processing()
		{
			string strMessage = "";
			SiteController objSites = new SiteController();

			foreach (var objSite in objSites.GetSites(-1, 1, 10000, ""))
            {
				if (objSite.IsActive) {
					PortalController objPortalController = new PortalController();
					PortalInfo objPortalSettings = objPortalController.GetPortal(objSite.PortalID);
					ModuleController objModules = new ModuleController();
					Hashtable objModuleSettings = objModules.GetModuleSettings(objSite.ModuleID);

					int intRefresh = 7;
					if (objModuleSettings["refresh"] != null) {
						intRefresh = int.Parse(objModuleSettings["refresh"].ToString());
					}
					if (DateTime.Now.Subtract(objSite.CreatedOnDate).Days % intRefresh == 0) {
						string strURL = objSite.URL;
						strMessage += "<br />Processing: " + strURL + "<br />";
						if (objSites.ValidateSite(objModuleSettings, objSite.SiteID, strURL)) {
							strMessage += "- Site Validated Successfully" + "<br />";
							string strThumbnail = objSites.CreateThumbnail(objModuleSettings, objSite.SiteID, strURL, objPortalSettings.HomeDirectoryMapPath, DotNetNuke.Common.Globals.ApplicationPath + "/" + objPortalSettings.HomeDirectory + "/");
							if (!string.IsNullOrEmpty(strThumbnail)) {
								strMessage += "- Created New Thumbnail: " + strThumbnail + "<br />";
								objSites.UpdateSite(objSite.SiteId, objSite.ModuleId, objSite.URL, objSite.Title, objSite.Description, objSite.Categories, objSite.IsActive, strThumbnail, objSite.UserId);
							} else {
								strMessage += "- Error Creating Thumbnail: " + strThumbnail + "<br />";
							}
						} else {
							strMessage += "- Validation Issue... Disabling Site And Notifying Owner" + "<br />";
							objSites.UpdateSite(objSite.SiteId, objSite.ModuleId, objSite.URL, objSite.Title, objSite.Description, objSite.Categories, false, objSite.Thumbnail, objSite.UserId);
							UserInfo objUser = UserController.GetUserById(objSite.PortalID, objSite.UserID);
							if ((objUser != null)) {
								try {
									DotNetNuke.Services.Mail.Mail.SendEmail(objUser.Email, objPortalSettings.Email, objPortalSettings.PortalName + " Site Gallery", "Your Site " + strURL + " Could Not Be Validated And Was Removed From The Gallery.");
								} catch {
									// error sending email
								}
							}
						}
					}
				}
			}

			return strMessage;
		}

	}

}


