<%@ Page Title="Edit Grant" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditGrant.aspx.cs" Inherits="Portal11.Admin.EditGrant" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %></h2>
        <hr />
        <p class="text-danger">
            <asp:Literal runat="server" ID="litDangerMessage" />
        </p>
        <p class="text-success">
            <asp:Literal runat="server" ID="litSuccessMessage" />
        </p>
    <div class="form-horizontal">

        <asp:Panel ID="pnlName" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Name</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
                            CssClass="text-danger" ErrorMessage="Please supply a Grant Name." />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlInactive" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-10 control-label">Grant is Inactive</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:CheckBox ID="chkInact" runat="server" CssClass="checkbox col-offset-xs-1 col-xs-1"
                            ToolTip="Turn the Grant off, disabling its use. Note: This action is difficult to undo!" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlDescription" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Description</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlOriginalFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Original Funds</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtOriginalFunds" CssClass="form-control" />
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOriginalFunds"
                            CssClass="text-danger" ErrorMessage="Please supply a Dollar Amount value." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtOriginalFunds"
                            CssClass="text-danger" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCurrentFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Current Funds</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtCurrentFunds" CssClass="form-control" />
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCurrentFunds"
                            CssClass="text-danger" ErrorMessage="Please supply a Dollar Amount value." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCurrentFunds"
                            CssClass="text-danger" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlGrantMaker" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Grant Maker</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlGrantMaker" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-lg-offset-1 col-lg-6 col-md-6 col-sm-4 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" InitialValue="" ControlToValidate="ddlGrantMaker"
                            CssClass="text-danger" ErrorMessage="Please select a Grant Maker from the list"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Button array -->
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Cancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Save_Click" ToolTip="Save this Grant and return to main page" Text="Save" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedGrantID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

    </div>

</asp:Content>
