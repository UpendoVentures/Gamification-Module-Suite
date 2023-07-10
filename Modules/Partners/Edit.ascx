<%@ Control language="C#" CodeBehind="Edit.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="HCC.Partners.Edit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="FilePickerUploader" Src="~/controls/filepickeruploader.ascx" %>

<div class="dnnForm dnnEdit dnnClear" id="dnnEdit">
    <ul class="dnnAdminTabNav dnnClear">
        <li><a href="#rbPartner">Partner</a></li>
        <li><a href="#rbServices">Services</a></li>
        <li><a href="#rbEmployees">Employees</a></li>
        <li><a href="#rbShowcase">Showcase</a></li>
        <li><a href="#rbActivity">Activity</a></li>
    </ul>
    <div class="rbPartner dnnClear" id="rbPartner">
    <fieldset>
         <div class="dnnFormItem">
            <dnn:label id="plOwner" runat="server" text="Created By:" helptext="Your User Account" controlname="cboOwner" />
            <asp:dropdownlist id="cboOwner" runat="server" CssClass="Normal" Width="300" autopostback="true" OnSelectedIndexChanged="cboOwner_SelectedIndexChanged" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plName" runat="server" text="Name:" CssClass="dnnFormRequired" helptext="Enter The Name Of Your Organization" controlname="txtName" />
            <asp:TextBox ID="txtName" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem" id="rowLogo" runat="server">
            <dnn:label id="plLogo" runat="server" text="Logo:" CssClass="dnnFormRequired" helptext="Provide A Logo For Your Organization ( Logos Will Be Resized To 200 X 200 Pixels )" controlname="ctlLogo" />
            <dnn:FilePickerUploader ID="ctlLogo" runat="server" UsePersonalFolder="True" Required="True" FileFilter="png,jpeg,jpg" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plSummary" runat="server" text="Summary:" CssClass="dnnFormRequired" helptext="Enter A Summary Description Of Your Organization" controlname="txtSummary" />
            <asp:TextBox ID="txtSummary" Runat="server" Width="300" MaxLength="500" TextMode="MultiLine" Rows="5" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plDescription" runat="server" text="Description:" CssClass="dnnFormRequired" helptext="Enter A Detailed Description Of Your Organization" controlname="txtDescription" />
            <asp:TextBox ID="txtDescription" Runat="server" Width="300" MaxLength="2000" TextMode="MultiLine" Rows="5" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plCity" runat="server" text="City:" CssClass="dnnFormRequired" helptext="Enter The City Where You Are Located" controlname="txtCity" />
            <asp:TextBox ID="txtCity" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plRegion" runat="server" text="Region:" CssClass="dnnFormRequired" helptext="Enter The Region Where You Are Located" controlname="txtRegion" />
            <asp:TextBox ID="txtRegion" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plTelephone" runat="server" text="Telephone:" CssClass="dnnFormRequired" helptext="Enter Your Telephone Number" controlname="txtTelephone" />
            <asp:TextBox ID="txtTelephone" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plURL" runat="server" text="Url:" CssClass="dnnFormRequired" helptext="Provide A Landing Page URL For Your Website" controlname="txtURL" />
            <asp:TextBox ID="txtURL" Runat="server" Width="300" MaxLength="250" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plEmail" runat="server" text="Email:" CssClass="dnnFormRequired" helptext="Enter Your Email Address" controlname="txtEmail" />
            <asp:TextBox ID="txtEmail" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plContact" runat="server" text="Contact:" CssClass="dnnFormRequired" helptext="Enter The Name Of The Primary Contact" controlname="txtContact" />
            <asp:TextBox ID="txtContact" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plActive" runat="server" text="Active?" helptext="In Order To Activate Your Partnership You Will Need To Meet The Requirements For Employees And Showcase Sites" controlname="chkActive" />
            <asp:CheckBox ID="chkActive" Runat="server" />
        </div>
    </fieldset>	
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdSave1" runat="server" text="Save" cssclass="dnnPrimaryAction" OnCommand="cmdSave_Click" /></li>
    </ul>
    </div>
    <div class="rbServices dnnClear" id="rbServices">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plServices" runat="server" text="Services:" helptext="Select All Services That Your Organization Offers" controlname="chkServices" />
            <asp:CheckBoxList ID="chkServices" runat="server" />
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdSave2" runat="server" text="Save" cssclass="dnnPrimaryAction" OnCommand="cmdSave_Click" /></li>
    </ul>
    </div>
    <div class="rbEmployees dnnClear" id="rbEmployees">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plUser" runat="server" text="Email:" helptext="Enter The Email Address Of An Employee That Works For Your Organization." controlname="txtUser" />
            <span><asp:TextBox ID="txtUser" Runat="server" Width="300" /><asp:linkbutton id="cmdAdd" runat="server" text="Add" cssclass="dnnPrimaryAction" OnCommand="cmdAdd_Click" /></span>
        </div>
        <asp:DataGrid ID="grdUsers" Runat="server" AutoGenerateColumns="False" GridLines="None" cellspacing="5" align="center" OnItemCommand="grdUsers_ItemCommand">
            <Columns>
                <asp:TemplateColumn HeaderText="">
                    <ItemTemplate>
                        <asp:Label ID="lblUser" Runat="server" Text='<%# Eval("DisplayName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="cmdDelete" Runat="server" Text="Remove" CssClass="dnnSecondaryAction" OnClientClick="return confirm('Are You Sure You Wish To Remove This Employee?');" CommandName="cmdDelete" CommandArgument='<%# Eval("UserId") %>'></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateColumn> 
            </Columns>
        </asp:DataGrid>
	    <br />
	    <p align="center">
            Employees Must Have Registered On The Site Previously And Have An Email Address That Matches Your Organization's Domain Name.
	    </p>
    </fieldset>
    </div>
    <div class="rbShowcase dnnClear" id="rbShowcase">
    <fieldset>
        <asp:DataGrid ID="grdSites" Runat="server" AutoGenerateColumns="False" GridLines="None" cellspacing="5" align="center">
            <Columns>
                <asp:TemplateColumn HeaderText="">
                    <ItemTemplate>
                        <%# DisplaySite(Eval("Title"), Eval("URL")) %>
                    </ItemTemplate>
                </asp:TemplateColumn> 
            </Columns>
        </asp:DataGrid>
	    <br />
	    <p align="center">
            <asp:Label ID="lblSites" runat="server" />
	    </p>
    </fieldset>
    </div>
    <div class="rbActivity dnnClear" id="rbActivity">
    <fieldset>
	<p align="center">
        <asp:label id="plStart" runat="server" text="Start:" />
        <asp:TextBox ID="txtStart" Runat="server" Width="150" />&nbsp;<asp:hyperlink id="cmdStart" runat="server" Text="#" />
        &nbsp;&nbsp;
        <asp:label id="plEnd" runat="server" text="End:" />
        <asp:TextBox ID="txtEnd" Runat="server" Width="150" />&nbsp;<asp:hyperlink id="cmdEnd" runat="server" Text="#" />
        &nbsp;&nbsp;
        <asp:linkbutton id="cmdFilter" runat="server" text="Filter" cssclass="dnnPrimaryAction" OnCommand="cmdFilter_Click" />
	</p>
    <asp:DataGrid ID="grdActivity" Runat="server" AutoGenerateColumns="False" GridLines="None" cellspacing="5" align="center">
        <Columns>
	 	    <asp:BoundColumn HeaderText="Name" HeaderStyle-Font-Bold="True" DataField="DisplayName" />
	 	    <asp:BoundColumn HeaderText="Activity" HeaderStyle-Font-Bold="True" DataField="ActivityName" />
	 	    <asp:BoundColumn HeaderText="Score" HeaderStyle-Font-Bold="True" DataField="Score" />
        </Columns>
    </asp:DataGrid>
    <br />
	<p align="center">
        <asp:Label ID="lblActivity" runat="server" />
	</p>
    </fieldset>
    </div>
</div>
<script type="text/javascript">
	(function ($, Sys) {
		var setUpManagement = function () {
			$('#dnnEdit').dnnTabs().dnnPanels();
		}

		$(document).ready(function () {
			setUpManagement();
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
				setUpManagement();
			});

		});
	} (jQuery, window.Sys));
	
</script>
