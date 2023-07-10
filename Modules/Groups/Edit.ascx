<%@ Control language="C#" CodeBehind="Edit.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="HCC.Groups.Edit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnEdit dnnClear" id="dnnEdit">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plName" runat="server" text="Group Name:" helptext="Enter The Name Of Your Group" controlname="txtName" />
            <asp:TextBox ID="txtName" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plURL" runat="server" text="URL:" helptext="Enter Your URL" controlname="txtURL" />
            <asp:TextBox ID="txtURL" Runat="server" Width="300" MaxLength="250" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plCity" runat="server" text="City:" helptext="Enter The City Where You Are Located" controlname="txtCity" />
            <asp:TextBox ID="txtCity" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plRegion" runat="server" text="Region:" helptext="Enter The Region Where You Are Located ( ie. Florida, USA )" controlname="txtRegion" />
            <asp:TextBox ID="txtRegion" Runat="server" Width="300" MaxLength="50" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plActive" runat="server" text="Active?" helptext="Indicate If Your Group Is Active" controlname="chkActive" />
            <asp:CheckBox ID="chkActive" Runat="server" Checked="true" />
        </div>
    </fieldset>	
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdSave" runat="server" text="Save" cssclass="dnnPrimaryAction" OnCommand="cmdSave_Click" /></li>
        <li><asp:linkbutton id="cmdCancel" runat="server" text="Cancel" cssclass="dnnSecondaryAction" OnCommand="cmdCancel_Click" /></li>
    </ul>
</div>
