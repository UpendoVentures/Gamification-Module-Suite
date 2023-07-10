<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.Partners.Settings" CodeBehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnSettings dnnClear" id="dnnSettings">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plDays" runat="server" text="Activity Days" helptext="Number of Days Of Activity" controlname="txtDays" />
            <asp:textbox id="txtDays" runat="server" maxlength="4" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plRows" runat="server" text="Rows" helptext="Number Of Display Rows" controlname="txtRows" />
            <asp:textbox id="txtRows" runat="server" maxlength="3" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plColumns" runat="server" text="Columns" helptext="Number Of Display Columns" controlname="txtColumns" />
            <asp:textbox id="txtColumns" runat="server" maxlength="1" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plServices" runat="server" text="Services" helptext="Comma Delimited List Of Services" controlname="txtServices" />
            <asp:textbox id="txtServices" runat="server" maxlength="500" textmode="multiline" rows="2" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plTemplate" runat="server" text="Template" helptext="An HTML Template for Displaying The Listing For A Partner. The Template Can Specify Tokens For All Partner Fields ( [NAME], [LOGO], [SUMMARY], [DESCRIPTION], [CITY], [REGION], [TELEPHONE], [WEBSITE], [EMAIL], [EMPLOYEES] )." controlname="txtTemplate" />
            <asp:textbox id="txtTemplate" runat="server" maxlength="2000" textmode="multiline" rows="5" />
        </div>
        <hr />
        <div class="dnnFormItem">
            <dnn:label id="plLogoWidth" runat="server" text="Logo Width" helptext="Logo Width" controlname="txtLogoWidth" />
            <asp:textbox id="txtLogoWidth" runat="server" maxlength="3" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plLogoHeight" runat="server" text="Logo Height" helptext="Logo Height" controlname="txtLogoHeight" />
            <asp:textbox id="txtLogoHeight" runat="server" maxlength="3" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plEmployees" runat="server" text="Employees" helptext="Minimum Number Of Employees To Be Considered An Active Partner" controlname="txtEmployees" />
            <asp:textbox id="txtEmployees" runat="server" maxlength="2" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plSites" runat="server" text="Sites" helptext="Minimum Number Of Showcase Sites To Be Considered An Active Partner" controlname="txtSites" />
            <asp:textbox id="txtSites" runat="server" maxlength="2" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plShowcaseModule" runat="server" text="Showcase Module" helptext="Module Where Showcase Sites Are Managed" controlname="cboShowcaseModule" />
            <asp:DropDownList ID="cboShowcaseModule" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plPartnerRole" runat="server" text="Partner Role" helptext="Security Role For All Partner Employees" controlname="txtPartnerRole" />
            <asp:textbox id="txtPartnerRole" runat="server" maxlength="50" />
        </div>
        <hr />
        <div class="dnnFormItem">
           <asp:DataGrid ID="grdPartners" Runat="server" AutoGenerateColumns="False" GridLines="None" cellspacing="5" align="center">
                <Columns>
	 	            <asp:BoundColumn HeaderText="Name" HeaderStyle-Font-Bold="True" DataField="PartnerName" />
	 	            <asp:BoundColumn HeaderText="City" HeaderStyle-Font-Bold="True" DataField="City" />
	 	            <asp:BoundColumn HeaderText="Region" HeaderStyle-Font-Bold="True" DataField="Region" />
                    <asp:TemplateColumn HeaderText="Email" HeaderStyle-Font-Bold="True">
                        <ItemTemplate>
                            <%# DisplayEmail(Eval("Email"), Eval("Contact"), Eval("PartnerName"), Eval("Summary")) %>
                        </ItemTemplate>
                    </asp:TemplateColumn>
	 	            <asp:BoundColumn HeaderText="Contact" HeaderStyle-Font-Bold="True" DataField="Contact" />
	 	            <asp:BoundColumn HeaderText="Score" HeaderStyle-Font-Bold="True" DataField="Score" />
                </Columns>
            </asp:DataGrid>
        </div>
    </fieldset>
</div>


