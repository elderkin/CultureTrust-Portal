<%@ Page Title="Edit Portal User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditRegistration.aspx.cs" Inherits="Portal11.Account.EditRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<%-- Edit the properties of an existing user --%>
    <h2><%: Title %></h2>
    <hr />

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>

    <div class="form-horizontal">
        <asp:Panel runat="server" DefaultButton="btnUpdate">

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtEmail" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="true">Email</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-12">
                <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" TextMode="Email" />
            </div>
            <div class="col-md-5 col-sm-3">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                    CssClass="text-danger" ErrorMessage="The email field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtFullName" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="true">Name</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-12">
                <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" Enabled="true" />
            </div>
            <div class="col-md-5 col-sm-3">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFullName"
                    CssClass="text-danger" ErrorMessage="The Name field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtAddress" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="false">Address</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-12">
                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtAddress" CssClass="form-control" />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtPhoneNumber" CssClass="col-sm-2 control-label" Font-Bold="false">Phone Number</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-12">
                <asp:TextBox runat="server" ID="txtPhoneNumber" CssClass="form-control " />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtGridViewRows" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="false">Rows in Each Grid</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="txtGridViewRows" CssClass="form-control" ToolTip="On every web page of the Portal, include a maximum of this many rows in each table (or grid)." />
            </div>
           <div class="col-md-7">
                <asp:RangeValidator runat="server" Type="Integer" 
                MinimumValue="0" MaximumValue="100" ControlToValidate="txtGridViewRows" 
                CssClass="text-danger" Display="Dynamic" ErrorMessage="Value must be a whole number between 0 and 100" />
           </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="cblOptions" CssClass="col-md-2 col-sm-2 col-xs-12 control-label" Font-Bold="false">Options</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-6">
                <div class="well">
                    <div class="checkbox" style="padding-top: 0">
                        <asp:CheckBoxList ID="cblOptions" runat="server" Style="margin-left: 20px">
                            <asp:ListItem Text="Account is Inactive" Value="Inactive" data-toggle="tooltip"
                                title="Account is 'turned off' and cannot be used to login"></asp:ListItem>
                            <asp:ListItem Text="System Administrator" Value="Administrator" data-toggle="tooltip"
                                title="Create, edit and delete systems structures and user accounts"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="rblRole" CssClass="col-md-2 col-sm-2 col-xs-12 control-label" Font-Bold="false">CT Staff Role</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-6">
                <div class="well">
                    <div class="checkbox" style="padding-top: 0">
                        <asp:RadioButtonList ID="rblRole" runat="server" >
                            <asp:ListItem Text="Not a Staff Member" Value="" Selected="True" data-toggle="tooltip"
                                title="The User is a Project Director or Project Staff, not a CW staff member"></asp:ListItem>
                            <asp:ListItem Text="Auditor" Value="Auditor" data-toggle="tooltip"
                                title="Examines, but does not change, Requests"></asp:ListItem>
                            <asp:ListItem Text="Community Director" Value="CommunityDirector" data-toggle="tooltip"
                                title="Reviews and Approves Requests"></asp:ListItem>
                            <asp:ListItem Text="Finance Director" Value="FinanceDirector" data-toggle="tooltip"
                                title="Reviews and Approves Requests"></asp:ListItem>
                            <asp:ListItem Text="Internal Coordinator" Value="InternalCoordinator" data-toggle="tooltip"
                                title="Create Requests"></asp:ListItem>
                            <asp:ListItem Text="President" Value="President" data-toggle="tooltip"
                                title="Reviews and Approves Requests"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="cblEmailOptions" CssClass="col-md-2 col-sm-2 col-xs-12 control-label" Font-Bold="false">Email Options</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-6">
                <div class="well">
                    <div class="checkbox" style="padding-top: 0">
                        <asp:CheckBoxList ID="cblEmailOptions" runat="server" Style="margin-left: 20px">
<%--                            <asp:ListItem Text="Send Email to Project Director on request approval" Value="EmailOnApprove" data-toggle="tooltip"
                                title="If set, PD will receive email when a request is approved."></asp:ListItem>
                            <asp:ListItem Text="Send Email to Project Director on request return" Value="EmailOnReturn" data-toggle="tooltip"
                                title="If set, PD will receive email when a request is returned"></asp:ListItem>--%>
                            <asp:ListItem Text="Send Email to user when a request awaits their action" Value="EmailOnRefer" data-toggle="tooltip"
                                title="If set, PD, TD, FD, TE will receive email when a request is sent to them"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
        </div>

    <!-- Login Count -->
        <div class="form-group">
            <div class="row">
                <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Login Count</asp:Label>
                <div class="col-md-3 col-sm-5 col-xs-12">
                    <asp:TextBox runat="server" ID="txtLoginCount" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>
        </div>

    <!-- Last Login -->
        <div class="form-group">
            <div class="row">
                <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Last Login</asp:Label>
                <div class="col-md-3 col-sm-5 col-xs-12">
                    <asp:TextBox runat="server" ID="txtLastLogin" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>
        </div>

    <!-- Project list - read only -->
        <div class="form-group">
            <div class="row">
                <asp:Label runat="server" AssociatedControlID="lstProjectMembership" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Project Membership</asp:Label>
                <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                    <!-- Fill this control from code-behind -->
                    <asp:ListBox runat="server" ID="lstProjectMembership" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>
        </div>

    <!-- Button array -->
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnUpdate" runat="server" Text="Save" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnUpdate_Click" ToolTip="Save these updates" />
        </div>
        </asp:Panel>


<%--    Niche to stash context during operation of page--%>
        <asp:Literal runat="server" ID="litSavedUserID" Visible="false" ></asp:Literal>
        <asp:Literal runat="server" ID="litSavedEmail" Visible="false"></asp:Literal>
    </div>

</asp:Content>
