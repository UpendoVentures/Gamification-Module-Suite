<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.Partners.DetailSettings" CodeBehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnSettings dnnClear" id="dnnSettings">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plTemplate" runat="server" text="Template" helptext="An HTML Template for Displaying The Listing For A Partner. The Template Can Specify Tokens For All Partner Fields ( [NAME], [LOGO], [SUMMARY], [DESCRIPTION], [CITY], [REGION], [TELEPHONE], [WEBSITE], [EMAIL], [EMPLOYEES] )." controlname="txtTemplate" />
            <asp:textbox id="txtTemplate" runat="server" maxlength="2000" textmode="multiline" rows="5" />
        </div>
   </fieldset>
</div>


