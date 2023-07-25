using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.WebControls;
using System.Linq;
using System.Web;

namespace HCC.Showcase
{
    public partial class View : PortalModuleBase
	{
		private int intPage = 1;

        protected Label lblLogin;
        protected ActionLink cmdAdd;
        protected DropDownList cboCategory;
        protected Literal litSites;
        protected HtmlControl ctlPaging;
        protected HyperLink cmdPrevious;
        protected Label lblPage;
        protected Label lblPages;
        protected HyperLink cmdNext;

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
			if ((Request.QueryString["p"] != null))
            {
				intPage = Convert.ToInt32(Request.QueryString["p"]);
			}

			if (!Page.IsPostBack)
            {
                if (Request.QueryString["id"] == null)
                {
                    if (Request.IsAuthenticated)
                    {
                        if (Settings["columns"] == null)
                        {
                            cmdAdd.Visible = false;
                            lblLogin.Text = "Please Configure The Module In Settings";
                        }
                        else
                        {
                            cmdAdd.Visible = true;
                            lblLogin.Visible = false;
                        }
                    }
                    else
                    {
                        cmdAdd.Visible = false;
                        lblLogin.Visible = true;
                    }
                }
                else
                {
                    cmdAdd.Visible = false;
                    lblLogin.Visible = false;
                    ctlPaging.Visible = false;
                    cboCategory.AutoPostBack = false;
                }
                cboCategory.Items.Add(new ListItem("<All Categories>", ""));
                if (Settings["categories"] != null && Request.QueryString["id"] == null)
                {
                    foreach (string strCategory in Settings["categories"].ToString().Split(','))
                    {
                        if (strCategory != "")
                        {
                            cboCategory.Items.Add(new ListItem(strCategory));
                        }
                    }
                }
                cboCategory.SelectedIndex = 0;
                if ((Request.QueryString["c"] != null))
                {
                    if (cboCategory.Items.FindByValue(Request.QueryString["c"]) != null)
                    {
                        cboCategory.ClearSelection();
                        cboCategory.Items.FindByValue(Request.QueryString["c"]).Selected = true;
                    }
                }
                BindData();
			}
		}

		private void BindData()
		{
			int intPages = 1;
			int intColumns = 3;
			int intRows = 3;

			if (Settings["columns"] != null)
            {
				intColumns = int.Parse(Settings["columns"].ToString());
			}
			if (Settings["rows"] != null)
            {
				intRows = int.Parse(Settings["rows"].ToString());
			}

            SiteController objSites = new SiteController();
            IEnumerable<dynamic> lstSites;
            if (Request.QueryString["id"] != null)
            {
                lstSites = objSites.GetSitesByUserId(ModuleId, int.Parse(Request.QueryString["id"]));
            }
            else
            {
                lstSites = objSites.GetSites(ModuleId, (intRows * intColumns), intPage, cboCategory.SelectedValue);
            }

            litSites.Text = DisplaySites(lstSites, intColumns);

            if (Request.QueryString["id"] == null)
            {
                if (Enumerable.Count(lstSites) > 0)
                {
                    intPages = (int)System.Math.Ceiling(((decimal)lstSites.First().Rows / (intRows * intColumns)));
                }

                lblPage.Text = intPage.ToString();
                lblPages.Text = intPages.ToString();
                if (intPage == 1)
                {
                    cmdPrevious.Visible = false;
                }
                else
                {
                    cmdPrevious.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "p=" + (intPage - 1).ToString() + ((cboCategory.SelectedValue == "") ? "" : "&c=" + cboCategory.SelectedValue));
                }
                if (intPage >= intPages)
                {
                    cmdNext.Visible = false;
                }
                else
                {
                    cmdNext.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "p=" + (intPage + 1).ToString() + ((cboCategory.SelectedValue == "") ? "" : "&c=" + cboCategory.SelectedValue));
                }
                if (intPages == 1)
                {
                    ctlPaging.Visible = false;
                }
            }
        }

        private string DisplaySites(IEnumerable<dynamic> lstSites, int intColumns)
        {
            int intSite = 0;
            string strSites = "<div><div class=\"row text-center\">";
            foreach (var objSite in lstSites)
            {
                intSite += 1;
                strSites += DisplaySite(objSite);
                if (intSite % intColumns == 0)
                {
                    strSites += "</div><div class=\"row text-center\">";
                }                
            }
            strSites += "</div></div>";
            return strSites;
        }

        private string DisplaySite(dynamic objSite)
		{
			string strTemplate = "<a href=\"[URL]\" title=\"[TITLE]\" target=\"_new\"><img src=\"[THUMBNAIL]\" alt=\"[TITLE]\" width=\"341\" height=\"256\"></a>";
			if (Settings["template"] != null) {
				strTemplate = Settings["template"].ToString();
			}
			strTemplate = strTemplate.Replace("[URL]", HttpUtility.HtmlEncode(objSite.URL.ToString()));
			strTemplate = strTemplate.Replace("[TITLE]", HttpUtility.HtmlEncode(objSite.Title.ToString()));
			strTemplate = strTemplate.Replace("[DESCRIPTION]", HttpUtility.HtmlEncode(objSite.Description.ToString()).Replace(System.Environment.NewLine, "<br />"));
			strTemplate = strTemplate.Replace("[THUMBNAIL]", objSite.Thumbnail.ToString());
            strTemplate = strTemplate.Replace("[USERID]", objSite.UserID.ToString());
            strTemplate = strTemplate.Replace("[USERNAME]", objSite.Username.ToString());
            strTemplate = strTemplate.Replace("[EMAIL]", objSite.Email.ToString());
            strTemplate = strTemplate.Replace("[DISPLAYNAME]", objSite.DisplayName.ToString());
            strTemplate = strTemplate.Replace("[CREATED]", Convert.ToDateTime(objSite.CreatedOnDate).ToShortDateString());
            if (Settings["width"] != null) {
                strTemplate = strTemplate.Replace("[WIDTH]", Settings["width"].ToString());
            }
            if (Settings["height"] != null) {
                strTemplate = strTemplate.Replace("[HEIGHT]", Settings["height"].ToString());
            }
			return strTemplate;
		}

        protected void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", ((cboCategory.SelectedValue == "") ? "" : "&c=" + cboCategory.SelectedValue)));
        }

    }
}

