<%@ Page Title="Change My Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangeMyPassword.aspx.cs" 
    Inherits="Portal11.Account.ChangeMyPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %></h2>
    <hr />

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>

    <div class="form-horizontal">
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtCurrentPassword" CssClass="col-md-2 col-sm-2 control-label">Current Password</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="txtCurrentPassword" TextMode="Password" CssClass="form-control" />
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCurrentPassword"
                    CssClass="text-danger" ErrorMessage="The current password field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtNewPassword" CssClass="col-md-2 col-sm-2 control-label">New Password</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" CssClass="form-control" />
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNewPassword"
                    CssClass="text-danger" ErrorMessage="The password field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtConfirmPassword" CssClass="col-md-2 col-sm-2 control-label">Confirm password</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:Panel runat="server" ID="pnlConfirmPassword">
                    <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" CssClass="form-control" />
                    <asp:Button ID="btnPasswordHelper" runat="server" Style="display: none" OnClick="btnChange_Click" />
                </asp:Panel>
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConfirmPassword"
                    CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." />
                <asp:CompareValidator runat="server" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword"
                    CssClass="text-danger" Display="Dynamic" ErrorMessage="The new password and confirmation password do not match." />
            </div>
        </div>

    <!-- Button array -->
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without changing password" />
            <asp:Button ID="btnChange" runat="server" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnChange_Click" ToolTip="Change your password and return to Dashboard" Text="Change" />
        </div>

    </div>

</asp:Content>
