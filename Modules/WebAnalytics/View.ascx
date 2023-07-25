<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.WebAnalytics.View" CodeBehind="View.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ register assembly="Telerik.Web.UI" namespace="Telerik.Charting" tagprefix="telerik" %>

<div class="dnnForm dnnWebAnalytics dnnClear" id="dnnWebAnalytics">
	<p align="center">
        <asp:label id="plStartDate" runat="server" text="Start Date:" />
        <asp:textbox id="txtStartDate" runat="server" />&nbsp;<asp:hyperlink id="cmdStartDate" runat="server" Text="#" />
        &nbsp;&nbsp;
        <asp:label id="plEndDate" runat="server" text="End Date:" />
        <asp:textbox id="txtEndDate" runat="server" />&nbsp;<asp:hyperlink id="cmdEndDate" runat="server" Text="#" />
        &nbsp;&nbsp;
        <asp:linkbutton id="cmdDisplay1" text="Display" runat="server" cssclass="dnnPrimaryAction" OnClick="cmdDisplay_Click" />
    </p>
 	<p align="center">
        <asp:Label id="lblTotals" runat="server" />
        <telerik:RadHtmlChart runat="server" ID="radGraph" Transitions="true" EnableViewState="false" Width="1000" />
    </p>
 	<p align="center">
        <asp:dropdownlist id="cboReport" runat="server" />&nbsp;        
        <asp:dropdownlist id="cboRows" runat="server" />&nbsp;
        <asp:dropdownlist id="cboType" runat="server">
            <asp:ListItem Value="C" Text="Chart" />
            <asp:ListItem Value="R" Text="Report" />
        </asp:dropdownlist>&nbsp;
        <asp:linkbutton id="cmdDisplay2" text="Display" runat="server" cssclass="dnnPrimaryAction" OnClick="cmdDisplay_Click" />
    </p>
 	<p align="center">
        <asp:datagrid id="grdReport" Width="98%" AutoGenerateColumns="true" EnableViewState="false" runat="server" BorderStyle="None" GridLines="None" CssClass="dnnGrid" visible="false">
            <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
            <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
	        <alternatingitemstyle cssclass="dnnGridAltItem" />
	        <edititemstyle cssclass="dnnFormInput" />
	        <selecteditemstyle cssclass="dnnFormError" />
	        <footerstyle cssclass="dnnGridFooter" />
	        <pagerstyle cssclass="dnnGridPager" />
        </asp:datagrid>
        <telerik:RadHtmlChart runat="server" ID="pieReport" Transitions="true" EnableViewState="false" visible="false" />
    </p>    
	<h2 id="dnnPanel-TabsAppearance" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded">Settings</a></h2>
    <fieldset class="dnnhsOtherSettings">               
        <div class="dnnFormItem">
            <dnn:label id="plLoggingMethod" runat="server" text="Logging Method" helptext="Most Sites Should Use The Batch Method For The Best Performance" controlname="optLoggingMethod" />
            <asp:RadioButtonList ID="optLoggingMethod" runat="server" RepeatDirection="Horizontal" >
                <asp:ListItem value="B" selected="true">Batch</asp:ListItem>
                <asp:ListItem value="D">Direct</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plRetentionHistory" runat="server" text="Retention History" helptext="Specify The Number Of Days Of History You Would Like To Retain" controlname="txtRetentionHistory" />
            <asp:textbox id="txtRetentionHistory" runat="server" width="100" />&nbsp;Days
        </div> 
	</fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton ID="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" Text="Update" OnClick="cmdUpdate_Click" /></li>
    </ul>
</div>
