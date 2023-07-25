<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.Partners.View" CodeBehind="View.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<p align="center">
    <dnn:ActionLink id="cmdManage" runat="server" Title="Manage Your Partnership" ControlKey="Edit" Security="View" />
    <asp:Label id="lblLogin" runat="server" text="<br />Please Login To Manage Your Partnership" />
</p>
<p align="center">
    <asp:Label id="lblMessage" runat="server" />
</p>
<p align="center">
    Service: <asp:DropDownList ID="cboService" runat="server" DataTextField="ServiceName" DataValueField="ServiceId" AutoPostBack="true" OnSelectedIndexChanged="cboService_SelectedIndexChanged" />
    <asp:Literal ID="litPartners" runat="server" />
</p>
<p id="ctlPaging" runat="server" align="center">
    <asp:Hyperlink ID="cmdPrevious" Runat="server" CssClass="dnnPrimaryAction">Previous</asp:Hyperlink>
    Page <asp:Label ID="lblPage" Runat="server" /> of <asp:Label ID="lblPages" Runat="server" />
    <asp:Hyperlink ID="cmdNext" Runat="server" CssClass="dnnPrimaryAction">Next</asp:Hyperlink>
</p>





