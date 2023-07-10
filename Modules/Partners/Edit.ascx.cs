using System;
using System.Collections;
using DotNetNuke.Entities.Modules;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;
using DotNetNuke.Entities.Users;
using HCC.Showcase;
using System.Linq;
using System.IO;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using System.Collections.Generic;

namespace HCC.Partners
{
    public partial class Edit : PortalModuleBase
	{
        protected DropDownList cboOwner;
        protected TextBox txtName;
        protected HtmlGenericControl rowLogo;
        protected DnnFilePickerUploader ctlLogo;
        protected TextBox txtSummary;
        protected TextBox txtDescription;
        protected TextBox txtCity;
        protected TextBox txtRegion;
        protected TextBox txtTelephone;
        protected TextBox txtURL;
        protected TextBox txtEmail;
        protected TextBox txtContact;
        protected CheckBox chkActive;
        protected CheckBoxList chkServices;
        protected TextBox txtUser;
        protected DataGrid grdUsers;
        protected LinkButton cmdAdd;
        protected DataGrid grdSites;
        protected Label lblSites;
        protected TextBox txtStart;
        protected HyperLink cmdStart;
        protected TextBox txtEnd;
        protected HyperLink cmdEnd;
        protected DataGrid grdActivity;
        protected Label lblActivity;

        private void LoadPartner()
		{
            int intDays = 30;
            if (Settings["days"] != null)
            {
                intDays = int.Parse(Settings["days"].ToString());
            }
            lblActivity.Text = "Partners Are Ranked Based On Their Past " + intDays.ToString() + " Days Of Community Activity";
            int intEmployees = 3;
            if (Settings["employees"] != null)
            {
                intEmployees = int.Parse(Settings["employees"].ToString());
            }
            int intSites = 1;
            if (Settings["sites"] != null)
            {
                intSites = int.Parse(Settings["sites"].ToString());
            }

            PartnerController objPartners = new PartnerController();
			var objPartner = objPartners.GetPartnerByUserId(PortalId, int.Parse(cboOwner.SelectedValue));
			if ((objPartner != null))
            {
				txtName.Text = objPartner.PartnerName;
                if (int.Parse(cboOwner.SelectedValue) == UserInfo.UserID)
                {
                    ctlLogo.FileID = int.Parse(objPartner.Logo);
                    rowLogo.Visible = true;
                }
                else
                {
                    rowLogo.Visible = false;
                }
                txtSummary.Text = objPartner.Summary;
				txtDescription.Text = objPartner.Description;
				txtCity.Text = objPartner.City;
				txtRegion.Text = objPartner.Region;
				txtTelephone.Text = objPartner.Telephone;
				txtURL.Text = objPartner.URL;
				txtEmail.Text = objPartner.Email;
                txtContact.Text = objPartner.Contact;
                chkActive.Checked = bool.Parse(objPartner.IsApproved.ToString());

                chkServices.Items.Clear();
                string strServices = Settings["services"].ToString();
                foreach (string strService in strServices.Split(','))
                {
                    chkServices.Items.Add(new ListItem(strService));
                }
                foreach (string strService in objPartner.Services.ToString().Split(','))
                {
                    if (chkServices.Items.FindByValue(strService) != null)
                    {
                        chkServices.Items.FindByValue(strService).Selected = true;
                    }
                }

				grdUsers.DataSource = objPartners.GetPartnerUsers(objPartner.PartnerId);
				grdUsers.DataBind();
				cmdAdd.Visible = true;

				SiteController objSites = new SiteController();
				grdSites.DataSource = objSites.GetSitesByUserId(int.Parse(Settings["showcasemodule"].ToString()), int.Parse(cboOwner.SelectedValue));
				grdSites.DataBind();
                ModuleController objModules = new ModuleController();
                IList<ModuleInfo> objModule = objModules.GetTabModulesByModule(int.Parse(Settings["showcasemodule"].ToString()));
                lblSites.Text = "You Must Enter Your Sites In The <a href=\"" + DotNetNuke.Common.Globals.NavigateURL(objModule.FirstOrDefault().TabID) + "\">Showcase</a>";

				if (grdUsers.Items.Count < intEmployees | grdSites.Items.Count < intSites)
                {
					chkActive.Checked = false;
					chkActive.Enabled = false;
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "In Order To Activate Your Partnership You Need To Add At Least " + intEmployees.ToString() + " Employees And " + intSites.ToString() + " Showcase Sites", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo);
				}
                else
                {
					chkActive.Enabled = true;
                    if (!chkActive.Checked)
                    {
                        chkActive.Checked = true;
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Your Partnership Meets The Minimum Criteria And Can Now Be Activated. Please Click Save To Continue.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo);
                    }
                }

