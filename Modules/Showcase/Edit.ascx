<%@ Control language="C#" CodeBehind="Edit.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="HCC.Showcase.Edit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<br />
<div class="dnnForm dnnEdit dnnClear" id="dnnEdit">
    <fieldset>
         <div class="dnnFormItem">
            <dnn:label id="plOwner" runat="server" text="Created By:" helptext="Your User Account" controlname="cboOwner" />
            <asp:dropdownlist id="cboOwner" runat="server" CssClass="Normal" autopostback="true" OnSelectedIndexChanged="cboOwner_SelectedIndexChanged" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plSite" runat="server" text="Item:" helptext="Select An Existing Item From The List Or Choose To Create A New Item" controlname="cboSite" />
            <asp:dropdownlist id="cboSite" runat="server" CssClass="Normal" autopostback="true" OnSelectedIndexChanged="cboSite_SelectedIndexChanged" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plURL" runat="server" text="URL:" CssClass="dnnFormRequired" helptext="Enter The URL" controlname="txtURL" />
            <asp:textbox id="txtURL" runat="server" maxlength="250" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plTitle" runat="server" text="Title:" CssClass="dnnFormRequired" helptext="Enter The Title" controlname="txtTitle" />
            <asp:textbox id="txtTitle" runat="server" maxlength="100" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plDescription" runat="server" text="Description:" CssClass="dnnFormRequired" helptext="Enter The Description" controlname="txtDescription" />
            <asp:textbox id="txtDescription" runat="server" maxlength="500" textmode="multiline" rows="5" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plCategories" runat="server" text="Categories:" helptext="Select The Categories" controlname="chkCategories" />
            <asp:CheckBoxList ID="chkCategories" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plActive" runat="server" text="Active?" helptext="Specify If Your Item Is Active" controlname="chkActive" />
            <asp:checkbox id="chkActive" runat="server" checked="true" />
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdSave" text="Save" runat="server" cssclass="dnnPrimaryAction" OnCommand="cmdSave_Click" /></li>
        <li><asp:linkbutton id="cmdCancel" text="Cancel" runat="server" cssclass="dnnSecondaryAction" OnCommand="cmdCancel_Click" /></li>
    </ul>
</div>
<div class="dnnForm dnnEdit dnnClear" id="dnnThumbnail">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plThumbnail" runat="server" text="Thumbnail:" controlname="cboThumbnail" />
            <asp:dropdownlist id="cboThumbnail" runat="server" autopostback="true" OnSelectedIndexChanged="cboThumbnail_SelectedIndexChanged" />
        </div>
   </fieldset>
</div>
<p align="center">
    <asp:Image id="imgThumbnail" runat="server" />
</p>

