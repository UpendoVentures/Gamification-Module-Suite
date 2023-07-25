<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.Groups.Settings" CodeBehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnSettings dnnClear" id="dnnSettings">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plGoogle" runat="server" text="Google API Key" helptext="Your Google API Key" controlname="txtGoogle" />
            <asp:textbox id="txtGoogle" runat="server" maxlength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plHeight" runat="server" text="Map Height" helptext="Height In Pixels" controlname="txtHeight" />
            <asp:textbox id="txtHeight" runat="server" maxlength="50" />
        </div>
        <hr />
        <div class="dnnFormItem">
            <dnn:label id="plInstructions" runat="server" text="Instructions" helptext="Any Special Instructions For Creating Groups ( Optional )" controlname="txtInstructions" />
            <asp:textbox id="txtInstructions" runat="server" maxlength="250" TextMode="MultiLine" Rows="2" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plFormat" runat="server" text="URL Format" helptext="Required URL Format ( Optional )" controlname="txtFormat" />
            <asp:textbox id="txtFormat" runat="server" maxlength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plValidation" runat="server" text="URL Validation" helptext="Expression That Must Exist On The Page For The URL ( Optional )" controlname="txtValidation" />
            <asp:textbox id="txtValidation" runat="server" maxlength="50" />
        </div>
   </fieldset>
</div>