                txtStart.Text = DateTime.Now.AddDays(-intDays).ToString("MMM d, yyyy");
                txtEnd.Text = DateTime.Now.ToString("MMM d, yyyy");
				grdActivity.DataSource = objPartners.GetPartnerActivity(PortalId, objPartner.PartnerId, DBNull.Value, DBNull.Value, Convert.ToDateTime(txtStart.Text), Convert.ToDateTime(txtEnd.Text), 0, 0, false);
				grdActivity.DataBind();
			}
            else
            {
				chkActive.Checked = false;
				chkActive.Enabled = false;
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Partners Must Have At Least " + intEmployees.ToString() + " Employees And " + intSites.ToString() + " Showcase Sites", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo);
				cmdAdd.Visible = false;
			}
		}

		private void SavePartner()
		{
            int intLogoWidth = 200;
            if (Settings["logowidth"] != null)
            {
                intLogoWidth = int.Parse(Settings["logowidth"].ToString());
            }
            int intLogoHeight = 200;
            if (Settings["logoheight"] != null)
            {
                intLogoHeight = int.Parse(Settings["logoheight"].ToString());
            }

            string strFileID = "";
            PartnerController objPartners = new PartnerController();
            if (rowLogo.Visible)
            {
                strFileID = objPartners.SaveLogo(DotNetNuke.Services.FileSystem.FolderManager.Instance.GetUserFolder(UserInfo).PhysicalPath + Path.GetFileName(ctlLogo.FilePath), new System.Drawing.Size(intLogoWidth, intLogoHeight), UserInfo);
            }
            else
            {
                var objPartner = objPartners.GetPartnerByUserId(PortalId, int.Parse(cboOwner.SelectedValue));
                if ((objPartner != null))
                {
                    strFileID = objPartner.Logo;
                }
            }

            string strServices = "";
            foreach (ListItem objService in chkServices.Items)
            {
                if (objService.Selected)
                {
                    strServices += "," + objService.Value;
                }
            }
            if (strServices != "")
            {
                strServices += ",";
            }

            int intPartnerId = objPartners.UpdatePartner(PortalId, txtName.Text, strFileID, txtSummary.Text, txtDescription.Text, txtCity.Text, txtRegion.Text, txtTelephone.Text, DotNetNuke.Common.Globals.AddHTTP(txtURL.Text), txtEmail.Text, txtContact.Text, strServices, chkActive.Checked, int.Parse(cboOwner.SelectedValue));

            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Update Successful", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
            cmdAdd.Visible = true;
            LoadPartner();
        }

        protected void Page_Load(System.Object sender, System.EventArgs e)
		{
            cmdStart.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStart);
            cmdEnd.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtEnd);

            if (!Page.IsPostBack)
            {
                PartnerController objPartners = new PartnerController();
                cboOwner.Items.Add(new ListItem(UserInfo.DisplayName, UserInfo.UserID.ToString()));
                if (ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, "EDIT"))
                {
                    foreach (var objUser in objPartners.GetPartnerOwners(PortalId))
                    {
                        if (objUser.UserId != UserInfo.UserID)
                        {
                            cboOwner.Items.Add(new ListItem(objUser.DisplayName, objUser.UserId.ToString()));
                        }
                    }
                }
                cboOwner.SelectedIndex = 0;
                LoadPartner();
			}
		}

        protected void cmdSave_Click(System.Object sender, System.EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtName.Text) & (ctlLogo.FileID != -1 || !rowLogo.Visible) & !string.IsNullOrEmpty(txtSummary.Text) & !string.IsNullOrEmpty(txtDescription.Text) & !string.IsNullOrEmpty(txtCity.Text) & !string.IsNullOrEmpty(txtRegion.Text) & !string.IsNullOrEmpty(txtTelephone.Text) & !string.IsNullOrEmpty(txtURL.Text) & !string.IsNullOrEmpty(txtEmail.Text))
            {
				SavePartner();
			}
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Please Provide All Required Information", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
			}
		}

        protected void cmdAdd_Click(System.Object sender, System.EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtEmail.Text))
            {
				// check if email address matches domain
				string strDomain = txtUser.Text;
				if (strDomain.IndexOf("@") != -1)
                {
					strDomain = strDomain.Substring(strDomain.IndexOf("@") + 1);
				}
				if (txtURL.Text.ToLower().Contains(strDomain.ToLower()))
                {
					// get user by email
					int intTotalRecords = 0;
					ArrayList arrUsers = UserController.GetUsersByEmail(PortalId, txtUser.Text, 0, 1, ref intTotalRecords);
					if (arrUsers.Count != 0)
                    {
						// check if user is already associated to a partner
						UserInfo objUser = (UserInfo)arrUsers[0];
						PartnerController objPartners = new PartnerController();
						if (Enumerable.Count(objPartners.GetUserPartners(objUser.UserID)) == 0)
                        {
							var objPartner = objPartners.GetPartnerByUserId(PortalId, int.Parse(cboOwner.SelectedValue));
							if ((objPartner != null))
                            {
								objPartners.AddPartnerUser(objPartner.PartnerId, objUser.UserID);
                                if (!string.IsNullOrEmpty((string)Settings["partnerrole"]))
                                {
                                    RoleController objRoles = new RoleController();
                                    RoleInfo objRole = objRoles.GetRoleByName(PortalId, Settings["partnerrole"].ToString());
                                    objRoles.AddUserRole(PortalId, objUser.UserID, objRole.RoleID, DateTime.MinValue, DateTime.MinValue);
                                }
                                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Employee Added", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
								grdUsers.DataSource = objPartners.GetPartnerUsers(objPartner.PartnerId);
								grdUsers.DataBind();
								grdActivity.DataSource = objPartners.GetPartnerActivity(PortalId, objPartner.PartnerId, DBNull.Value, DBNull.Value, Convert.ToDateTime(txtStart.Text), Convert.ToDateTime(txtEnd.Text), 0, 0, false);
								grdActivity.DataBind();
                            }
						}
                        else
                        {
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "The Employee Specified Is Already Associated To A Partner", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
						}
					}
                    else
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "A User Account Does Not Exist For The Email Address Specified", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
					}
				}
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "The Email Address Entered Does Not Match The Website Domain For This Partner", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
				}
			}
		}

        protected void cboOwner_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            LoadPartner();
        }

        protected void grdUsers_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "cmdDelete")
            {
				PartnerController objPartners = new PartnerController();
				var objPartner = objPartners.GetPartnerByUserId(PortalId, int.Parse(cboOwner.SelectedValue));
				if ((objPartner != null))
                {
                    UserInfo objUser = UserController.GetUserById(PortalId, int.Parse(e.CommandArgument.ToString()));
                    if (objUser != null)
                    {
                        objPartners.DeletePartnerUser(objPartner.PartnerId, objUser.UserID);
                        if (!string.IsNullOrEmpty((string)Settings["partnerrole"]))
                        {
                            RoleController objRoles = new RoleController();
                            RoleInfo objRole = objRoles.GetRoleByName(PortalId, Settings["partnerrole"].ToString());
                            RoleController.DeleteUserRole(objUser, objRole, PortalSettings, false);
                        }
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Employee Removed", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                        grdUsers.DataSource = objPartners.GetPartnerUsers(objPartner.PartnerId);
                        grdUsers.DataBind();
                        grdActivity.DataSource = objPartners.GetPartnerActivity(PortalId, objPartner.PartnerId, DBNull.Value, DBNull.Value, Convert.ToDateTime(txtStart.Text), Convert.ToDateTime(txtEnd.Text), 0, 0, false);
                        grdActivity.DataBind();
                    }
                }
			}
		}

        protected void cmdFilter_Click(System.Object sender, System.EventArgs e)
		{
			PartnerController objPartners = new PartnerController();
			var objPartner = objPartners.GetPartnerByUserId(PortalId, UserId);
			if ((objPartner != null))
            {
				grdActivity.DataSource = objPartners.GetPartnerActivity(PortalId, objPartner.PartnerId, DBNull.Value, DBNull.Value, Convert.ToDateTime(txtStart.Text), Convert.ToDateTime(txtEnd.Text), 0, 0, false);
				grdActivity.DataBind();
			}
		}

		public string DisplaySite(object Title, object URL)
		{
			return "<a href=\"" + URL.ToString() + "\" target=\"_new\">" + Title.ToString() + "</a>";
		}

	}

}
