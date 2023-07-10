using System;
using System.IO;
using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Security.Permissions;

namespace HCC.Showcase
{
    public partial class Edit : PortalModuleBase
	{
        protected DropDownList cboOwner;
        protected DropDownList cboSite;
        protected TextBox txtURL;
        protected TextBox txtTitle;
        protected TextBox txtDescription;
        protected CheckBoxList chkCategories;
        protected CheckBox chkActive;
        protected LabelControl plThumbnail;
        protected DropDownList cboThumbnail;
        protected Image imgThumbnail;

		private void LoadSites()
		{
            SiteController objSites = new SiteController();
            cboSite.Items.Clear();
			cboSite.Items.Add(new ListItem("<Create New Item>", "-1"));
			foreach (var objSite in objSites.GetSitesByUserId(ModuleId, int.Parse(cboOwner.SelectedValue)))
            {
				cboSite.Items.Add(new ListItem(objSite.Title, objSite.SiteId.ToString()));
			}

			txtURL.Text = "";
			txtTitle.Text = "";
			txtDescription.Text = "";
            chkCategories.Items.Clear();
            string strCategories = Settings["categories"].ToString();
            foreach (string strCategory in strCategories.Split(','))
            {
                chkCategories.Items.Add(new ListItem(strCategory));
            }
			chkActive.Checked = true;
			imgThumbnail.ImageUrl = "";
			plThumbnail.Visible = false;
			cboThumbnail.Visible = false;
		}

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
            {
                SiteController objSites = new SiteController();
                cboOwner.Items.Add(new ListItem(UserInfo.DisplayName, UserInfo.UserID.ToString()));
                if (ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, "EDIT"))
                {
                    foreach (var objUser in objSites.GetSiteOwners(ModuleId))
                    {
                        cboOwner.Items.Add(new ListItem(objUser.DisplayName, objUser.UserId.ToString()));
                    }
                }
                cboOwner.SelectedIndex = 0;
                LoadSites();
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Please Be Patient When You Save An Item As The System Needs To Validate The Url And Generate A Thumbnail", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }

        protected void cboOwner_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            LoadSites();
        }

        protected void cboSite_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SiteController objSites = new SiteController();
			var objSite = objSites.GetSite(int.Parse(cboSite.SelectedValue));
			if (objSite != null)
            {
				txtURL.Text = objSite.URL;
				txtTitle.Text = objSite.Title;
				txtDescription.Text = objSite.Description;
                foreach (string strCategory in objSite.Categories.ToString().Split(','))
                {
                    if (chkCategories.Items.FindByValue(strCategory) != null)
                    {
                        chkCategories.Items.FindByValue(strCategory).Selected = true;
                    }
                }
                chkActive.Checked = objSite.IsActive;
				imgThumbnail.ImageUrl = objSite.Thumbnail;

				cboThumbnail.Items.Clear();
				string[] strFiles = Directory.GetFiles(PortalSettings.HomeDirectoryMapPath + Settings["folder"].ToString().Replace("/", "\\"), "Site" + int.Parse(cboSite.SelectedValue).ToString("00000") + "_*");
				foreach (string strFile in strFiles)
                {
					cboThumbnail.Items.Insert(0, new ListItem(strFile.Substring(strFile.IndexOf("_") + 1), strFile));
				}
				if (cboThumbnail.Items.Count > 0)
                {
					plThumbnail.Visible = true;
					cboThumbnail.Visible = true;
					cboThumbnail.SelectedIndex = 0;
				}
			}
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
            if (!string.IsNullOrEmpty(txtTitle.Text) & !string.IsNullOrEmpty(txtDescription.Text))
            {
                bool blnValid = true;
                string strURL = DotNetNuke.Common.Globals.AddHTTP(txtURL.Text);

                string strValidate = strURL;
                if (bool.Parse(Settings["unique"].ToString()))
                {
                    Uri objUri = new Uri(strURL);
                    strValidate = objUri.Host;
                }

                SiteController objSites = new SiteController();
                foreach (var objSite in objSites.GetSitesByURL(ModuleId, strValidate))
                {
                    if (objSite.SiteId != int.Parse(cboSite.SelectedValue))
                    {
                        blnValid = false;
                    }
                }
                if (blnValid)
                {
                    if (objSites.ValidateSite(Settings, int.Parse(cboSite.SelectedValue), strURL))
                    {
                        string strCategories = "";
                        foreach(ListItem objCategory in chkCategories.Items)
                        {
                            if (objCategory.Selected)
                            {
                                strCategories += "," + objCategory.Value;
                            }
                        }
                        if (strCategories != "")
                        {
                            strCategories += ",";
                        }
                        int intSiteId = objSites.UpdateSite(int.Parse(cboSite.SelectedValue), ModuleId, strURL, txtTitle.Text, txtDescription.Text, strCategories, chkActive.Checked, "", int.Parse(cboOwner.SelectedValue));
                        if (chkActive.Checked)
                        {
                            string strThumbnail = objSites.CreateThumbnail(Settings, intSiteId, strURL, PortalSettings.HomeDirectoryMapPath, PortalSettings.HomeDirectory);
                            if (!string.IsNullOrEmpty(strThumbnail))
                            {
                                objSites.UpdateSite(intSiteId, ModuleId, strURL, txtTitle.Text, txtDescription.Text, strCategories, chkActive.Checked, strThumbnail, int.Parse(cboOwner.SelectedValue));
                                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Item Saved Successfully", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                            }
                            else
                            {
                                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "An Issue Was Encountered When Generating The Thumbnail", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                            }
                        }
                        else
                        {
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Item Saved Successfully", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                        }
                        LoadSites();
                    }
                    else
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Item Can Not Be Validated. " + Settings["instructions"], DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    }
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Item Already Exists", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Please Provide All Required Information", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }

		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
		}

		protected void cboThumbnail_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			imgThumbnail.ImageUrl = cboThumbnail.SelectedValue.Replace(PortalSettings.HomeDirectoryMapPath, PortalSettings.HomeDirectory).Replace("\\", "/");
		}
	}

}

