using System;
using DotNetNuke.Entities.Modules;
using System.Web;
using DotNetNuke.Services.FileSystem;
using System.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;

namespace HCC.Partners
{
    public partial class View : PortalModuleBase
	{
        private int intPage = 1;

        protected ActionLink cmdManage;
        protected Label lblLogin;
        protected Label lblMessage;
        protected DropDownList cboService;
        protected Literal litPartners;
        protected HtmlControl ctlPaging;
        protected HyperLink cmdPrevious;
        protected Label lblPage;
        protected Label lblPages;
        protected HyperLink cmdNext;

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            if (Request.QueryString["p"] != null)
            {
                intPage = Convert.ToInt32(Request.QueryString["p"]);
            }

            if (!Page.IsPostBack)
            {
                if (Request.IsAuthenticated)
                {
                    if (Settings["columns"] == null)
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

                cboService.Items.Add(new ListItem("<All Services>", ""));
                if (Settings["services"] != null)
                {
                    foreach (string strService in Settings["services"].ToString().Split(','))
                    {
                        if (strService != "")
                        {
                            cboService.Items.Add(new ListItem(strService));
                        }
                    }
                }
                cboService.SelectedIndex = 0;
                if ((Request.QueryString["s"] != null))
                {
                    if (cboService.Items.FindByValue(Request.QueryString["s"]) != null)
                    {
                        cboService.ClearSelection();
                        cboService.Items.FindByValue(Request.QueryString["s"]).Selected = true;
                    }
                }

                BindData();
			}
		}

        private void BindData()
        {
            object objService = "";
            int intPages = 1;
            int intColumns = 3;
            int intRows = 3;

            if (cboService.SelectedValue == "")
            {
                objService = DBNull.Value;
            }
            else
            {
                objService = cboService.SelectedValue;
            }
            if (Settings["columns"] != null)
            {
                intColumns = int.Parse(Settings["columns"].ToString());
            }
            if (Settings["rows"] != null)
            {
                intRows = int.Parse(Settings["rows"].ToString());
            }
            int intDays = 30;
            if (Settings["days"] != null)
            {
                intDays = int.Parse(Settings["days"].ToString());
            }
            lblMessage.Text = "Partners Are Ranked Based On Their Past " + intDays.ToString() + " Days Of Activity";

            PartnerController objPartners = new PartnerController();
            IEnumerable<dynamic> lstPartners;
            lstPartners = objPartners.GetPartnerActivity(PortalId, DBNull.Value, objService, DBNull.Value, DateTime.Now.AddDays(-intDays), DateTime.Now, (intRows * intColumns), intPage, true);

            litPartners.Text = DisplayPartners(lstPartners, intColumns);

            if (Request.QueryString["id"] == null)
            {
                if (Enumerable.Count(lstPartners) > 0)
                {
                    intPages = (int)System.Math.Ceiling(((decimal)lstPartners.First().Rows / (intRows * intColumns)));
                }

                lblPage.Text = intPage.ToString();
                lblPages.Text = intPages.ToString();
                if (intPage == 1)
                {
                    cmdPrevious.Visible = false;
                }
                else
                {
                    cmdPrevious.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "p=" + (intPage - 1).ToString() + ((cboService.SelectedValue == "") ? "" : "&s=" + cboService.SelectedValue));
                }
                if (intPage >= intPages)
                {
                    cmdNext.Visible = false;
                }
                else
                {
                    cmdNext.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "p=" + (intPage + 1).ToString() + ((cboService.SelectedValue == "") ? "" : "&s=" + cboService.SelectedValue));
                }
                if (intPages == 1)
                {
                    ctlPaging.Visible = false;
                }
            }
        }

        private string DisplayPartners(IEnumerable<dynamic> lstPartners, int intColumns)
        {
            int intPartner = 0;
            string strPartners = "<div><div class=\"row text-center\">";
            foreach (var objPartner in lstPartners)
            {
                intPartner += 1;
                strPartners += DisplayPartner(objPartner);
                if (intPartner % intColumns == 0)
                {
                    strPartners += "</div><div class=\"row text-center\">";
                }
            }
            strPartners += "</div></div>";
            return strPartners;
        }

        private string DisplayPartner(dynamic objPartner)
		{
            string strTemplate = "<a href=\"[URL]\" title=\"[NAME]\" target=\"_new\"><img src=\"[LOGO]\" alt=\"[NAME]\"></a>";
            if (Settings["template"] != null)
            {
                strTemplate = Settings["template"].ToString();
            }
			strTemplate = strTemplate.Replace("[ID]", objPartner.PartnerId.ToString());
            strTemplate = strTemplate.Replace("[USERID]", objPartner.UserId.ToString());
            strTemplate = strTemplate.Replace("[NAME]", HttpUtility.HtmlEncode(objPartner.PartnerName.ToString()));
			IFileInfo objFile = FileManager.Instance.GetFile(int.Parse(objPartner.Logo.ToString()));
			string strURL = FileManager.Instance.GetUrl(objFile);
			strTemplate = strTemplate.Replace("[LOGO]", strURL);
			strTemplate = strTemplate.Replace("[SUMMARY]", HttpUtility.HtmlEncode(objPartner.Summary.ToString()).Replace(System.Environment.NewLine, "<br />"));
			strTemplate = strTemplate.Replace("[DESCRIPTION]", HttpUtility.HtmlEncode(objPartner.Description.ToString()).Replace(System.Environment.NewLine, "<br />"));
			strTemplate = strTemplate.Replace("[CITY]", HttpUtility.HtmlEncode(objPartner.City.ToString()));
			strTemplate = strTemplate.Replace("[REGION]", HttpUtility.HtmlEncode(objPartner.Region.ToString()));
			strTemplate = strTemplate.Replace("[TELEPHONE]", HttpUtility.HtmlEncode(objPartner.Telephone.ToString()));
			strTemplate = strTemplate.Replace("[URL]", HttpUtility.HtmlEncode(objPartner.URL.ToString()));
            strTemplate = strTemplate.Replace("[WEBSITE]", new Uri(objPartner.URL.ToString()).Host);
            strTemplate = strTemplate.Replace("[EMAIL]", "<a href='" + objPartner.Email.ToString().Replace("@", " [at] ") + "' rel='nofollow' onclick=\"this.href='mailto:' + '" + objPartner.Email.ToString().Substring(0, objPartner.Email.ToString().IndexOf("@")) + "' + '@' + '" + objPartner.Email.ToString().Substring(objPartner.Email.ToString().IndexOf("@") + 1) + "'\">" + HttpUtility.HtmlEncode(objPartner.Email.ToString()) + "</a>");
            strTemplate = strTemplate.Replace("[CONTACT]", HttpUtility.HtmlEncode(objPartner.Contact.ToString()));
            return strTemplate;
		}

        protected void cboService_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", ((cboService.SelectedValue == "") ? "" : "&s=" + cboService.SelectedValue)));
        }
    }

}