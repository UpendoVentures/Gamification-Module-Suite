<%@ Control language="C#" CodeBehind="Edit.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="HCC.Personalization.Edit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnEdit dnnClear" id="dnnEdit">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plModule" runat="server" text="Module:" helptext="Select A Module From The Current Page" controlname="cboModule" />
            <asp:DropDownList ID="cboModule" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboModule_SelectedIndexChanged" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plRules" runat="server" text="" />
            <asp:DataGrid ID="grdRules" Runat="server" AutoGenerateColumns="False" GridLines="None" cellspacing="5" OnItemCommand="grdRules_ItemCommand">
                <Columns>
                    <asp:TemplateColumn HeaderText="Attribute" HeaderStyle-Font-Bold="true" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="lblAttribute" Runat="server" Text='<%# GetAttributeName(Eval("Attribute")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Condition" HeaderStyle-Font-Bold="true" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="lblCondition" Runat="server" Text='<%# GetConditionName(Eval("Condition")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Value" HeaderStyle-Font-Bold="true" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="lblValue" Runat="server" Text='<%# Eval("Value") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="">
                        <ItemTemplate>
                            <asp:LinkButton ID="cmdDelete" Runat="server" Text="Remove" CssClass="dnnSecondaryAction" OnClientClick="return confirm('Are You Sure You Wish To Remove This Rule?');" CommandName="cmdDelete" CommandArgument='<%# Eval("Attribute") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn> 
                </Columns>
            </asp:DataGrid>
        </div>
    </fieldset>
    <hr />
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plRule" runat="server" text="Display When:" helptext="Specify Your Personalization Rule" controlname="cboAttribute" />
            <asp:DropDownList ID="cboAttribute" runat="server">
                <asp:ListItem Value="">&lt;Select Attribute&gt;</asp:ListItem>
                <asp:ListItem Value="DEVICE:HardwareVendor">Hardware Vendor</asp:ListItem>
                <asp:ListItem Value="DEVICE:HardwareModel">Hardware Model</asp:ListItem>
                <asp:ListItem Value="DEVICE:PlatformVendor">Platform Vendor</asp:ListItem>
                <asp:ListItem Value="DEVICE:PlatformName">Platform Name</asp:ListItem>
                <asp:ListItem Value="DEVICE:PlatformVersion">Platform Version</asp:ListItem>
                <asp:ListItem Value="DEVICE:BrowserVendor">Browser Vendor</asp:ListItem>
                <asp:ListItem Value="DEVICE:BrowserName">Browser Name</asp:ListItem>
                <asp:ListItem Value="DEVICE:BrowserVersion">Browser Version</asp:ListItem>
                <asp:ListItem Value="DEVICE:IsMobile">Is Mobile Device?</asp:ListItem>
                <asp:ListItem Value="USERAGENT">Device User Agent</asp:ListItem>
                <asp:ListItem Value="LANGUAGE">Language</asp:ListItem>
                <asp:ListItem Value="LOCATION:COUNTRY">Country</asp:ListItem>
                <asp:ListItem Value="LOCATION:REGION">Region</asp:ListItem>
                <asp:ListItem Value="LOCATION:CITY">City</asp:ListItem>
                <asp:ListItem Value="IP">IP Address</asp:ListItem>
                <asp:ListItem Value="CAMPAIGN">Campaign</asp:ListItem>
                <asp:ListItem Value="PAGE">Page</asp:ListItem>
                <asp:ListItem Value="REFERRER">Referrer</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plCondition" runat="server" text="" helptext="" controlname="cboCondition" />
            <asp:DropDownList ID="cboCondition" runat="server">
                <asp:ListItem Value="">&lt;Select Condition&gt;</asp:ListItem>
                <asp:ListItem Value="=">Equals</asp:ListItem>
                <asp:ListItem Value="!=">Does Not Equal</asp:ListItem>
                <asp:ListItem Value="<">Is Less Than</asp:ListItem>
                <asp:ListItem Value=">">Is Greater Than</asp:ListItem>
                <asp:ListItem Value="~">Contains</asp:ListItem>
                <asp:ListItem Value="!~">Does Not Contain</asp:ListItem>
                <asp:ListItem Value="[]">Is In</asp:ListItem>
                <asp:ListItem Value="[!]">Is Not In</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plValue" runat="server" text="" helptext="" controlname="txtValue" />
            <asp:TextBox ID="txtValue" runat="server" />
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdAdd" runat="server" text="Add Rule" cssclass="dnnPrimaryAction" OnCommand="cmdAdd_Click" /></li>
        <li><asp:linkbutton id="cmdCancel" runat="server" text="Cancel" cssclass="dnnSecondaryAction" OnCommand="cmdCancel_Click" /></li>
    </ul>
</div>
