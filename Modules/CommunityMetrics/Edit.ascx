<%@ Control language="C#" CodeBehind="Edit.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="HCC.CommunityMetrics.Edit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ register assembly="Telerik.Web.UI" namespace="Telerik.Charting" tagprefix="telerik" %>

<div class="dnnForm dnnEdit dnnClear" id="dnnEdit">
    <ul class="dnnAdminTabNav dnnClear">
        <li><a href="#rbActivities">Activities</a></li>
        <li><a href="#rbUserActivity">User Activity</a></li>
    </ul>
    <div class="rbActivities dnnClear" id="rbActivities">
        <fieldset>
            <div class="dnnFormItem">
	            <dnn:label id="plActivity" runat="server" controlname="cboActivity" text="Activity" helptext="Select An Activity From The List" suffix=":" />
                <asp:DropDownList ID="cboActivity" runat="server" CssClass="NormalTextBox" Width="300" DataValueField="ActivityId" DataTextField="ActivityName" AutoPostBack="true" OnSelectedIndexChanged="cboActivity_SelectedIndexChanged" />
            </div>
            <div class="dnnFormItem">
	        <dnn:label id="plName" runat="server" controlname="txtName" text="Name" helptext="Enter The Name Of The Activity" suffix=":" />
                <asp:TextBox ID="txtName" Runat="server" CssClass="NormalTextBox" MaxLength="50" Width="300" />
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plDescription" runat="server" controlname="txtDescription" text="Description" helptext="Enter The Description Of The Activity" suffix=":" />
                <asp:TextBox ID="txtDescription" Runat="server" CssClass="NormalTextBox" Width="300" TextMode="MultiLine" Rows="3" />
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plTypeName" runat="server" controlname="cboTypeName" text="Activity Type" helptext="Select The Activity Type ( Activity Types Are .NET Classes That Implement IActivity )" suffix=":" />
                <asp:DropDownList ID="cboTypeName" Runat="server" CssClass="NormalTextBox" Width="300" AutoPostBack="true" OnSelectedIndexChanged="cboTypeName_SelectedIndexChanged" />
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plLastExecution" runat="server" controlname="txtLastExecution" text="Last Execution" helptext="The Last Execution Date When Metrics Were Collected For This Activity" suffix=":" />
                <asp:TextBox ID="txtLastExecution" Runat="server" CssClass="NormalTextBox" MaxLength="200" Width="300" />
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plFactor" runat="server" controlname="txtFactor" text="Factor" helptext="The Multiplication Factor For This Activity Which Will Be Used When Calculating A User Score" suffix=":" />
	            <asp:TextBox ID="txtFactor" Runat="server" CssClass="NormalTextBox" MaxLength="200" Width="300" />
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plMinDaily" runat="server" controlname="txtMinDaily" text="Min Daily" helptext="Minimum Number Of Daily Points Required For A User To Receive Credit For This Activity" suffix=":" />
	            <asp:TextBox ID="txtMinDaily" Runat="server" CssClass="NormalTextBox" MaxLength="200" Width="300" />
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plMaxDaily" runat="server" controlname="txtMaxDaily" text="Max Daily" helptext="Maximum Number Of Daily Points A User Can Receive For This Activity" suffix=":" />
	            <asp:TextBox ID="txtMaxDaily" Runat="server" CssClass="NormalTextBox" MaxLength="200" Width="300" />
            </div>
            <div class="dnnFormItem">
	        <dnn:label id="plMetricType" runat="server" controlname="optMetricType" text="Metric Type" helptext="The Type Of Metric Being Collected - Either Daily Totals, Cumulative Totals, or One-Time Events" suffix=":" />
                <asp:RadioButtonList ID="optMetricType" CssClass="dnnFormRadioButtons" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="0">Daily</asp:ListItem>
                    <asp:ListItem Value="1">Cumulative</asp:ListItem>
                    <asp:ListItem Value="2">Once</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plUserFilter" runat="server" controlname="txtUserFilter" text="User Filter" helptext="A Comma Delimited List Of UserIDs Which Should Be Excluded For This Activity" suffix=":" />
	            <asp:TextBox ID="txtUserFilter" Runat="server" CssClass="NormalTextBox" MaxLength="500" Width="300" TextMode="MultiLine" Rows="2" />
            </div>
            <div class="dnnFormItem">
	            <dnn:label id="plIsActive" runat="server" controlname="chkIsActive" text="Active" helptext="Specify If This Activity Is Enabled" suffix="?" />
	            <asp:CheckBox ID="chkIsActive" runat="server" CssClass="Normal"></asp:CheckBox>
            </div>
	        <hr />
            <asp:repeater id="rptSettings" runat="server" OnItemDataBound="rptSettings_ItemDataBound">
                <ItemTemplate>
        	        <div class="dnnFormItem">
                        <div class="dnnLabel">    
                            <label ID="label" runat="server"><asp:Label ID="lblSetting" runat="server" />:</label>
                            <asp:LinkButton ID="cmdHelp" TabIndex="-1" runat="server" CausesValidation="False" CssClass="dnnFormHelp"></asp:LinkButton>
                            <asp:Panel ID="pnlHelp" runat="server" CssClass="dnnTooltip">
                                <div class="dnnFormHelpContent dnnClear">
                                    <asp:Label ID="lblHelp" runat="server" class="dnnHelpText" />
                                    <a href="#" class="pinHelp"></a>
                                </div>   
                            </asp:Panel>
                        </div>
		                <asp:TextBox ID="txtSetting" Runat="server" CssClass="NormalTextBox" Width="300" TextMode="MultiLine" Rows="2" />
        	        </div>
                </ItemTemplate>
	        </asp:repeater>
        </fieldset>
        <ul class="dnnActions dnnClear">
            <li><asp:linkbutton id="cmdUpdate" runat="server" text="Save" cssclass="dnnPrimaryAction" OnCommand="cmdUpdate_Click" /></li>
            <li><asp:linkbutton id="cmdCancel" runat="server" text="Cancel" cssclass="dnnSecondaryAction" OnCommand="cmdCancel_Click" /></li>
            <li><asp:linkbutton id="cmdCopy" runat="server" text="Copy" cssclass="dnnSecondaryAction" OnCommand="cmdCopy_Click" /></li>
            <li><asp:linkbutton id="cmdTest" runat="server" text="Test" cssclass="dnnSecondaryAction" OnCommand="cmdTest_Click" /></li>
        </ul>
        <br /><hr /><br />
        <p align="center">
            <asp:label id="plStartDate" runat="server" text="Start Date:" />
            <asp:textbox id="txtStartDate" runat="server" />&nbsp;<asp:hyperlink id="cmdStartDate" runat="server" Text="#" />
            &nbsp;&nbsp;
            <asp:label id="plEndDate" runat="server" text="End Date:" />
            <asp:textbox id="txtEndDate" runat="server" />&nbsp;<asp:hyperlink id="cmdEndDate" runat="server" Text="#" />
        </p>
        <p align="center">
            <telerik:RadHtmlChart runat="server" ID="radGraph" Transitions="true" EnableViewState="false" Width="800" />
            <asp:Label ID="lblGraph" runat="server" Text="No Activity Between Dates Specified" Visible="false" />
        </p>
    </div>
    <div class="rbUserActivity dnnClear" id="rbUserActivity">
        <fieldset>
            <div class="dnnFormItem">
                <dnn:label id="plDate" runat="server" text="Date:" CssClass="dnnFormRequired" helptext="Enter The Date" controlname="txtDate" />
                <asp:TextBox ID="txtDate" Runat="server" Width="300" MaxLength="50" />&nbsp;<asp:hyperlink id="cmdDate" runat="server" Text="#" />
            </div>
            <div class="dnnFormItem">
                <dnn:label id="plActivities" runat="server" text="Activity:" CssClass="dnnFormRequired" helptext="Select The Activity" controlname="cboActivities" />
                <asp:DropDownList ID="cboActivities" runat="server" CssClass="NormalTextBox" Width="300" DataValueField="ActivityId" DataTextField="ActivityName" AutoPostBack="true" OnSelectedIndexChanged="cboActivities_SelectedIndexChanged" />
            </div>
            <div class="dnnFormItem">
                <dnn:label id="plUser" runat="server" text="User:" CssClass="dnnFormRequired" helptext="Select The User Or Enter Their Username" controlname="cboUser" />
                <asp:DropDownList ID="cboUser" runat="server" CssClass="NormalTextBox" Width="300" DataValueField="UserId" DataTextField="DisplayName" AutoPostBack="true" OnSelectedIndexChanged="cboUser_SelectedIndexChanged" />
                <asp:TextBox ID="txtUser" Runat="server" Width="300" MaxLength="50" Visible="false" />
            </div>
            <div class="dnnFormItem">
                <dnn:label id="plCount" runat="server" text="Count:" CssClass="dnnFormRequired" helptext="Enter The Count" controlname="txtCount" />
                <asp:TextBox ID="txtCount" Runat="server" Width="300" MaxLength="50" />
            </div>
            <div class="dnnFormItem">
                <dnn:label id="plNotes" runat="server" text="Notes:" helptext="Optionally Enter Some Notes" controlname="txtNotes" />
                <asp:TextBox ID="txtNotes" Runat="server" Width="300" MaxLength="255" TextMode="MultiLine" Rows="3" />
            </div>
        </fieldset>
        <ul class="dnnActions dnnClear">
            <li><asp:linkbutton id="cmdSave" runat="server" text="Save" cssclass="dnnPrimaryAction" OnCommand="cmdSave_Click" /></li>
        </ul>
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