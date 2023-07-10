<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.VisitorsOnline.Settings" CodeBehind="Settings.ascx.cs" %>
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
            <dnn:label id="plOnlineTime" runat="server" text="Online Time" helptext="Window Of Time Where A Visitor Is Considered To Be Online" controlname="txtOnlineTime" />
            <asp:textbox id="txtOnlineTime" runat="server" maxlength="50" /> Minutes
        </div>
   </fieldset>
</div>


