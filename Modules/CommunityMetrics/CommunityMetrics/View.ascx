<%@ Control language="C#" CodeBehind="View.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="HCC.CommunityMetrics.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<div class="dnnForm CommunityMetrics dnnClear" id="CommunityMetrics">
<p align="center">
    <dnn:ActionLink id="cmdManage" runat="server" Title="Manage Activities" ControlKey="Edit" cssclass="dnnPrimaryAction" />
</p>
<p id="ctlFilter" runat="server" align="center">
    <asp:DropDownList ID="cboDays" runat="server" CssClass="NormalTextBox" AutoPostBack="true" OnSelectedIndexChanged="cboDays_SelectedIndexChanged">
        <asp:ListItem Value="D">Past Day</asp:ListItem>
        <asp:ListItem Value="W">Past Week</asp:ListItem>
        <asp:ListItem Value="M">Past Month</asp:ListItem>
        <asp:ListItem Value="Q">Past Quarter</asp:ListItem>
        <asp:ListItem Value="Y">Past Year</asp:ListItem>
        <asp:ListItem Value="*">All Time</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="cboActivity" runat="server" CssClass="NormalTextBox" AutoPostBack="true" OnSelectedIndexChanged="cboActivity_SelectedIndexChanged" />
    <asp:DropDownList ID="cboRows" runat="server" CssClass="NormalTextBox" AutoPostBack="true" OnSelectedIndexChanged="cboRows_SelectedIndexChanged">
        <asp:ListItem Value="10">Top 10</asp:ListItem>
        <asp:ListItem Value="20">Top 20</asp:ListItem>
        <asp:ListItem Value="30">Top 30</asp:ListItem>
        <asp:ListItem Value="40">Top 40</asp:ListItem>
        <asp:ListItem Value="50">Top 50</asp:ListItem>
        <asp:ListItem Value="100">Top 100</asp:ListItem>
    </asp:DropDownList>
    <asp:label id="lblFilter" runat="server" />
</p>
<asp:datagrid id="grdUserActivity" runat="server" AutoGenerateColumns="False" GridLines="None" cellspacing="5" cellpadding="5" OnItemDataBound="grdUserActivity_ItemDataBound"  OnItemCommand="grdUserActivity_ItemCommand">
    <Columns>
        <asp:TemplateColumn HeaderText="" HeaderStyle-Font-Bold="True">
            <ItemTemplate>
                <asp:Label ID="lblRank" Runat="server" Text='<%# DisplayRank() %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="" HeaderStyle-Font-Bold="True">
            <ItemTemplate>
                <asp:Label ID="lblPhoto" Runat="server" Text='<%# DisplayPhoto(Eval("UserId"), Eval("DisplayName")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Name" HeaderStyle-Font-Bold="True">
            <ItemTemplate>
                <asp:Label ID="lblUser" Runat="server" Text='<%# DisplayUser(Eval("UserId"), Eval("DisplayName")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Joined" HeaderStyle-Font-Bold="True" HeaderStyle-CssClass="joined" ItemStyle-CssClass="joined">
            <ItemTemplate>
                <asp:Label ID="lblDate" Runat="server" Text='<%# DisplayDate(Eval("CreatedOnDate")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
	    <asp:BoundColumn HeaderText="Activity" HeaderStyle-Font-Bold="True" DataField="ActivityName" />
	    <asp:BoundColumn HeaderText="Score" HeaderStyle-Font-Bold="True" DataField="Score" HeaderStyle-CssClass="score" ItemStyle-CssClass="score" />
        <asp:TemplateColumn HeaderText="" HeaderStyle-Font-Bold="True">
            <ItemTemplate>
                <asp:LinkButton ID="cmdDetails" runat="server" Text="Details" CommandName="Details" CommandArgument='<%# Eval("UserId") %>' />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:datagrid>
<p align="center">
    <br /><asp:LinkButton ID="cmdBack" runat="server" Text="Return To Summary View" CssClass="dnnPrimaryAction" OnCommand="cmdBack_Click" />
    <br /><asp:Label id="lblActivities" runat="server" />
</p>
</div>
