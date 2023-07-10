<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.CommunityMetrics.Settings" CodeBehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnSettings dnnClear" id="dnnSettings">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="lblFilter" runat="server" text="Allow Filtering?" helptext="Do You Wish To Allow People To Filter The Display Results?" controlname="chkFilter" />
            <asp:checkbox id="chkFilter" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="lblRange" runat="server" text="Date Range" helptext="Specify If The Date Range Is Dynamic ( Based On The Current Date ) Or Static ( A Predefined Date Range )" controlname="optRange" />
            <asp:radiobuttonlist ID="optRange" runat="server" CssClass="NormalTextBox" AutoPostBack="true" OnSelectedIndexChanged="optRange_SelectedIndexChanged" RepeatDirection="Horizontal">
                <asp:ListItem Value="D">Dynamic</asp:ListItem>
                <asp:ListItem Value="S">Static</asp:ListItem>
            </asp:radiobuttonlist>
        </div>
        <div id="rowDays" runat="server" visible="true" class="dnnFormItem">
            <dnn:label id="plDays" runat="server" text="Default Days" helptext="Default Number Of Days" controlname="cboDays" />
            <asp:DropDownList ID="cboDays" runat="server" CssClass="NormalTextBox">
                <asp:ListItem Value="D">Past Day</asp:ListItem>
                <asp:ListItem Value="W">Past Week</asp:ListItem>
                <asp:ListItem Value="M">Past Month</asp:ListItem>
                <asp:ListItem Value="Q">Past Quarter</asp:ListItem>
                <asp:ListItem Value="Y">Past Year</asp:ListItem>
                <asp:ListItem Value="*">All Time</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div id="rowStart" runat="server" visible="false" class="dnnFormItem">
            <dnn:label id="plStart" runat="server" text="Start Date" helptext="Start Date" controlname="txtStart" />
            <asp:textbox id="txtStart" runat="server" />
        </div>
        <div id="rowEnd" runat="server" visible="false" class="dnnFormItem">
            <dnn:label id="plEnd" runat="server" text="End Date" helptext="End Date" controlname="txtEnd" />
            <asp:textbox id="txtEnd" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plActivity" runat="server" text="Default Activity" helptext="Default Activity" controlname="cboActivity" />
            <asp:DropDownList ID="cboActivity" runat="server" CssClass="NormalTextBox" DataValueField="ActivityId" DataTextField="ActivityName" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plRows" runat="server" text="Default Rows" helptext="Default Number of Rows" controlname="cboRows" />
            <asp:DropDownList ID="cboRows" runat="server" CssClass="NormalTextBox">
                <asp:ListItem Value="10">Top 10</asp:ListItem>
                <asp:ListItem Value="20">Top 20</asp:ListItem>
                <asp:ListItem Value="30">Top 30</asp:ListItem>
                <asp:ListItem Value="40">Top 40</asp:ListItem>
                <asp:ListItem Value="50">Top 50</asp:ListItem>
                <asp:ListItem Value="100">Top 100</asp:ListItem>
            </asp:DropDownList>
        </div>
   </fieldset>
</div>


