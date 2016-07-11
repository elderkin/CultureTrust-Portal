<%@ Page Title="Change User Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangeUserPassword.aspx.cs" 
    Inherits="Portal11.Account.ChangeUserPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<%--Edit the password of an existing user --%>
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
            <asp:Label runat="server" AssociatedControlID="txtFullName" CssClass="col-md-2 col-sm-2 control-label">Name</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-12">
                <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" Enabled="false" />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtPassword" CssClass="col-md-2 col-sm-2 control-label">New Password</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-12">
                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" Text="Enter the new password for the User" />
            </div>

            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                    CssClass="text-danger" ErrorMessage="The password field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtConfirmPassword" CssClass="col-md-2 col-sm-2 control-label">Confirm password</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-12">
                <asp:Panel runat="server">
                    <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" CssClass="form-control" />
                    <asp:Button ID="btnUserHelper" runat="server" Style="display: none" OnClick="btnUpdate_Click" />
                </asp:Panel>
            </div>
                <div class="col-md-7">
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConfirmPassword"
                    CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." />
                <asp:CompareValidator runat="server" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword"
                    CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
            </div>
        </div>

        <div class="form-group">
            <div class="row">
                <div class="col-md-offset-2 col-md-1 col-xs-2">
                    <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_Click" Text="Cancel" CssClass="btn btn-default" />
                </div>
                <div class="col-md-offset-1 col-md-1 col-xs-offset-1 col-xs-2">
                    <asp:Button runat="server" ID="btnUpdate" OnClick="btnUpdate_Click" Text="Update" CssClass="btn btn-default" />
                </div>
            </div>
        </div>

<%--    Niche to stash context during operation of page--%>
        <asp:Literal runat="server" ID="litSavedUserID" Visible="false" ></asp:Literal>
    </div>

</asp:Content>
