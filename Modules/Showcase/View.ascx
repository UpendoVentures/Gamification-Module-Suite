<%@ Control language="C#" CodeBehind="View.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="HCC.Showcase.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<p align="center">
    <dnn:ActionLink id="cmdAdd" runat="server" Title="Manage Your Showcase" ControlKey="Edit" Security="View" />
    <asp:Label id="lblLogin" runat="server" text="<br />You Must Be Logged In To Submit Or Manage Your Showcase" />
</p>
<p align="center">
    Category: <asp:DropDownList ID="cboCategory" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboCategory_SelectedIndexChanged" />
    <asp:Literal ID="litSites" runat="server" />
</p>
<p id="ctlPaging" runat="server" align="center">
    <asp:Hyperlink ID="cmdPrevious" Runat="server" CssClass="dnnPrimaryAction">Previous</asp:Hyperlink>
    Page <asp:Label ID="lblPage" Runat="server" /> of <asp:Label ID="lblPages" Runat="server" />
    <asp:Hyperlink ID="cmdNext" Runat="server" CssClass="dnnPrimaryAction">Next</asp:Hyperlink>
</p>
