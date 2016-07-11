<%@ Page Title="Edit GL Code" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditGLCode.aspx.cs" 
    Inherits="Portal11.Admin.EditGLCode" %>
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
                    <asp:Label runat="server" AssociatedControlID="txtCode" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="true">Code</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtCode" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCode"
                            CssClass="text-danger" ErrorMessage="Please supply a GL Code." />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlInactive" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="chkInact" CssClass="col-sm-2 col-xs-10 control-label" Font-Bold="false">GL Code is Inactive</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:CheckBox ID="chkInact" runat="server" CssClass="checkbox col-offset-xs-1 col-xs-1"
                            ToolTip="Turn the GLCode off, disabling its use. Note: This action is difficult to undo!" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlDep" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="chkDep" CssClass="col-sm-2 col-xs-10 control-label" Font-Bold="false">For Deposit Requests</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:CheckBox ID="chkDep" runat="server" CssClass="checkbox col-offset-xs-1 col-xs-1"
                            ToolTip="Offer this GL code when creating Deposit Requests" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlExp" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="chkExp" CssClass="col-sm-2 col-xs-10 control-label" Font-Bold="false">For Expense Requests</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:CheckBox ID="chkExp" runat="server" CssClass="checkbox col-offset-xs-1 col-xs-1"
                            ToolTip="Offer this GL Code when creating Expense Requests" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlDescription" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtDescription" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Description</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Button array -->
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Cancel_Click" CausesValidation="false" ToolTip="Return to the main page without saving" />
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Save_Click" ToolTip="Save this GL Code and return to main page" Text="Save" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedGLCodeID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

    </div>

</asp:Content>
