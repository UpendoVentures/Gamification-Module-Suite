using System;
using DotNetNuke.Entities.Modules;
using System.Web;
using DotNetNuke.Services.FileSystem;
using System.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;

namespace HCC.Partners
{
    public partial class Detail : PortalModuleBase
	{

        protected Literal ctlDetail;

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    PartnerController objPartners = new PartnerController();
                    var objPartner = objPartners.GetPartnerByUserId(PortalId, int.Parse(Request.QueryString["id"]));
                    ctlDetail.Text = DisplayPartner(objPartner);
                }
                if (Settings["template"] == null)
                {
                    ctlDetail.Text = "Please Configure The Module In Settings";
                }
            }
        }

		public string DisplayPartner(dynamic objPartner)
		{
            PartnerController objPartners = new PartnerController();
            string strTemplate = "<a href=\"[URL]\" title=\"[NAME]\" target=\"_new\"><img src=\"[LOGO]\" alt=\"[NAME]\"></a>";
            if (Settings["template"] != null)
            {
                strTemplate = Settings["template"].ToString();
            }
			strTemplate = strTemplate.Replace("[ID]", objPartner.PartnerId.ToString());
			strTemplate = strTemplate.Replace("[NAME]", HttpUtility.HtmlEncode(objPartner.PartnerName));
			IFileInfo objFile = FileManager.Instance.GetFile(int.Parse(objPartner.Logo.ToString()));
			string strURL = FileManager.Instance.GetUrl(objFile);
			strTemplate = strTemplate.Replace("[LOGO]", strURL);
            strTemplate = strTemplate.Replace("[SUMMARY]", HttpUtility.HtmlEncode(objPartner.Summary).Replace(System.Environment.NewLine, "<br />"));
            strTemplate = strTemplate.Replace("[DESCRIPTION]", HttpUtility.HtmlEncode(objPartner.Description).Replace(System.Environment.NewLine, "<br />"));
            strTemplate = strTemplate.Replace("[CITY]", HttpUtility.HtmlEncode(objPartner.City));
			strTemplate = strTemplate.Replace("[REGION]", HttpUtility.HtmlEncode(objPartner.Region));
			strTemplate = strTemplate.Replace("[TELEPHONE]", HttpUtility.HtmlEncode(objPartner.Telephone));
            strTemplate = strTemplate.Replace("[URL]", HttpUtility.HtmlEncode(objPartner.URL));
            strTemplate = strTemplate.Replace("[WEBSITE]", new Uri(objPartner.URL).Host);
            strTemplate = strTemplate.Replace("[EMAIL]", "<a href='" + objPartner.Email.ToString().Replace("@", " [at] ") + "' rel='nofollow' onclick=\"this.href='mailto:' + '" + objPartner.Email.ToString().Substring(0, objPartner.Email.ToString().IndexOf("@")) + "' + '@' + '" + objPartner.Email.ToString().Substring(objPartner.Email.ToString().IndexOf("@") + 1) + "'\">" + HttpUtility.HtmlEncode(objPartner.Email.ToString()) + "</a>");
            strTemplate = strTemplate.Replace("[CONTACT]", HttpUtility.HtmlEncode(objPartner.Contact));
            if (strTemplate.Contains("[EMPLOYEES]"))
            {
                string strEmployees = "";
                foreach (var objUser in objPartners.GetPartnerUsers(objPartner.PartnerId))
                {
                    strEmployees += ((strEmployees == "") ? "" : ", ") + "<a href=" + DotNetNuke.Common.Globals.ApplicationPath + "/Activity-Feed/userId/" + objUser.UserId.ToString() + ">" + objUser.DisplayName + "</a>";
                }
                strTemplate = strTemplate.Replace("[EMPLOYEES]", strEmployees);
            }
            if (strTemplate.Contains("[SERVICES]"))
            {
                string strServices = objPartner.Services;
                if (strServices != "")
                {
                    strServices = strServices.Substring(1, strServices.Length - 2).Replace(",", ", ");
                }
                strTemplate = strTemplate.Replace("[SERVICES]", strServices);
            }
            return strTemplate;
		}

	}

}